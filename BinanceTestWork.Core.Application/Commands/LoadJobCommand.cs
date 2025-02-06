using BinanceTestWork.Core.Application.Abstractions;
using BinanceTestWork.Core.Domain.Entities;
using BinanceTestWork.Core.Domain.Services;
using FluentValidation;
using CryptoExchange.Net.CommonObjects;
using MediatR;
using Microsoft.Extensions.Logging;
using BinanceTestWork.Core.Application.Validators;

namespace BinanceTestWork.Core.Application.Commands
{
    /// <summary>
    /// Класс команды для загрузки задания.
    /// </summary>
    public class LoadJobCommand : IRequest<Guid>
    {
        /// <summary>
        /// Коллекция пар строк.
        /// </summary>
        public ICollection<string> Pairs { get; set; } = null!;

        /// <summary>
        /// Дата начала.
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Дата окончания.
        /// </summary>
        public DateTime? EndDate { get; set; }
    }

    /// <summary>
    /// Класс обработчика команды для загрузки задания.
    /// </summary>
    public class LoadJobCommandHandler : BaseHandler, IRequestHandler<LoadJobCommand, Guid>
    {
        private readonly IBinanceService<Kline> _binanceService;

        /// <summary>
        /// Валидатор команды.
        /// </summary>
        private readonly LoadJobCommandValidator _validator;

        /// <summary>
        /// Конструктор обработчика команды.
        /// </summary>
        /// <param name="repository">Репозиторий.</param>
        /// <param name="validator">Валидатор команды.</param>
        /// <param name="logger">Логгер.</param>
        public LoadJobCommandHandler(IBinanceService<Kline> binanceService, IRepository<Kline> repository, LoadJobCommandValidator validator,
            ILogger<BaseHandler> logger) : base(repository, logger)
        {
            _binanceService = binanceService;
            _validator = validator;
        }

        /// <summary>
        /// Обрабатывает команду.
        /// </summary>
        /// <param name="request">Команда.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <returns>Идентификатор задания.</returns>
        public async Task<Guid> Handle(LoadJobCommand request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
            {
                ShowErrors(validation.Errors);
                return Guid.Empty;
            }

            var jobCreate = Job.Create(request.Pairs, request.StartDate, request.EndDate);

            if (jobCreate.IsFailure)
            {
                ShowError(jobCreate.Error);
                return Guid.Empty;
            }

            var job = jobCreate.Value;

            await _repository.AddAsync(job, cancellationToken);

            try
            {
                var tasks = request.Pairs.Select(async pair =>
                {
                    _logger.LogInformation("Начинаю загрузку данных для валютной пары {Pair}", pair);
                    var data = await _binanceService.GetHistoricalDataAsync(pair, request.StartDate, request.EndDate ?? DateTime.UtcNow, cancellationToken);
                    await _repository.SaveHistoricalDataAsync(pair, data);
                });
                await Task.WhenAll(tasks);

                // Обновление статуса задачи на "Завершено"
                await _repository.UpdateJobStatusAsync(job.Id, JobStatuses.Completed, DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при загрузке данных для задачи {JobId}", job.Id);
                await _repository.UpdateJobStatusAsync(job.Id, JobStatuses.Error);
            }

            return jobCreate.Value.Id;
        }
    }
}