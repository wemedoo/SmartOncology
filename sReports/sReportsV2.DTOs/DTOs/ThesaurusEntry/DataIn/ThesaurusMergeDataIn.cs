using sReportsV2.Common.Enums;
using System.Collections.Generic;

namespace sReportsV2.DTOs.ThesaurusEntry.DataIn
{
    public class ThesaurusMergeDataIn
    {
        public int CurrentId { get; set; }
        public int TargetId { get; set; }
        public List<string> ValuesForMerge { get; set; }
        public int? StateCD { get; set; }
    }
}