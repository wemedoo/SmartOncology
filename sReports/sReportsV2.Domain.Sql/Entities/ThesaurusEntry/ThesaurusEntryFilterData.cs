using sReportsV2.Common.Entities;

namespace sReportsV2.Domain.Sql.Entities.ThesaurusEntry
{
    public class ThesaurusEntryFilterData : EntityFilter
    {
        public int Id { get; set; }
        public int? ThesaurusId { get; set; }
        public string PreferredTerm { get; set; }
        public string Synonym { get; set; }
        public string SimilarTerm { get; set; }
        public string Abbreviation { get; set; }
        public string UmlsCode { get; set; }
        public string UmlsName { get; set; }
        public int? StateCD { get; set; }
        public string ActiveLanguage { get; set; }
        public bool IsSearchTable { get; set; }

    }
}
