using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sReportsV2.Domain.Sql.Entities.Patient
{
    [Table("PatientIdentifiers")]
    public class PatientIdentifier : Base.IdentifierBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int PatientIdentifierId { get; set; }

        public int? PatientId { get; set; }
        [ForeignKey("PatientId")]
        public Patient Patient { get; set; }

        public PatientIdentifier()
        {
        }

        public PatientIdentifier(int? identifierTypeCD, string value, int? identifierUseCD) : base(identifierTypeCD, value, identifierUseCD)
        {
        }

        public PatientIdentifier(int? createdById, string organizationTimeZone = null) : base(createdById, organizationTimeZone) {
        
        }
        
    }
}
