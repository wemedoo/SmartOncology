namespace sReportsV2.DTOs.DTOs.FormInstance.DTO
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class DischargeSummary
    {
        [JsonProperty("sex")]
        public string Sex { get; set; }

        [JsonProperty("age")]
        public string Age { get; set; }

        [JsonProperty("patient_number")]
        public string PatientNumber { get; set; }

        [JsonProperty("admission_date")]
        public string AdmissionDate { get; set; }

        [JsonProperty("admission_time")]
        public string AdmissionTime { get; set; }

        [JsonProperty("discharge_date")]
        public string DischargeDate { get; set; }

        [JsonProperty("admitting_doctor")]
        public string AdmittingDoctor { get; set; }
    }

    public class PrincipalDiagnosisAtAdmission
    {
        [JsonProperty("main_diagnosis")]
        public List<string> MainDiagnosis { get; set; }
    }

    public class DischargeDiagnosis
    {
        [JsonProperty("main_diagnosis")]
        public List<string> MainDiagnosis { get; set; }

        [JsonProperty("other_diagnosis")]
        public List<string> OtherDiagnosis { get; set; }
    }

    public class OtherConsultations
    {
        [JsonProperty("consultations_with_specialists")]
        public List<string> ConsultationsWithSpecialists { get; set; }
    }

    public class ProcedureSurgeries
    {
        [JsonProperty("procedures")]
        public List<string> Procedures { get; set; }

        [JsonProperty("procedure_dates")]
        public List<string> ProcedureDates { get; set; }
    }

    public class MedicalHistory
    {
        [JsonProperty("comorbidities")]
        public List<string> Comorbidities { get; set; }

        [JsonProperty("symptoms_progression")]
        public List<string> SymptomsProgression { get; set; }
    }

    public class ReasonForAdmission
    {
        [JsonProperty("reasons")]
        public List<string> Reasons { get; set; }
    }

    public class VitalSigns
    {
        [JsonProperty("pulse")]
        public string Pulse { get; set; }

        [JsonProperty("systolic_blood_pressure")]
        public int SystolicBloodPressure { get; set; }

        [JsonProperty("diastolic_blood_pressure")]
        public int DiastolicBloodPressure { get; set; }
    }

    public class HospitalCourse
    {
        [JsonProperty("vital_signs")]
        public VitalSigns VitalSigns { get; set; }

        [JsonProperty("laboratory_test_results")]
        public string LaboratoryTestResults { get; set; }

        [JsonProperty("discharge_status")]
        public string DischargeStatus { get; set; }
    }

    public class Medication
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("form")]
        public string Form { get; set; }

        [JsonProperty("dose")]
        public string Dose { get; set; }

        [JsonProperty("frequency")]
        public string Frequency { get; set; }
    }

    public class MedicationDuringHospitalization
    {
        [JsonProperty("medications")]
        public List<Medication> Medications { get; set; }
    }

    public class AdviceFollowingDischarge
    {
        [JsonProperty("recommended_advice")]
        public List<string> RecommendedAdvice { get; set; }

        [JsonProperty("recommended_diet")]
        public string RecommendedDiet { get; set; }

        [JsonProperty("follow_up_information")]
        public string FollowUpInformation { get; set; }
    }

    public class MedicationAtDischarge
    {
        [JsonProperty("medications")]
        public List<Medication> Medications { get; set; }
    }

    public class AIResponseBody
    {
        [JsonProperty("advice_following_discharge")]
        public AdviceFollowingDischarge AdviceFollowingDischarge { get; set; }

        [JsonProperty("discharge_summary")]
        public DischargeSummary DischargeSummary { get; set; }

        [JsonProperty("hospital_course")]
        public HospitalCourse HospitalCourse { get; set; }

        [JsonProperty("reason_for_admission")]
        public ReasonForAdmission ReasonForAdmission { get; set; }

        [JsonProperty("principal_diagnosis_at_admission")]
        public PrincipalDiagnosisAtAdmission PrincipalDiagnosisAtAdmission { get; set; }

        [JsonProperty("discharge_diagnosis")]
        public DischargeDiagnosis DischargeDiagnosis { get; set; }

        [JsonProperty("other_consultations")]
        public OtherConsultations OtherConsultations { get; set; }

        [JsonProperty("procedure_surgeries")]
        public ProcedureSurgeries ProcedureSurgeries { get; set; }

        [JsonProperty("medical_history")]
        public MedicalHistory MedicalHistory { get; set; }

        [JsonProperty("medication_during_hospitalization")]
        public MedicationDuringHospitalization MedicationDuringHospitalization { get; set; }

        [JsonProperty("medication_at_discharge")]
        public MedicationAtDischarge MedicationAtDischarge { get; set; }
    }

}
