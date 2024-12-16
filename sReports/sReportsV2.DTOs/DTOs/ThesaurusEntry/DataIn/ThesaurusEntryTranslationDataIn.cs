using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace sReportsV2.DTOs.ThesaurusEntry
{
    [DataContract]
    public class ThesaurusEntryTranslationDataIn
    {
        [DataMember(Name = "Id")]
        public string Id { get; set; }
        
        [DataMember(Name = "language")]
        public string Language { get; set; }

        [DataType(DataType.Html)]
        [DataMember(Name = "definition")]
        public string Definition { get; set; }

        [DataMember(Name = "preferredTerm")]
        public string PreferredTerm { get; set; }

        [DataMember(Name = "parentId")]
        public string ParentId { get; set; }

        [DataMember(Name = "synonyms")]
        public List<string> Synonyms { get; set; }

        [DataMember(Name = "abbreviations")]
        public List<string> Abbreviations { get; set; }

        public int ThesaurusEntryId { get; set; }
    }
}