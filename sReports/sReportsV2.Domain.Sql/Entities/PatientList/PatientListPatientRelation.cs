using sReportsV2.Domain.Sql.Entities.User;
using sReportsV2.Domain.Sql.EntitiesBase;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace sReportsV2.Domain.Sql.Entities.PatientList
{
    public class PatientListPatientRelation: Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int PatientListPatientRelationId { get; set; }
        public int PatientId { get; set; }
        [ForeignKey("PatientId")]
        public Patient.Patient Patient{ get; set; }
        public int PatientListId { get; set; }
        [ForeignKey("PatientListId")]
        public virtual PatientList PatientList { get; set; }
    }
}