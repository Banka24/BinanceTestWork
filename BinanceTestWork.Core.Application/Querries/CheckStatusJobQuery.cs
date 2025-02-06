using BinanceTestWork.Core.Application.Abstractions;
using BinanceTestWork.Core.Application.DTO;
using BinanceTestWork.Core.Application.Validators;
using BinanceTestWork.Core.Domain.Services;
using CryptoExchange.Net.CommonObjects;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BinanceTestWork.Core.Application.Querries
{
    /// <summary>
    /// Запрос для проверки статуса работы.
    /// </summary>
    public class CheckStatusJobQuery : IRequest<JobDTO>
    {
        /// <summary>
        /// Идентификатор работы.
        /// </summary>
        public Guid JobId { get; set; }
    }

    /// <summary>
    /// Обработчик запроса для проверки статуса работы.
    /// </summary>
    public class CheckStatusJobQueryHandler : BaseHandler, IRequestHandler<CheckStatusJobQuery, JobDTO>
    {
        /// <summary>
        /// Валидатор запроса.
        /// </summary>
        private readonly CheckStatusJobQueryValidator _validator;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="CheckStatusJobQueryHandler"/>.
        /// </summary>
        /// <param name="repository">Репозиторий для работы с данными.</param>
        /// <param name="validator">Валидатор запроса.</param>
        /// <param name="logger">Логгер для записи сообщений.</param>
        public CheckStatusJobQueryHandler(IRepository<Kline> repository, CheckStatusJobQueryValidator validator,
            ILogger<BaseHandler> logger) : base(repository, logger)
        {
            _validator = validator;
        }

        /// <summary>
        /// Обрабатывает запрос на проверку статуса работы.
        /// </summary>
        /// <param name="request">Запрос на проверку статуса работы.</param>
        /// <param name="cancellationToken">Токен отмены операции.</param>
        /// <returns>DTO объекта работы.</returns>
        public async Task<JobDTO> Handle(CheckStatusJobQuery request, CancellationToken cancellationToken)
        {
            var validation = _validator.Validate(request);
            if (!validation.IsValid)
            {
                ShowErrors(validation.Errors);
                return null!;
            }

            var job = await _repository.GetByIdAsync(request.JobId, cancellationToken);

            return job is null
                ? throw new KeyNotFoundException($"Задача с идентификатором {request.JobId} не найдена.")
                : new JobDTO(job.Id, job.Status.ToString(), job.EndDate);
        }
    }
}