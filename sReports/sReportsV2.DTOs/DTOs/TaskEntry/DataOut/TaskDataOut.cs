using sReportsV2.DTOs.CodeEntry.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.DTOs.DTOs.TaskEntry.DataOut
{
    public class TaskDataOut
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
        public int? TaskDocumentCD { get; set; }
        public TaskDocumentDataOut TaskDocument { get; set; }

        public string ConvertTaskTypeCDToDisplayName(List<CodeDataOut> types, string language)
        {
            return types.Find(x => x.Id == this.TaskTypeCD)?.Thesaurus.GetPreferredTermByTranslationOrDefault(language);
        }

        public string ConvertTaskStatusCDToDisplayName(List<CodeDataOut> statuses, string language)
        {
            return statuses.Find(x => x.Id == this.TaskStatusCD)?.Thesaurus.GetPreferredTermByTranslationOrDefault(language);
        }
    }
}
