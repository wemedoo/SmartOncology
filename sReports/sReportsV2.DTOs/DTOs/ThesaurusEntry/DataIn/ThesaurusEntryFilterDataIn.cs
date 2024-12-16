using System.Collections.Generic;

namespace sReportsV2.DTOs.ThesaurusEntry
{
    public class ThesaurusEntryFilterDataIn : sReportsV2.DTOs.Common.DataIn
    {
        public int Id { get; set; }
        public string PreferredTerm { get; set; }
        public string Synonym { get; set; }
        public string SimilarTerm { get; set; }
        public string Abbreviation { get; set; }
        public string UmlsCode { get; set; }
        public string UmlsName { get; set; }
        public int? StateCD { get; set; }
        public int ActiveThesaurus { get; set; }
        public string ActiveLanguage { get; set; }
        public bool IsSearchTable { get; set; }
    }
}
