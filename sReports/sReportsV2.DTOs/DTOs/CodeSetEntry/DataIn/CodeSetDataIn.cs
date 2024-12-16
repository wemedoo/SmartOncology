using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.CodeSetEntry.DataIn
{
    public class CodeSetDataIn
    {
        public int CodeSetId { get; set; }
        public int NewCodeSetId { get; set; }
        public string CodeSetDisplay { get; set; }
        public string RowVersion { get; set; }
        public int ThesaurusEntryId { get; set; }
        public DateTimeOffset ActiveFrom { get; set; }
        public DateTimeOffset ActiveTo { get; set; }
        public bool ApplicableInDesigner { get; set; }
    }
}
