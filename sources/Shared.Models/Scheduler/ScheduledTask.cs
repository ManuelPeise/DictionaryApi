using Shared.Enums;

namespace Shared.Models.Scheduler
{
    public class ScheduledTask
    {
        public int Id { get; set; }
        public ScheduledTaskType Type { get; set; }
        public ScheduledTaskStatus Status { get; set; }
        public string RequestUrl { get; set; } = string.Empty;
        public string? Message { get; set; }
        public ScheduleInterval Interval { get; set; } = ScheduleInterval.None;
        public DateTime? ScheduledFireTime { get; set; }
        public DateTime? LastFireTime { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; }
    }
}
