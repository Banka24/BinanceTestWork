using Binance.Net.Clients;
using CryptoExchange.Net.CommonObjects;
using BinanceTestWork.Core.Application;

namespace BinanceTestWork.Infrastructure
{
    /// <summary>
    /// Представляет сервис для работы с Binance API.
    /// </summary>
    public class BinanceService : IBinanceService<Kline>
    {
        /// <summary>
        /// Клиент для работы с Binance API.
        /// </summary>
        private readonly BinanceRestClient _client;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="BinanceService"/>.
        /// </summary>
        /// <param name="client">Клиент для работы с Binance API.</param>
        public BinanceService(BinanceRestClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Получает исторические данные для указанного символа за указанный период времени.
        /// </summary>
        /// <param name="symbol">Символ, для которого нужно получить исторические данные.</param>
        /// <param name="startDate">Дата и время начала периода.</param>
        /// <param name="endDate">Дата и время окончания периода.</param>
        /// <param name="token">Токен отмены операции.</param>
        /// <returns>Список исторических данных.</returns>
        public async Task<List<Kline>> GetHistoricalDataAsync(string symbol, DateTime startDate, DateTime endDate, CancellationToken token = default)
        {
            var klines = new List<Kline>();
            var startTime = startDate;
            var endTime = endDate;

            while (startTime < endTime)
            {
                var result = await _client
                    .SpotApi
                    .CommonSpotClient
                    .GetKlinesAsync(symbol, TimeSpan.FromHours(1), startTime, endTime, 1000, token);

                if (!result.Success)
                {
                    throw new HttpRequestException($"Ошибка при получении данных: {result?.Error!.Message}");
                }

                klines.AddRange(result.Data);
                if (result.Data.Count() < 1000)
                {
                    break;
                }

                startTime = result
                    .Data
                    .Last()
                    .OpenTime
                    .AddHours(1);
            }

            return klines;
        }
    }
}