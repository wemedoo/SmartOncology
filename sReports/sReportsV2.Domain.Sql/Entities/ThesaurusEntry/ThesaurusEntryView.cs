using sReportsV2.Domain.Sql.EntitiesBase;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Newtonsoft.Json;

namespace sReportsV2.Domain.Sql.Entities.ThesaurusEntry
{
    public class ThesaurusEntryView : Entity
    {
        [Key]
        [Column("ThesaurusEntryId")]
        public int ThesaurusEntryId { get; set; }
        public string PreferredTerm { get; set; }
        public string Definition { get; set; }
        public string Language { get; set; }
        public int? StateCD { get; set; }
        public string State { get; set; }

        [NotMapped]
        public List<string> Synonyms { get; set; }
        public string SynonymsString
        {
            get
            {
                return this.Synonyms == null || !this.Synonyms.Any() ? null : JsonConvert.SerializeObject(this.Synonyms);
            }

            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    this.Synonyms = new List<string>();
                else
                    this.Synonyms = JsonConvert.DeserializeObject<List<string>>(value);
            }
        }

        [NotMapped]
        public List<string> Abbreviations { get; set; }
        public string AbbreviationsString
        {
            get
            {
                return this.Abbreviations == null || !this.Abbreviations.Any() ? null : JsonConvert.SerializeObject(this.Abbreviations);
            }

            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    this.Abbreviations = new List<string>();
                else
                    this.Abbreviations = JsonConvert.DeserializeObject<List<string>>(value);
            }
        }
    }
}
