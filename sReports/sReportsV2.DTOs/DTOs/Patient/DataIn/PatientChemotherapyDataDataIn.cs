using System;

namespace sReportsV2.DTOs.DTOs.Patient.DataIn
{
    public class PatientChemotherapyDataDataIn
    {
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
        public string ClinicalTrials { get; set; }
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
    }
}
