using sReportsV2.Domain.Sql.EntitiesBase;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace sReportsV2.Domain.Sql.Entities.ProjectEntry
{
    public class ProjectPatientRelation : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int ProjectPatientRelationId { get; set; }

        public int? ProjectId { get; set; }
        [ForeignKey("ProjectId")]
        public Project Project { get; set; }

        public int PatientId { get; set; }
        [ForeignKey("PatientId")]
        public Patient.Patient Patient { get; set; }
    }
}
