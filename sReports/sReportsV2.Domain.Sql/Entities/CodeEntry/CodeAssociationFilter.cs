using sReportsV2.Common.Entities;

namespace sReportsV2.Domain.Sql.Entities.CodeEntry
{
    public class CodeAssociationFilter : EntityFilter
    {
        public int CodeSetId { get; set; }
        public int ParentId { get; set; }
        public int? ChildId { get; set; }
        public string ActiveLanguage { get; set; }
        public bool IsChildToParent { get; set; }
        public string SearchTerm { get; set; }
    }
}
