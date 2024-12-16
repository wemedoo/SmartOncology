using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.TaskEntry.DataOut
{
    public class TaskDocumentDataOut
    {
        public int TaskDocumentId { get; set; }
        public int TaskDocumentCD { get; set; }
        public string FormId { get; set; }
        public string FormTitle { get; set; }
    }
}
