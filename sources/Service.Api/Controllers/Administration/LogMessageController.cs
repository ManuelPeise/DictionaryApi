using Logic.Administration.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.Enums;
using Shared.Models.Administration;

namespace Service.Api.Controllers.Administration
{
    public class LogMessageController: ApiControllerBase
    {
        private readonly ILogService _logService;

        public LogMessageController(ILogService logService)
        {
            _logService = logService;
        }

        [UserRoleAuthentication(RequiredRole = UserRoleEnum.Admin)]
        [HttpGet(Name = "GetLogMessages")]
        public async Task<List<LogMessageExportModel>> GetLogMessages()
        {
            return await _logService.GetLogMessages();
        }

        [UserRoleAuthentication(RequiredRole = UserRoleEnum.Admin)]
        [HttpGet(Name = "DeleteMessage")]
        public async Task<List<LogMessageExportModel>> DeleteMessage([FromQuery] int messageId)
        {
            return await _logService.DeleteLogMessage(messageId);
        }

        [UserRoleAuthentication(RequiredRole = UserRoleEnum.Admin)]
        [HttpGet(Name = "DeleteMessages")]
        public async Task<List<LogMessageExportModel>> DeleteMessages([FromQuery] List<int> messageIds)
        {
            return await _logService.DeleteLogMessages(messageIds);
        }
    }
}
