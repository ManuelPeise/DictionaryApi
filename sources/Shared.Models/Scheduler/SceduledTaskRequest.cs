using Shared.Enums;

namespace Shared.Models.Scheduler
{
    public class SceduledTaskRequest
    {
        public ScheduledTaskType Type { get; set; }
        public string Message { get; set; } = string.Empty;
        public ScheduleInterval Interval { get; set; } = ScheduleInterval.None;
        public DateTime? FireTime { get; set; }
    }
}
