using Shared.Enums;
using Shared.Models.Scheduler;

namespace Logic.Shared.Interfaces
{
    public interface IScheduledTasks
    {
        Task<List<ScheduledTask>> GetScheduledTasks();
        Task ExecuteTask(int id);
        Task ExecutePendingTasks();
        Task ScheduleTask(SceduledTaskRequest request);
        Task DeleteTask(int id);
    }
}
