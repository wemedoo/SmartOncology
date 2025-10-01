using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace sReportsV2.Domain.Sql.Entities.ThesaurusEntry
{
    public class FormThesaurusReportView
    {
        [Key]
        [Column("ThesaurusEntryId")]
        public int ThesaurusEntryId { get; set; }
        public string PreferredTerm { get; set; }
        public string System { get; set; }
        public string Version { get; set; }
        public string Code { get; set; }
        public string Value { get; set; }
        public DateTime? VersionPublishDate { get; set; }
    }
}
