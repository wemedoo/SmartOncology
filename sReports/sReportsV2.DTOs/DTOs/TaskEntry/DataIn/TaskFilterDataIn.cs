using sReportsV2.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.DTOs.TaskEntry.DataIn
{
    public class TaskFilterDataIn : Common.DataIn
    {
        public int? PatientId { get; set; }
        public int? TaskStatus { get; set; }
        public string ActiveLanguage { get; set; }
    }
}
