using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using sReportsV2.Domain.Sql.Entities.User;
using System.Collections.Generic;
using System.Linq;
using sReportsV2.Common.Enums;
using sReportsV2.Domain.Sql.Entities.Common;
using System;
using sReportsV2.Common.Extensions;

namespace sReportsV2.Domain.Sql.Entities.TaskEntry
{
    public class Task : EntitiesBase.Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("TaskId")]
        public int TaskId { get; set; }
        public int PatientId { get; set; }
        [ForeignKey("PatientId")]
        public Patient.Patient Patient { get; set; }
        public int EncounterId { get; set; }
        [ForeignKey("EncounterId")]
        public Encounter.Encounter Encounter { get; set; }
        public int TaskTypeCD { get; set; }
        [ForeignKey("TaskTypeCD")]
        public Code TaskType { get; set; }
        public int TaskStatusCD { get; set; }
        [ForeignKey("TaskStatusCD")]
        public Code TaskStatus { get; set; }
        public int? TaskPriorityCD { get; set; }
        [ForeignKey("TaskPriorityCD")]
        public Code TaskPriority { get; set; }
        public int? TaskClassCD { get; set; }
        [ForeignKey("TaskClassCD")]
        public Code TaskClass { get; set; }
        public string TaskDescription { get; set; }
        public string TaskEntityId { get; set; }
        public DateTimeOffset TaskStartDateTime { get; set; }
        public DateTimeOffset? TaskEndDateTime { get; set; }
        public DateTimeOffset? ScheduledDateTime { get; set; }
        public int? TaskDocumentId { get; set; }
        [ForeignKey("TaskDocumentId")]
        public TaskDocument TaskDocument { get; set; }

        public void Copy(Task task)
        {
            this.TaskTypeCD = task.TaskTypeCD;
            this.TaskStatusCD = task.TaskStatusCD;
            this.TaskPriorityCD = task.TaskPriorityCD;
            this.TaskClassCD = task.TaskClassCD;
            this.TaskDescription = task.TaskDescription;
            this.TaskEntityId = task.TaskEntityId;
            this.TaskStartDateTime = task.TaskStartDateTime;
            this.TaskEndDateTime = task.TaskEndDateTime;
            this.ScheduledDateTime = task.ScheduledDateTime;
            this.TaskDocumentId = task.TaskDocumentId;
        }
    }
}
