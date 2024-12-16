using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.CodeSetEntry.DataIn
{
    public class CodeSetFilterDataIn : Common.DataIn
    {
        public int? CodeSetId { get; set; }
        public bool ShowActive { get; set; }
        public bool ShowInactive { get; set; }
        public string CodeSetDisplay { get; set; }
        public string ActiveLanguage { get; set; }
    }
}
