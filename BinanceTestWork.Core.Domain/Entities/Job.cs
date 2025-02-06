using CSharpFunctionalExtensions;

namespace BinanceTestWork.Core.Domain.Entities
{
    /// <summary>
    /// Класс Job представляет задание, которое может быть создано с помощью метода Create.
    /// </summary>
    public class Job : Entity<Guid>
    {
        /// <summary>
        /// Список пар строк, связанных с заданием.
        /// </summary>
        public ICollection<string> Pairs { get; private init; }

        /// <summary>
        /// Статус задания.
        /// </summary>
        public JobStatuses Status { get; private init; }

        /// <summary>
        /// Дата начала задания.
        /// </summary>
        public DateTime StartDate { get; private init; }

        /// <summary>
        /// Дата окончания задания.
        /// </summary>
        public DateTime? EndDate { get; private init; }

        /// <summary>
        /// Конструктор класса Job.
        /// </summary>
        /// <param name="id">Уникальный идентификатор задания.</param>
        /// <param name="pairs">Список пар строк, связанных с заданием.</param>
        /// <param name="status">Статус задания.</param>
        /// <param name="startDate">Дата начала задания.</param>
        /// <param name="endDate">Дата окончания задания.</param>
        private Job(Guid id, ICollection<string> pairs, JobStatuses status,
            DateTime startDate, DateTime? endDate)
        {
            Id = id;
            Pairs = pairs;
            Status = status;
            StartDate = startDate;
            EndDate = endDate;
        }

        /// <summary>
        /// Создает новое задание.
        /// </summary>
        /// <param name="pairs">Список пар строк, связанных с заданием.</param>
        /// <param name="startDate">Дата начала задания.</param>
        /// <param name="endDate">Дата окончания задания.</param>
        /// <returns>Результат создания задания.</returns>
        public static Result<Job> Create(ICollection<string> pairs, DateTime startDate, DateTime? endDate)
        {
            if (pairs.Count <= 0 || pairs is null)
                return Result.Failure<Job>("Количествно валютных пар не может быть пустым или меньше 0");

            if (startDate != default)
                return Result.Failure<Job>("Дата не может быть пустой");

            var job = new Job(Guid.NewGuid(), pairs, JobStatuses.InProcessing, startDate, endDate);

            return Result.Success(job);
        }
    }

    public enum JobStatuses
    {
        InProcessing,
        Completed,
        Error
    }
}