using sReportsV2.Domain.Sql.Entities.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sReportsV2.Domain.Sql.Entities.Patient
{
    [Table("PatientContactAddresses")]
    public class PatientContactAddress : AddressBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("PatientContactAddressId")]
        public int PatientContactAddressId { get; set; }
        [ForeignKey("PatientContact")]
        public int PatientContactId { get; set; }
        public PatientContact PatientContact { get; set; }
    }
}
