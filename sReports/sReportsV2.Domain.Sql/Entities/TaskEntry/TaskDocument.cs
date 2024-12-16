using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using sReportsV2.Domain.Sql.Entities.Common;
using System;
using sReportsV2.Common.Extensions;

namespace sReportsV2.Domain.Sql.Entities.TaskEntry
{
    public class TaskDocument : EntitiesBase.Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("TaskDocumentId")]
        public int TaskDocumentId { get; set; }
        public int TaskDocumentCD { get; set; }
        [ForeignKey("TaskDocumentCD")]
        public Code TaskDocumentCode { get; set; }
        public string FormId { get; set; }
        public string FormTitle { get; set; }

        public TaskDocument() { }
        public TaskDocument(int? createdById, string organizationTimeZone = null) : base(createdById, organizationTimeZone) { }
    }
}
