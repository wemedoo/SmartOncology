using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.TaskEntry.DataIn
{
    public class TaskDocumentDataIn
    {
        public int TaskDocumentCD { get; set; }
        public string FormId { get; set; }
        public string FormTitle { get; set; }
    }
}
