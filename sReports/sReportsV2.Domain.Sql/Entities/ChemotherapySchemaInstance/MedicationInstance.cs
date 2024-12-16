using sReportsV2.Domain.Sql.Entities.ChemotherapySchema;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Domain.Sql.EntitiesBase;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace sReportsV2.Domain.Sql.Entities.ChemotherapySchemaInstance
{
    public class MedicationInstance : Entity, IEditChildEntries<MedicationDoseInstance>
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("MedicationInstanceId")]
        public int MedicationInstanceId { get; set; }
        [ForeignKey("MedicationId")]
        public virtual Medication Medication { get; set; }
        public int MedicationId { get; set; }
        public List<MedicationDoseInstance> MedicationDoses { get; set; } = new List<MedicationDoseInstance>();
        public int ChemotherapySchemaInstanceId { get; set; }

        public void Copy(MedicationInstance medication)
        {
            this.MedicationId = medication.MedicationId;
            
            CopyEntries(medication.MedicationDoses);
        }

        public void CopyEntries(List<MedicationDoseInstance> upcomingEntries)
        {
            if (upcomingEntries != null)
            {
                DeleteExistingRemovedEntries(upcomingEntries);
                AddNewOrUpdateOldEntries(upcomingEntries);
            }
        }

        public void DeleteExistingRemovedEntries(List<MedicationDoseInstance> upcomingEntries)
        {
            foreach (var medicationDose in MedicationDoses)
            {
                var remainingDose = upcomingEntries.Exists(x => x.MedicationDoseInstanceId == medicationDose.MedicationDoseInstanceId);
                if (!remainingDose)
                {
                    medicationDose.IsDeleted = true;
                }
            }
        }

        public void AddNewOrUpdateOldEntries(List<MedicationDoseInstance> upcomingEntries)
        {
            foreach (var dose in upcomingEntries)
            {
                if (dose.MedicationDoseInstanceId == 0)
                {
                    MedicationDoses.Add(dose);
                }
                else
                {
                    var dbDose = MedicationDoses.Find(x => x.MedicationDoseInstanceId == dose.MedicationDoseInstanceId && !x.IsDeleted);
                    if (dbDose != null)
                    {
                        dbDose.Copy(dose);
                    }
                }
            }
        }
    }
}
