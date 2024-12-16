using sReportsV2.Common.Entities;
using System;

namespace sReportsV2.Domain.Sql.Entities.CodeEntry
{
    public class CodeFilter : EntityFilter
    {
        public int? Id { get; set; }
        public int CodeId { get; set; }
        public string CodeDisplay { get; set; }
        public DateTimeOffset? ValidFrom { get; set; }
        public DateTimeOffset? ValidTo { get; set; }
        public int CodeSetId { get; set; }
        public string CodeSetDisplay { get; set; }
        public string ActiveLanguage { get; set; }
        public bool ShowActive { get; set; }
        public bool ShowInactive { get; set; }
    }
}
