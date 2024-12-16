using sReportsV2.Common.Entities;

namespace sReportsV2.Domain.Sql.Entities.ThesaurusEntry
{
    public class GlobalThesaurusFilter : EntityFilter
    {
        public string Term { get; set; }
        public string Language { get; set; }
        public string Author { get; set; }
        public string License { get; set; }
        public string TermIndicator { get; set; }
    }
}
