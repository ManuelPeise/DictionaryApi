using Data.Database;
using Data.Database.Entities;
using Logic.Shared.Interfaces;
using Shared.Enums;

namespace Logic.Shared
{
    public class Logger<T>: ILogger<T> where T : class
    {
        private readonly DatabaseContext _context;
       
        public Logger(DatabaseContext context)
        {
            _context = context;
           
        }

        public async Task LogMessageAsync(string message, LogMessageTypeEnum type, string? exceptionMessage = null, string? stackTrace = null)
        {
            var logEntry = new LogMessageEntity
            {
                Message = message,
                ExeptionMessage = exceptionMessage,
                StackTrace = stackTrace,
                LogMessageType = type,
                Module = typeof(T).Name,
                CreatedBy = "System",
                CreatedAt = DateTime.UtcNow
            };
            
            _context.LogTable.Add(logEntry);
            
            await _context.SaveChangesAsync();
        }
    }
}
