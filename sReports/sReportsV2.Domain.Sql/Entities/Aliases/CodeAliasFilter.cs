using sReportsV2.Common.Entities;
using System;

namespace sReportsV2.Domain.Sql.Entities.Aliases
{
    public class CodeAliasFilter : EntityFilter
    {
        public int AliasId { get; set; }
        public int CodeId { get; set; }
        public string CodeDisplay { get; set; }
        public string System { get; set; }
        public string Inbound { get; set; }
        public string Outbound { get; set; }
        public DateTimeOffset ActiveFrom { get; set; }
        public DateTimeOffset ActiveTo { get; set; }
    }
}
