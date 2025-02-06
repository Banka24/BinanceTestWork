using BinanceTestWork.Core.Domain.Entities;
using BinanceTestWork.Core.Domain.Services;
using CryptoExchange.Net.CommonObjects;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BinanceTestWork.Infrastructure
{
    /// <summary>
    /// Представляет реализацию репозитория для работы с MongoDB.
    /// </summary>
    public class MongoDBRepository : IRepository<Kline>
    {
        /// <summary>
        /// Логгер для записи сообщений об ошибках.
        /// </summary>
        private readonly ILogger<MongoDBRepository> _logger;

        /// <summary>
        /// Коллекция MongoDB для работы с данными.
        /// </summary>
        private readonly IMongoCollection<Job> _mongoCollection;

        /// <summary>
        /// База данных MongoDB.
        /// </summary>
        private readonly IMongoDatabase _database;

        /// <summary>
        /// Конструктор класса MongoDBRepository.
        /// </summary>
        /// <param name="logger">Логгер для записи сообщений об ошибках.</param>
        /// <param name="mongoClient">Клиент MongoDB.</param>
        /// <param name="databaseName">Имя базы данных.</param>
        /// <param name="collectionName">Имя коллекции.</param>
        public MongoDBRepository(ILogger<MongoDBRepository> logger, IMongoClient mongoClient, string databaseName, string collectionName)
        {
            _logger = logger;
            _database = mongoClient.GetDatabase(databaseName);
            _mongoCollection = _database.GetCollection<Job>(collectionName);
        }

        /// <summary>
        /// Добавляет новую работу в коллекцию MongoDB.
        /// </summary>
        /// <param name="job">Работа для добавления.</param>
        /// <param name="cancellation">Токен отмены операции.</param>
        /// <returns>True, если операция прошла успешно, иначе false.</returns>
        public async Task<bool> AddAsync(Job job, CancellationToken cancellation = default)
        {
            try
            {
                await _mongoCollection.InsertOneAsync(job, new InsertOneOptions(), cancellation);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Получает работу по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор работы.</param>
        /// <param name="cancellation">Токен отмены операции.</param>
        /// <returns>Работа, если она найдена, иначе null.</returns>
        public async Task<Job> GetByIdAsync(Guid id, CancellationToken cancellation = default)
        {
            var filter = Builders<Job>.Filter.Eq(j => j.Id, id);
            Job job = null!;

            try
            {
                job = await _mongoCollection
                    .Find(filter)
                    .FirstOrDefaultAsync(cancellation);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex.Message);
            }

            return job;
        }

        /// <summary>
        /// Обновляет статус работы и, при необходимости, время окончания.
        /// </summary>
        /// <param name="jobId">Идентификатор работы.</param>
        /// <param name="status">Новый статус работы.</param>
        /// <param name="endTime">Время окончания работы (опционально).</param>
        public async Task UpdateJobStatusAsync(Guid jobId, JobStatuses status, DateTime? endTime = null)
        {
            var update = Builders<Job>.Update.Set(j => j.Status, status);
            if (endTime.HasValue)
            {
                update = update.Set(j => j.EndDate, endTime.Value);
            }

            try
            {
                await _mongoCollection.UpdateOneAsync(j => j.Id.Equals(jobId), update);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        /// <summary>
        /// Сохраняет исторические данные в коллекцию MongoDB.
        /// </summary>
        /// <param name="pair">Имя коллекции для сохранения данных.</param>
        /// <param name="data">Список объектов для сохранения.</param>
        public async Task SaveHistoricalDataAsync(string pair, List<Kline> data)
        {
            var collection = _database.GetCollection<BsonDocument>(pair);
            var documents = data.Select(k => new BsonDocument
            {
                { "OpenTime", k.OpenTime },
                { "OpenPrice", k.OpenPrice },
                { "HighPrice", k.HighPrice },
                { "LowPrice", k.LowPrice },
                { "ClosePrice", k.ClosePrice },
                { "Volume", k.Volume }
            });

            try
            {
                await collection.InsertManyAsync(documents);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex.Message);
            }
            return;
        }
    }
}