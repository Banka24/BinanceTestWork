namespace BinanceTestWork.Infrastructure
{
    /// <summary>
    /// Класс BinanceApiSettings представляет настройки для API Binance.
    /// </summary>
    public class BinanceApiSettings
    {
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }

        public BinanceApiSettings()
        {
            
        }

        public BinanceApiSettings(string apiKey, string apiSecret)
        {
            ApiKey = apiKey;
            ApiSecret = apiSecret;
        }
    }
}