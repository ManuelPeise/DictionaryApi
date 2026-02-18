using Shared.Models.Administration;

namespace Logic.Administration.Interfaces
{
    public interface ILogService
    {
        Task<List<LogMessageExportModel>> GetLogMessages();
        Task<List<LogMessageExportModel>> DeleteLogMessage(int id);
        Task<List<LogMessageExportModel>> DeleteLogMessages(List<int> messageIds);
    }
}
