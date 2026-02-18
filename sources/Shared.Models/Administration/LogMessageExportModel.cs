using Shared.Enums;

namespace Shared.Models.Administration
{
    public class LogMessageExportModel
    {
        public int Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? ExeptionMessage { get; set; }
        public string? StackTrace { get; set; }
        public string Module { get; set; } = string.Empty;
        public LogMessageTypeEnum LogMessageType { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
