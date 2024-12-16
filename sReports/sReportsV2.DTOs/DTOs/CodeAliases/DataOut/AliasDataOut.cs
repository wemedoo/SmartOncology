using System;
using sReportsV2.DTOs.CodeEntry.DataOut;

namespace sReportsV2.DTOs.DTOs.CodeAliases.DataOut
{
    public class AliasDataOut
    {
        public int AliasId { get; set; }
        public string Alias { get; set; }
        public int CodeId { get; set; }
        public virtual CodeDataOut Code { get; set; }
        public string System { get; set; }
        public DateTimeOffset ActiveFrom { get; set; }
        public DateTimeOffset ActiveTo { get; set; }
    }
}
