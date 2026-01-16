using Shared.Enums;

namespace Data.Database.Entities
{
    public class LogMessageEntity: AEntityBase
    {
        public string Message { get; set; } = string.Empty;
        public string? ExeptionMessage { get; set; }
        public string? StackTrace { get; set; }
        public string Module { get; set; } = string.Empty;
        public LogMessageTypeEnum LogMessageType { get; set; }
    }
}
