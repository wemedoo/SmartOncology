using sReportsV2.Common.Entities;

namespace sReportsV2.Domain.Sql.Entities.CodeSetEntry
{
    public class CodeSetFilter : EntityFilter
    {
        public int? CodeSetId { get; set; }
        public string CodeSetDisplay { get; set; }
        public bool ShowActive { get; set; }
        public bool ShowInactive { get; set; }
        public string ActiveLanguage { get; set; }
        public bool OnlyApplicableInDesigner { get; set; }
    }
}
