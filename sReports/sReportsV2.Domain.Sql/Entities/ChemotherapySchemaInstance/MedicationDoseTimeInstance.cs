using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sReportsV2.Domain.Sql.Entities.ChemotherapySchemaInstance
{
    [Table("MedicationDoseTimeInstances")]
    public class MedicationDoseTimeInstance
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("MedicationDoseTimeInstanceId")]
        public int MedicationDoseTimeInstanceId { get; set; }
        public string Time { get; set; }
        public string Dose { get; set; }
        public bool IsDeleted { get; set; }
        public int MedicationDoseInstanceId { get; set; }

        public void Copy(MedicationDoseTimeInstance medicationDoseTime)
        {
            this.Time = medicationDoseTime.Time;
            this.Dose = medicationDoseTime.Dose;
        }
    }
}
