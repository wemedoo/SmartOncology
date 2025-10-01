using sReportsV2.Common.Constants;
using sReportsV2.Common.Extensions;
using sReportsV2.Common.SmartOncologyEnums;
using sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchemaInstance.DataOut;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace sReportsV2.DTOs.DTOs.SmartOncology.ProgressNote.DataOut
{
    public class SchemaTableDataOut
    {
        public int Id { get; set; }
        public string RowVersion { get; set; }
        public DateTime FirstDay { get; set; }
        public List<SchemaTableDayDataOut> Days { get; set; } = new List<SchemaTableDayDataOut>();
        public List<SchemaTableMedicationInstanceDataOut> Medications { get; set; } = new List<SchemaTableMedicationInstanceDataOut>();
        public List<ChemotherapySchemaInstanceVersionDataOut> History { get; set; } = new List<ChemotherapySchemaInstanceVersionDataOut>();
        public List<MedicationReplacementDataOut> MedicationReplacements { get; set; } = new List<MedicationReplacementDataOut>();
        public List<SchemaTableMedicationInstanceDataOut> GetAllMedicationsForTreatment(TreatmentType treatmentType)
        {
            bool isSupportiveTherapy = false;
            if (treatmentType == TreatmentType.SupportiveTherapy)
            {
                isSupportiveTherapy = true;
            }

            return FilterMedications(isSupportiveTherapy);
        }

        public string RenderDaysTableHeader()
        {
            StringBuilder stringBuilder = new StringBuilder();
            string additionalStyleClass = Days.Count <= 2 ? "expand" : "";

            foreach(var day in Days.OrderBy(x => x.DayNumber))
            {
                string date = day.Date.GetDateTimeDisplay(DateTimeConstants.DateFormat, excludeTimePart: true);
                stringBuilder.Append($"<th class=\"{additionalStyleClass}\">{date}</th>");
            }

            return stringBuilder.ToString();
        }

        public void SetSchemaDays(bool initial = false)
        {
            var days = initial ? GetSchemaDaysInitial() : GetSchemaDays();
            Days = days;
        }

        public string RenderTimeDosesValue(MedicationDoseInstanceDataOut dose)
        {
            return dose != null ? dose.RenderTimeDosesValue() : string.Empty;
        }

        public string HasDose(MedicationDoseInstanceDataOut dose)
        {
            bool hasDose = dose != null;

            return hasDose.ToString().ToLower();
        }
        
        public string GetDoseEncoded(MedicationDoseInstanceDataOut dose)
        {
            return dose != null ? dose.ToJsonUrlEncoded() : string.Empty;
        }

        public MedicationDoseInstanceDataOut GetDose(string medicationName, int dayNumber)
        {
            MedicationDoseInstanceDataOut doseInstance = null;
            SchemaTableMedicationInstanceDataOut medicationInstanceData = Medications.Find(m => m.Medication.Name == medicationName && IsMedicationAppliedAtDay(m, dayNumber));
            if (medicationInstanceData != null)
            {
                doseInstance = medicationInstanceData.MedicationDoses.Find(d => d.DayNumber == dayNumber);
            } 
            else
            {
                SchemaTableMedicationDataOut medicationData = Medications.Select(m => m.Medication).FirstOrDefault(m => m.Name == medicationName && IsMedicationAppliedAtDay(m, dayNumber));
                if (medicationData != null)
                {
                    var dose = medicationData.MedicationDoses.Find(d => d.DayNumber == dayNumber);
                    //doseInstance = Mapper.Map<MedicationDoseInstanceDataOut>(dose);
                    doseInstance = new MedicationDoseInstanceDataOut
                    {
                        DayNumber = dose.DayNumber,
                        StartsAt = dose.StartsAt,
                        IntervalId = dose.IntervalId,
                        UnitId = dose.UnitId,
                        Unit = dose.Unit,
                        MedicationDoseTimes = dose.MedicationDoseTimes.Select(doseTime => new MedicationDoseTimeInstanceDataOut 
                        {
                            Time = doseTime.Time,
                            Dose = doseTime.Dose,
                            MedicationDoseId = doseTime.MedicationDoseId
                        }).ToList()
                    };
                }
            }

            return doseInstance;
        }

        public bool IsMedicationReplaced(int medicationInstanceId)
        {
            return MedicationReplacements.Exists(x => x.ReplaceMedicationId == medicationInstanceId);
        }

        private bool IsMedicationAppliedAtDay(SchemaTableMedicationDataOut medication, int dayNumber)
        {
            return medication.MedicationDoses.Exists(d => d.DayNumber == dayNumber);
        }

        private bool IsMedicationAppliedAtDay(SchemaTableMedicationInstanceDataOut medicationInstance, int dayNumber)
        {
            return medicationInstance.MedicationDoses.Exists(d => d.DayNumber == dayNumber);
        }

        private List<SchemaTableMedicationInstanceDataOut> FilterMedications(bool isSupportiveTherapy)
        {
            return Medications.Where(m => m.Medication.IsSupportiveMedication == isSupportiveTherapy).OrderBy(x => x.Id).ToList();
        }

        private List<SchemaTableDayDataOut> GetSchemaDays()
        {
            return Medications
                .SelectMany(m => m.MedicationDoses)
                .GroupBy(d => d.DayNumber)
                .Select(x => SetSchemaDay(x.FirstOrDefault().DayNumber))
                .OrderBy(x => x.DayNumber)
                .ToList();
        }

        private List<SchemaTableDayDataOut> GetSchemaDaysInitial()
        {
            return Medications
                .Select(m => m.Medication)
                .SelectMany(m => m.MedicationDoses)
                .GroupBy(d => d.DayNumber)
                .Select(x => SetSchemaDay(x.FirstOrDefault().DayNumber))
                .OrderBy(x => x.DayNumber)
                .ToList();
        }

        private SchemaTableDayDataOut SetSchemaDay(int dayNumber)
        {
            bool isDelayed = false;
            int addDay = dayNumber;
            if(History.Count > 0)
            {
                int delaysPerDaySumedUp = History.Where(x => dayNumber >= x.FirstDelayDay).Sum(x => x.DelayFor);
                isDelayed = delaysPerDaySumedUp > 0;
                addDay += delaysPerDaySumedUp;
            }

            DateTime date = FirstDay.AppendDays(addDay);

            return new SchemaTableDayDataOut() { DayNumber = dayNumber, Date = date, IsDelayed  = isDelayed };
        }
    }
}
