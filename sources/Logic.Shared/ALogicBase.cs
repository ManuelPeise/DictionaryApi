using Logic.Shared.Interfaces;
using Shared.Enums;

namespace Logic.Shared
{
    public abstract class ALogicBase<T> where T : class
    {
        private readonly ILogger<T> _logger;

        public ALogicBase(ILogger<T> logger)
        {
            _logger = logger;
        }

        protected async Task LogMessageAsync(string message, LogMessageTypeEnum type, string? exceptionMessage = null, string? stackTrace = null)
        {
            await _logger.LogMessageAsync(message, type, exceptionMessage, stackTrace);
        }
    }
}
