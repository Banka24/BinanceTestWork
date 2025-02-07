using BinanceTestWork.Core.Application.Commands;
using BinanceTestWork.Core.Application.DTO;
using BinanceTestWork.Core.Application.Querries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BinanceTestWork.API.Controllers
{
    /// <summary>
    /// Контроллер для работы с историческими данными.
    /// </summary>
    [ApiController]
    [Route("api/historical-data")]
    public class HistoricalDataController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<HistoricalDataController> _logger;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="HistoricalDataController"/>.
        /// </summary>
        /// <param name="mediator">Медиатор для отправки запросов и команд.</param>
        public HistoricalDataController(IMediator mediator, ILogger<HistoricalDataController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Загружает исторические данные.
        /// </summary>
        /// <param name="request">Команда для загрузки данных.</param>
        /// <returns>Результат загрузки данных.</returns>
        [HttpPost("load")]
        public async Task<IActionResult> Load([FromBody] LoadJobCommand request)
        {

            if(request is null)
            {
                _logger.LogWarning("Тело запроса обязательно.");
                return BadRequest("Переданы неправильные данные");
            }

            Guid id = default;
            try
            {
                _logger.LogInformation($"Получен запрос на загрузку данных: {request}");
                id = await _mediator.Send(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return id == Guid.Empty ? BadRequest("Данные не соответствуют форме") : Ok(id);
        }

        /// <summary>
        /// Проверяет статус загрузки исторических данных.
        /// </summary>
        /// <param name="jobId">Запрос для проверки статуса.</param>
        /// <returns>Результат проверки статуса.</returns>
        [HttpGet("status")]
        public async Task<IActionResult> Status([FromQuery] string jobId)
        {
            JobDTO jobDTO = null!;
            try
            {
                var request = new CheckStatusJobQuery()
                {
                    JobId = Guid.Parse(jobId)
                };

                jobDTO = await _mediator.Send(request);
            }
            catch(KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, $"Объект с ID {jobId} не найден.");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Произошла непредвиденная ошибка при проверке статуса задания.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Произошла непредвиденная ошибка.");
            }

            return Ok(jobDTO);
        }
    }
}