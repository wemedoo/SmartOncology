using System;

namespace sReportsV2.DTOs.DTOs.TaskEntry.DataIn
{
    public class TaskDataIn
    {
        public int TaskId { get; set; }
        public int PatientId { get; set; }
        public int EncounterId { get; set; }
        public int TaskTypeCD { get; set; }
        public int TaskStatusCD { get; set; }
        public int? TaskPriorityCD { get; set; }
        public int? TaskClassCD { get; set; }
        public string TaskDescription { get; set; }
        public string TaskEntityId { get; set; }
        public DateTimeOffset TaskStartDateTime { get; set; }
        public DateTimeOffset? TaskEndDateTime { get; set; }
        public DateTimeOffset? ScheduledDateTime { get; set; }
        public DateTimeOffset? LastUpdate { get; set; }
    }
}
