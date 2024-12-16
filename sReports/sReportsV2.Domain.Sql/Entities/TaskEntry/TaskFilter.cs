using sReportsV2.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.TaskEntry
{
    public class TaskFilter : EntityFilter
    {
        public int? PatientId { get; set; }
        public int? TaskStatus { get; set; }
        public string ActiveLanguage { get; set; }
    }
}
