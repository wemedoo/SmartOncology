using sReportsV2.Domain.Sql.EntitiesBase;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using sReportsV2.Domain.Sql.Entities.User;

namespace sReportsV2.Domain.Sql.Entities.ProjectEntry
{
    public class ProjectPersonnelRelation : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int ProjectPersonnelRelationId { get; set; }

        public int? ProjectId { get; set; }
        [ForeignKey("ProjectId")]
        public Project Project { get; set; }

        public int PersonnelId { get; set; }
        [ForeignKey("PersonnelId")]
        public Personnel Personnel { get; set; }
    }
}
