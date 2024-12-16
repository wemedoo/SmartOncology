using sReportsV2.Domain.Sql.EntitiesBase;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace sReportsV2.Domain.Sql.Entities.ProjectEntry
{
    public class ProjectDocumentRelation : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int ProjectPersonnelRelationId { get; set; }

        public int? ProjectId { get; set; }
        [ForeignKey("ProjectId")]
        public Project Project { get; set; }

        public string FormId { get; set; }
    }
}
