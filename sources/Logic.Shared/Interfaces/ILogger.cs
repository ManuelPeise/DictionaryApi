using Shared.Enums;

namespace Logic.Shared.Interfaces
{
    public interface ILogger<T> where T : class
    {
        Task LogMessageAsync(string message, LogMessageTypeEnum type, string? exceptionMessage = null, string? stackTrace = null);
    }
}
