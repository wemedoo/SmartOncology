using sReportsV2.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.FieldInstanceHistory.FieldInstanceHistoryDataIn
{
    public class FieldInstanceHistoryFilterDataIn : DataIn
    {
        public string FormInstanceId { get; set; }
        public int? UserId { get; set; }
        public string FieldLabel { get; set; }
        public string FieldSetLabel { get; set; }
        public bool IncludeIsDeletedInQuery { get; set; }
        public string FieldInstanceRepetitionId { get; set; }
    }
}
