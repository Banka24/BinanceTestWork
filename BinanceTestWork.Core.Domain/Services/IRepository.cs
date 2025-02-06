using BinanceTestWork.Core.Domain.Entities;

namespace BinanceTestWork.Core.Domain.Services
{
    public interface IRepository<T>
    {
        public Task<bool> AddAsync(Job job, CancellationToken cancellation = default);
        public Task<Job> GetByIdAsync(Guid id, CancellationToken cancellation = default);
        public Task SaveHistoricalDataAsync(string pair, List<T> data);
        public Task UpdateJobStatusAsync(Guid jobId, JobStatuses status, DateTime? endTime = null);
    }
}