using sReportsV2.Domain.Sql.Entities.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sReportsV2.Domain.Sql.Entities.Patient
{
    public class PatientContactTelecom : TelecomBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("PatientContactTelecomId")]
        public int PatientContactTelecomId { get; set; }
        [ForeignKey("PatientContact")]
        public int PatientContactId { get; set; }
        public PatientContact PatientContact { get; set; }
    }
}
