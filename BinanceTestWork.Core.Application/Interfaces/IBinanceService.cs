namespace BinanceTestWork.Core.Application
{
    public interface IBinanceService<T>
    {
        Task<List<T>> GetHistoricalDataAsync(string symbol, DateTime startDate, DateTime endDate, CancellationToken token = default);
    }
}