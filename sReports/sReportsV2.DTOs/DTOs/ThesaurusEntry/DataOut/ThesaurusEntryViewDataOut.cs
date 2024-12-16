using System.Collections.Generic;

namespace sReportsV2.DTOs.DTOs.ThesaurusEntry.DataOut
{
    public class ThesaurusEntryViewDataOut
    {
        public int ThesaurusEntryId { get; set; }
        public string PreferredTerm { get; set; }
        public string Definition { get; set; }
        public string Language { get; set; }
        public int? StateCD { get; set; }
        public string State { get; set; }
        public List<string> Synonyms { get; set; }
        public List<string> Abbreviations { get; set; }
    }
}
