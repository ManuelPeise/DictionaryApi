using Shared.Enums;

namespace Data.Database.Entities
{
    public class ScheduledTaskEntity: AEntityBase
    {
        public ScheduledTaskType Type { get; set; }
        public ScheduledTaskStatus Status { get; set; }
        public string RequestUrl { get; set; } = string.Empty;
        public string? Message { get; set; }
        public ScheduleInterval Interval { get; set; } = ScheduleInterval.None;
        public DateTime? ScheduledFireTime { get; set; }
        public DateTime? LastFireTime { get; set; }
        
    }
}
