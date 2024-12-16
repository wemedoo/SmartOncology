using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Domain.Sql.EntitiesBase;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace sReportsV2.Domain.Sql.Entities.Patient
{
    [Table("PatientChemotherapyDatas")]
    public class PatientChemotherapyData : Entity
    {
        public int PatientChemotherapyDataId { get; set; }
        public int PatientId { get; set; }

        public string IdentificationNumber { get; set; }
        // 0 .. * ?
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

        [ForeignKey("ContraceptionCD")]
        public virtual Code ContraceptionCode { get; set; }
        public int? ContraceptionCD { get; set; }
        // 0 .. * ?
        public string ClinicalTrials { get; set; }
        public bool PreviousTreatment { get; set; }
        public bool TreatmentInCantonalHospitalGraubunden { get; set; }
        // 0 .. * ?
        public string HistoryOfOncologicalDisease { get; set; }
        // 0 .. * ?
        public string HospitalOrPraxisOfPreviousTreatments { get; set; }

        [ForeignKey("DiseaseContextAtInitialPresentationCD")]
        public virtual Code DiseaseContextInitialPresentation { get; set; }
        public int? DiseaseContextAtInitialPresentationCD { get; set; }

        [ForeignKey("DiseaseContextAtCurrentPresentationCD")]
        public virtual Code DiseaseContextCurrentPresentation { get; set; }
        public int? DiseaseContextAtCurrentPresentationCD { get; set; }

        public string StageAtInitialPresentation { get; set; }
        public string StageAtCurrentPresentation { get; set; }
        public string Anatomy { get; set; }
        public string Morphology { get; set; }
        public string TherapeuticContext { get; set; }
        public string ChemotherapyType { get; set; }
        public int ChemotherapyCourse { get; set; }
        public int ChemotherapyCycle { get; set; }
        public DateTime? FirstDayOfChemotherapy { get; set; }
        // 0 .. * ?
        public int? ConsecutiveChemotherapyDays { get; set; }

        public void Copy(PatientChemotherapyData patientChemotherapyData)
        {
            if (patientChemotherapyData == null) { return; }

            this.IdentificationNumber = patientChemotherapyData.IdentificationNumber;
            this.StageAtInitialPresentation = patientChemotherapyData.StageAtInitialPresentation;
            this.StageAtCurrentPresentation = patientChemotherapyData.StageAtCurrentPresentation;
            this.Anatomy = patientChemotherapyData.Anatomy;
            this.Morphology = patientChemotherapyData.Morphology;
            this.TherapeuticContext = patientChemotherapyData.TherapeuticContext;
            this.ChemotherapyType = patientChemotherapyData.ChemotherapyType;

            this.CapabilityToWork = patientChemotherapyData.CapabilityToWork;
            this.ChemotherapyCourse = patientChemotherapyData.ChemotherapyCourse;
            this.ChemotherapyCycle = patientChemotherapyData.ChemotherapyCycle;

            this.PatientInfoSignedOn = patientChemotherapyData.PatientInfoSignedOn;
            this.CopyDeliveredOn = patientChemotherapyData.CopyDeliveredOn;
            this.FirstDayOfChemotherapy = patientChemotherapyData.FirstDayOfChemotherapy;

            this.DesireToHaveChildren = patientChemotherapyData.DesireToHaveChildren;
            this.FertilityConservation = patientChemotherapyData.FertilityConservation;
            this.SemenCryopreservation = patientChemotherapyData.SemenCryopreservation;
            this.EggCellCryopreservation = patientChemotherapyData.EggCellCryopreservation;
            this.SexualHealthAddressed = patientChemotherapyData.SexualHealthAddressed;
            this.PreviousTreatment = patientChemotherapyData.PreviousTreatment;
            this.TreatmentInCantonalHospitalGraubunden = patientChemotherapyData.TreatmentInCantonalHospitalGraubunden;

            this.ContraceptionCD = patientChemotherapyData.ContraceptionCD;
            this.DiseaseContextAtInitialPresentationCD = patientChemotherapyData.DiseaseContextAtInitialPresentationCD;
            this.DiseaseContextAtCurrentPresentationCD = patientChemotherapyData.DiseaseContextAtCurrentPresentationCD;

            this.Allergies = patientChemotherapyData.Allergies;
            this.HospitalOrPraxisOfPreviousTreatments = patientChemotherapyData.HospitalOrPraxisOfPreviousTreatments;
            this.ConsecutiveChemotherapyDays = patientChemotherapyData.ConsecutiveChemotherapyDays;

            this.PatientInformedFor = patientChemotherapyData.PatientInformedFor;
            this.HistoryOfOncologicalDisease = patientChemotherapyData.HistoryOfOncologicalDisease;
            this.ClinicalTrials = patientChemotherapyData.ClinicalTrials;
        }

        public List<int> GetClinicalTrialIds()
        {
            return GetRepetitiveValues(ClinicalTrials).Select(strValue => {
                int.TryParse(strValue, out int value);
                return value;
            }).ToList();
        }

        private List<string> GetRepetitiveValues(string values)
        {
            if (string.IsNullOrWhiteSpace(values))
                return new List<string>();
            else
                return values.Split(';').ToList();
        }
    }
}
