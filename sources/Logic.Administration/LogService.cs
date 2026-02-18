using Data.Accessor.Interfaces;
using Data.Database;
using Data.Database.Entities;
using Logic.Administration.Interfaces;
using Logic.Shared;
using Microsoft.AspNetCore.Http;
using Shared.Enums;
using Shared.Models.Administration;

namespace Logic.Administration
{
    public class LogService : LogicBase, ILogService
    {
        private readonly Logger<LogService> _logger;
        public LogService(DatabaseContext dbContext, IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork)
            : base(dbContext, httpContextAccessor, unitOfWork)
        {
            _logger = new Logger<LogService>(dbContext);
        }

        public async Task<List<LogMessageExportModel>> GetLogMessages()
        {
            var logMessages = new List<LogMessageExportModel>();

            try
            {
                var logMessageEntities = await UnitOfWork.AdministrationUnitOfWork.LogRepository.GetAllAsync();

                if (!logMessageEntities.Any())
                {
                    throw new Exception("Could not load log messages from database.");
                }

                return GetExportModels(logMessageEntities);

            }
            catch (Exception exception)
            {
                await _logger.LogMessageAsync(
                    "Could not load log messages from database.",
                    LogMessageTypeEnum.Error,
                    exception.Message,
                    exception.StackTrace);

                return logMessages;
            }
        }

        public async Task<List<LogMessageExportModel>> DeleteLogMessage(int id)
        {
            var messages = new List<LogMessageExportModel>();

            try
            {
                var currentUser = GetCurrentUser();

                var messageEntity = await UnitOfWork.AdministrationUnitOfWork.LogRepository.FirstOrDefaultByIdAsync(id);

                if(messageEntity == null)
                {
                    throw new Exception($"Could not delete log message [{id}].");
                }

                await UnitOfWork.AdministrationUnitOfWork.LogRepository.DeleteAsync(messageEntity);

                await UnitOfWork.SaveChangesAsync(currentUser.EmailAddress);

                var logMessageEntities = await UnitOfWork.AdministrationUnitOfWork.LogRepository.GetAllAsync();
                
                messages = GetExportModels(logMessageEntities);
            }
            catch (Exception exception)
            {
                await _logger.LogMessageAsync(
                    $"Could not delete log message [{id}].", 
                    LogMessageTypeEnum.Error, 
                    exception.Message, 
                    exception.StackTrace);
            }

            return messages;
        }

        public async Task<List<LogMessageExportModel>> DeleteLogMessages(List<int> messageIds)
        {
            var messages = new List<LogMessageExportModel>();

            try
            {
                var currentUser = GetCurrentUser();

                var messageEntities = await UnitOfWork.AdministrationUnitOfWork.LogRepository.GetAllByAsync(e => messageIds.Contains(e.Id));

                if (messageEntities == null || !messageEntities.Any())
                {
                    throw new Exception("Could not delete log messages.");
                }

                await UnitOfWork.AdministrationUnitOfWork.LogRepository.BulkDelete(messageEntities);

                await UnitOfWork.SaveChangesAsync(currentUser.EmailAddress);

                var logMessageEntities = await UnitOfWork.AdministrationUnitOfWork.LogRepository.GetAllAsync();

                messages = GetExportModels(logMessageEntities);
            }
            catch (Exception exception)
            {
                await _logger.LogMessageAsync(
                    "Could not delete log messages.",
                    LogMessageTypeEnum.Error,
                    exception.Message,
                    exception.StackTrace);
            }

            return messages;
        }

        private List<LogMessageExportModel> GetExportModels(HashSet<LogMessageEntity> logMessages)
        {
            return logMessages.Select(e => new LogMessageExportModel
            {
                Id = e.Id,
                Message = e.Message,
                ExeptionMessage = e.ExeptionMessage,
                LogMessageType = e.LogMessageType,
                Module = e.Module,
                StackTrace = e.StackTrace,
                TimeStamp = e.CreatedAt
            }).ToList();
        }
    }
}
