using Data.Accessor.Interfaces;
using Data.Database;
using Data.Database.Entities;
using Logic.Shared.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Shared.Enums;
using Shared.Models.Scheduler;
using Shared.Models.Settings;
using System.Net;

namespace Logic.Shared
{
    public class ScheduledTasks : LogicBase, IScheduledTasks
    {
        private readonly Logger<ScheduledTasks> _logger;
        private readonly ApiSettings _apiSettings;
        private readonly HttpClient _httpClient;

        public ScheduledTasks(
            DatabaseContext dbContext,
            IHttpContextAccessor httpContextAccessor,
            IUnitOfWork unitOfWork,
            IOptions<ApiSettings> apiSettings)
            : base(dbContext, httpContextAccessor, unitOfWork)
        {
            _logger = new Logger<ScheduledTasks>(dbContext);
            _apiSettings = apiSettings.Value;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(_apiSettings.BaseUrl)
            };
        }

        public async Task<List<ScheduledTask>> GetScheduledTasks()
        {
            try
            {
                var tasks = await UnitOfWork.ScheduledTaskRepository.GetAllAsync();

                return tasks.Select(t => new ScheduledTask
                {
                    Id = t.Id,
                    Type = t.Type,
                    Status = t.Status,
                    RequestUrl = t.RequestUrl,
                    Message = t.Message,
                    Interval = t.Interval,
                    ScheduledFireTime = t.ScheduledFireTime,
                    LastFireTime = t.LastFireTime,
                    CreatedBy = t.CreatedBy,
                    CreatedAt = t.CreatedAt,
                    UpdatedBy = t.UpdatedBy,
                    UpdatedAt = t.UpdatedAt
                }).ToList();
            }
            catch (Exception exception)
            {
                await _logger.LogMessageAsync(
                    "Error getting scheduled tasks",
                    LogMessageTypeEnum.Error,
                    exception.Message,
                    exception.StackTrace);

                return new List<ScheduledTask>();
            }
        }

        public async Task ExecuteTask(int id)
        {
            try
            {
                var currentUser = GetCurrentUser();

                var task = await UnitOfWork.ScheduledTaskRepository.FirstOrDefaultByIdAsync(id);

                if (task == null)
                {
                    throw new Exception($"Scheduled task with id {id} not found.");
                }

                await ExecuteTask(task);

                await UnitOfWork.SaveChangesAsync(currentUser.EmailAddress);
            }
            catch (Exception exception)
            {
                await _logger.LogMessageAsync(
                    $"Error executing scheduled task with id {id}",
                    LogMessageTypeEnum.Error,
                    exception.Message,
                    exception.StackTrace);
            }
        }

        public async Task DeleteTask(int id)
        {
            try
            {
                var currentUser = GetCurrentUser();

                var task = await UnitOfWork.ScheduledTaskRepository.FirstOrDefaultByIdAsync(id);

                if (task == null)
                {
                    throw new Exception($"Scheduled task with id {id} not found.");
                }

                await UnitOfWork.ScheduledTaskRepository.DeleteAsync(task);

                await UnitOfWork.SaveChangesAsync(currentUser.EmailAddress);
            }
            catch (Exception exception)
            {
                await _logger.LogMessageAsync(
                    $"Error executing scheduled task with id {id}",
                    LogMessageTypeEnum.Error,
                    exception.Message,
                    exception.StackTrace);
            }
        }

        public async Task ExecutePendingTasks()
        {
            try
            {

                var pendingTasks = await GetPendingTasks();

                foreach (var task in pendingTasks)
                {
                    await ExecuteTask(task);

                    // await UnitOfWork.ScheduledTaskRepository.UpdateAsync(task);
                }

                await UnitOfWork.SaveChangesAsync("System");
            }
            catch (Exception exception)
            {
                await _logger.LogMessageAsync(
                    "Error executing pending scheduled tasks",
                    LogMessageTypeEnum.Error,
                    exception.Message,
                    exception.StackTrace);
                throw;
            }
        }

        public async Task ScheduleTask(SceduledTaskRequest request)
        {
            try
            {
                var currentUser = GetCurrentUser();

                var scheduledTask = new ScheduledTaskEntity
                {
                    Type = request.Type,
                    Message = request.Message,
                    Interval = request.Interval,
                    ScheduledFireTime = request.FireTime,
                    LastFireTime = null,
                    Status = ScheduledTaskStatus.Pending,
                    RequestUrl = GenerateRequestUrl(request.Type),
                };

                await UnitOfWork.ScheduledTaskRepository.AddAsync(scheduledTask);

                await UnitOfWork.SaveChangesAsync(currentUser.EmailAddress);
            }
            catch (Exception exception)
            {
                await _logger.LogMessageAsync(
                    $"Error while scheduling new task type of {request.Type.ToString()}.",
                    LogMessageTypeEnum.Error,
                    exception.Message,
                    exception.StackTrace);
                throw;
            }
        }

        private string GenerateRequestUrl(ScheduledTaskType taskType)
        {
            string url = string.Empty;

            switch (taskType)
            {
                case ScheduledTaskType.VocabularyImportService:
                    url = $"VocabularyImport/ExecuteVocabularyTask";
                    break;
                case ScheduledTaskType.KaikkiWordService:
                    url = $"";
                    break;
                default:
                    throw new NotImplementedException($"No execution logic implemented for task type {taskType}");
            }

            return url;
        }

        private async Task<List<ScheduledTaskEntity>> GetPendingTasks()
        {
            try
            {
                var pendingTasks = await UnitOfWork.ScheduledTaskRepository
                    .GetAllByAsync(t => t.Status == ScheduledTaskStatus.Pending, null, false);

                return pendingTasks.ToList();
            }
            catch (Exception exception)
            {
                await _logger.LogMessageAsync(
                    "Error getting pending scheduled tasks",
                    LogMessageTypeEnum.Error,
                    exception.Message,
                    exception.StackTrace);
                throw;
            }
        }

        private async Task ExecuteTask(ScheduledTaskEntity task, DateTime executionTime)
        {
            try
            {
                var requestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(task.RequestUrl, UriKind.Relative),
                    Version = HttpVersion.Version11
                };

                var response = await _httpClient.SendAsync(requestMessage);

                response.EnsureSuccessStatusCode();

                if (task.Interval == ScheduleInterval.None)
                {
                    task.Status = ScheduledTaskStatus.Completed;
                    task.LastFireTime = executionTime;
                }
                else
                {
                    task.Status = ScheduledTaskStatus.InProgress;
                    task.ScheduledFireTime = executionTime.Add(GetIntervalTimeSpan(task.Interval));
                    task.LastFireTime = executionTime;
                }

                task.Message = $"Task executed successfully with status code {response.StatusCode}";
            }
            catch (Exception exception)
            {
                task.Status = ScheduledTaskStatus.Failed;
                task.Message = $"Task execution failed with error: {exception.Message}";

                await _logger.LogMessageAsync(
                    $"Error executing scheduled task with id {task.Id}",
                    LogMessageTypeEnum.Error,
                    exception.Message,
                    exception.StackTrace);
            }

        }

        private TimeSpan GetIntervalTimeSpan(ScheduleInterval interval)
        {
            switch (interval)
            {
                case ScheduleInterval.None:
                    return TimeSpan.Zero;
                case ScheduleInterval.Hourly:
                    return TimeSpan.FromHours(1);
                case ScheduleInterval.QuarterHourly:
                    return TimeSpan.FromMinutes(15);
                case ScheduleInterval.Daily:
                    return TimeSpan.FromDays(1);
                case ScheduleInterval.Weekly:
                    return TimeSpan.FromDays(7);
                case ScheduleInterval.Monthly:
                    return TimeSpan.FromDays(30);
                default:
                    throw new NotImplementedException($"No timespan defined for interval {interval}");
            }
        }
    }
}
