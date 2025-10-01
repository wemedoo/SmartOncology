using System.Collections.Generic;
using System.Runtime.Serialization;

namespace sReportsV2.DTOs.DTOs.ThesaurusEntry.DataIn
{
    public class SkosDataIn
    {
        [DataMember(Name = "thesaurusId")]
        public string ThesaurusId { get; set; }
        [DataMember(Name = "broaderThesaurusIds")]
        public List<int> BroaderThesaurusIds { get; set; } = new List<int>();
        [DataMember(Name = "narrowerThesaurusIds")]
        public List<int> NarrowerThesaurusIds { get; set; } = new List<int>();
        [DataMember(Name = "conceptSchemes")]
        public List<string> ConceptSchemes { get; set; } = new List<string>();
    }
}
