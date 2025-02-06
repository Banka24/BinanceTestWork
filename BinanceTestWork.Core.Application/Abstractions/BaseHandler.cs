using BinanceTestWork.Core.Domain.Services;
using CryptoExchange.Net.CommonObjects;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;

namespace BinanceTestWork.Core.Application.Abstractions
{
    /// <summary>
    /// Базовый класс обработчика, который содержит общую функциональность для всех обработчиков.
    /// </summary>
    public abstract class BaseHandler
    {
        /// <summary>
        /// Репозиторий для работы с данными.
        /// </summary>
        protected readonly IRepository<Kline> _repository;

        /// <summary>
        /// Логгер для записи сообщений об ошибках.
        /// </summary>
        protected readonly ILogger<BaseHandler> _logger;

        /// <summary>
        /// Конструктор базового обработчика.
        /// </summary>
        /// <param name="repository">Репозиторий для работы с данными.</param>
        /// <param name="logger">Логгер для записи сообщений об ошибках.</param>
        protected BaseHandler(IRepository<Kline> repository, ILogger<BaseHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        /// <summary>
        /// Метод для отображения ошибок валидации.
        /// </summary>
        /// <param name="errors">Коллекция ошибок валидации.</param>
        protected virtual void ShowErrors(ICollection<ValidationFailure> errors)
        {
            foreach (var error in errors)
            {
                _logger.LogError($"Код ошибки: {error.ErrorCode}," +
                    $" Свойство: {error.PropertyName}, " +
                    $"Сообщение:{error.ErrorMessage}");
            }
        }

        /// <summary>
        /// Метод для отображения ошибки.
        /// </summary>
        /// <param name="error">Сообщение об ошибке.</param>
        protected virtual void ShowError(string error)
        {
            _logger.LogError($"Соообщение ошибки: {error}");
        }
    }
}