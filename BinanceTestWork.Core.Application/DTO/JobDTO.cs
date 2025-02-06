namespace BinanceTestWork.Core.Application.DTO
{
    /// <summary>
    /// Класс DTO для передачи данных о работе.
    /// </summary>
    public record class JobDTO
    {
        /// <summary>
        /// Идентификатор работы.
        /// </summary>
        public Guid JobId { get; init; }

        /// <summary>
        /// Статус работы.
        /// </summary>
        public string Status { get; init; }

        /// <summary>
        /// Дата окончания работы.
        /// </summary>
        public DateTime? EndDate { get; init; }

        /// <summary>
        /// Конструктор класса JobDTO.
        /// </summary>
        /// <param name="jobId">Идентификатор работы.</param>
        /// <param name="status">Статус работы.</param>
        /// <param name="endDate">Дата окончания работы.</param>
        public JobDTO(Guid jobId, string status, DateTime? endDate)
        {
            JobId = jobId;
            Status = status;
            EndDate = endDate;
        }
    }
}