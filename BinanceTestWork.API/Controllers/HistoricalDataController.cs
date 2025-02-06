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

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="HistoricalDataController"/>.
        /// </summary>
        /// <param name="mediator">Медиатор для отправки запросов и команд.</param>
        public HistoricalDataController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Загружает исторические данные.
        /// </summary>
        /// <param name="request">Команда для загрузки данных.</param>
        /// <returns>Результат загрузки данных.</returns>
        [HttpPost("load")]
        public async Task<IActionResult> Load([FromBody] LoadJobCommand request)
        {
            var id = await _mediator.Send(request);

            return id == Guid.Empty ? BadRequest("Данные не соответствуют форме") : Ok(id);
        }

        /// <summary>
        /// Проверяет статус загрузки исторических данных.
        /// </summary>
        /// <param name="request">Запрос для проверки статуса.</param>
        /// <returns>Результат проверки статуса.</returns>
        [HttpGet("status")]
        public async Task<IActionResult> Status([FromQuery] CheckStatusJobQuery request)
        {
            string errorMessage = "";
            JobDTO jobDTO = null!;
            try
            {
                jobDTO = await _mediator.Send(request);
            }
            catch(KeyNotFoundException ex)
            {
                errorMessage = ex.Message;
            }

            return jobDTO is null ? BadRequest(errorMessage) : Ok(jobDTO);
        }
    }
}