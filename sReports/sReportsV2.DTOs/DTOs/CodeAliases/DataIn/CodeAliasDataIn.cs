using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.CodeAliases.DataIn
{
    public class CodeAliasDataIn
    {
        public int AliasId { get; set; }
        public int CodeId { get; set; }
        public string System { get; set; }
        public string Inbound { get; set; }
        public string Outbound { get; set; }
        public DateTimeOffset ActiveFrom { get; set; }
        public DateTimeOffset ActiveTo { get; set; }
        public int InboundAliasId { get; set; }
        public int? OutboundAliasId { get; set; }
    }
}
