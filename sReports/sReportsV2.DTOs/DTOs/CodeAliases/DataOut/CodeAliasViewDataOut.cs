using System;

namespace sReportsV2.DTOs.DTOs.CodeAliases.DataOut
{
    public class CodeAliasViewDataOut
    {
        public int AliasId { get; set; }
        public int CodeId { get; set; }
        public int CodeSetId { get; set; }
        public string System { get; set; }
        public string InboundAlias { get; set; }
        public string OutboundAlias { get; set; }
        public DateTimeOffset ActiveFrom { get; set; }
        public DateTimeOffset ActiveTo { get; set; }
        public int InboundAliasId { get; set; }
        public int? OutboundAliasId { get; set; }
    }
}
