using Logic.Shared.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.Enums;
using Shared.Models.Scheduler;

namespace Service.Api.Controllers.Scheduler
{
    public class TaskScheduleController : ApiControllerBase
    {
        private readonly IScheduledTasks _scheduledTasks;
        public TaskScheduleController(IScheduledTasks scheduledTasks)
        {
            _scheduledTasks = scheduledTasks;
        }

        [ApiAuthentication]
        [HttpGet(Name = "GetScheduledTasks")]
        public async Task<List<ScheduledTask>> GetScheduledTasks()
        {
            return await _scheduledTasks.GetScheduledTasks();
        }

        [ApiAuthentication(RequiredRole = UserRoleEnum.Admin)]
        [HttpPost(Name = "CreateTask")]
        public async Task CreateTask([FromBody] SceduledTaskRequest request)
        {
            await _scheduledTasks.ScheduleTask(request.Type, request.Message);
        }

        [ApiAuthentication(RequiredRole = UserRoleEnum.Admin)]
        [HttpPost(Name = "ExecuteTask")]
        public async Task ExecuteTask([FromQuery] int  id)
        {
            await _scheduledTasks.ExecuteTask(id);
        }

        [ApiAuthentication(RequiredRole = UserRoleEnum.Admin)]
        [HttpPost(Name = "DeleteTask")]
        public async Task DeleteTask([FromQuery] int id)
        {
            await _scheduledTasks.DeleteTask(id);
        }

        [UserRoleAuthentication(RequiredRole = UserRoleEnum.MaintenanceUser)]
        [HttpPost(Name = "ExecutePendingTasks")]
        public async Task ExecutePendingTasks()
        {
            await _scheduledTasks.ExecutePendingTasks();
        }
    }
}
