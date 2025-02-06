namespace BinanceTestWork.Core.Application.Exceptions
{
    /// <summary>
    /// Исключение, которое возникает, когда ключ не найден в коллекции.
    /// </summary>
    public class KeyNotFoundException : Exception
    {
        /// <summary>
        /// Конструктор исключения KeyNotFoundException.
        /// </summary>
        /// <param name="message">Сообщение об ошибке.</param>
        public KeyNotFoundException(string? message) : base(message)
        {
        }
    }
}