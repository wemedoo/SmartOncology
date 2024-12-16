using sReportsV2.DTOs.CodeEntry.DataOut;
using sReportsV2.DTOs.DTOs.TrialManagement;
using System;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.DTOs.DTOs.Patient.DataOut
{
    public class PatientChemotherapyDataDataOut
    {
        public int Id { get; set; }
        public string IdentificationNumber { get; set; }
        public string Allergies { get; set; }
        public string PatientInformedFor { get; set; }
        public string PatientInformedBy { get; set; }
        public DateTime? PatientInfoSignedOn { get; set; }
        public DateTime? CopyDeliveredOn { get; set; }
        public int CapabilityToWork { get; set; }
        public bool DesireToHaveChildren { get; set; }
        public bool FertilityConservation { get; set; }
        public bool SemenCryopreservation { get; set; }
        public bool EggCellCryopreservation { get; set; }
        public bool SexualHealthAddressed { get; set; }
        public int? ContraceptionCD { get; set; }
        public List<ClinicalTrialDataOut> ClinicalTrials { get; set; } = new List<ClinicalTrialDataOut>();
        public bool PreviousTreatment { get; set; }
        public bool TreatmentInCantonalHospitalGraubunden { get; set; }
        public string HistoryOfOncologicalDisease { get; set; }
        public string HospitalOrPraxisOfPreviousTreatments { get; set; }
        public int? DiseaseContextAtInitialPresentationCD { get; set; }
        public string StageAtInitialPresentation { get; set; }
        public int? DiseaseContextAtCurrentPresentationCD { get; set; }
        public string StageAtCurrentPresentation { get; set; }
        public string Anatomy { get; set; }
        public string Morphology { get; set; }
        public string TherapeuticContext { get; set; }
        public string ChemotherapyType { get; set; }
        public int ChemotherapyCourse { get; set; }
        public int ChemotherapyCycle { get; set; }
        public DateTime? FirstDayOfChemotherapy { get; set; }
        public int? ConsecutiveChemotherapyDays { get; set; }

        public bool IsNewPatientChemotherapyData()
        {
            return Id == 0;
        }

        public List<string> GetRepetitiveValues(string values)
        {
            if (string.IsNullOrWhiteSpace(values))
                return new List<string>();
            else
                return values.Split(';').ToList();
        }

        public string ConvertContraceptionCDToDisplayName(List<CodeDataOut> contraceptions, string language)
        {
            if (this.ContraceptionCD != null && this.ContraceptionCD.HasValue)
                return contraceptions.Find(x => x.Id == this.ContraceptionCD)?.Thesaurus.GetPreferredTermByTranslationOrDefault(language);

            return "";
        }

        public string ConvertDiseaseContextAtInitialPresentationCDToDisplayName(List<CodeDataOut> diseaseContexts, string language)
        {
            if (this.DiseaseContextAtInitialPresentationCD != null && this.DiseaseContextAtInitialPresentationCD.HasValue)
                return diseaseContexts.Find(x => x.Id == this.DiseaseContextAtInitialPresentationCD)?.Thesaurus.GetPreferredTermByTranslationOrDefault(language);

            return "";
        }

        public string ConvertDiseaseContextAtCurrentPresentationCDToDisplayName(List<CodeDataOut> diseaseContexts, string language)
        {
            if (this.DiseaseContextAtCurrentPresentationCD != null && this.DiseaseContextAtCurrentPresentationCD.HasValue)
                return diseaseContexts.Find(x => x.Id == this.DiseaseContextAtCurrentPresentationCD)?.Thesaurus.GetPreferredTermByTranslationOrDefault(language);

            return "";
        }
    }
}
