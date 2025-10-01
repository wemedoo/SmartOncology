using sReportsV2.Common.Configurations;
using sReportsV2.Common.Constants;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.DTOs.DTOs.FormInstance.DTO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace sReportsV2.BusinessLayer.Helpers
{
    public class AIResponseParser
    {
        public List<FieldSet> FieldSets { get; set; }
        public FormInstance FormInstance { get; set; }
        public AIResponseBody AIResponseBody { get; set; }

        public AIResponseParser(List<FieldSet> fieldSets, FormInstance formInstance, AIResponseBody aIResponseBody)
        {
            FieldSets = fieldSets;
            FormInstance = formInstance;
            AIResponseBody = aIResponseBody;
        }

        public void HandleResponse()
        {
            HandleResponse(AIResponseBody.DischargeSummary);
            HandleResponse(AIResponseBody.PrincipalDiagnosisAtAdmission);
            HandleResponse(AIResponseBody.DischargeDiagnosis);
            HandleResponse(AIResponseBody.OtherConsultations);
            HandleResponse(AIResponseBody.ProcedureSurgeries);
            HandleResponse(AIResponseBody.MedicalHistory);
            HandleResponse(AIResponseBody.ReasonForAdmission);
            HandleResponse(AIResponseBody.HospitalCourse);
            HandleResponse(AIResponseBody.AdviceFollowingDischarge);
            HandleResponse("Medication During Hospitalization", GetListOfEmpty(AIResponseBody.MedicationDuringHospitalization.Medications));
            HandleResponse("Medication At Discharge", GetListOfEmpty(AIResponseBody.MedicationAtDischarge.Medications));
        }

        private void HandleResponse(DischargeSummary dischargeSummary)
        {
            if (dischargeSummary != null)
            {
                FieldSet fieldSet = FieldSets.Find(fS => fS.Label == "Discharge Summary");

                if (fieldSet != null)
                {
                    SetField(fieldSet, "Sex", dischargeSummary.Sex);
                    SetField(fieldSet, "Age", dischargeSummary.Age);
                    SetField(fieldSet, "Patient Number", dischargeSummary.PatientNumber);
                    SetField(fieldSet, "Admission Date", dischargeSummary.AdmissionDate);
                    if (!string.IsNullOrEmpty(dischargeSummary.AdmissionTime) && !string.IsNullOrEmpty(dischargeSummary.AdmissionDate))
                    {
                        SetField(fieldSet, "Admission Time", dischargeSummary.AdmissionTime);
                    }
                    SetField(fieldSet, "Discharge Date", dischargeSummary.DischargeDate);
                    SetField(fieldSet, "Admitting Doctor", dischargeSummary.AdmittingDoctor);
                }
            }
        }

        private void HandleResponse(PrincipalDiagnosisAtAdmission principalDiagnosisAtAdmission)
        {
            if (principalDiagnosisAtAdmission != null)
            {
                SetFieldSetWithOneField("Main (Principal) Diagnosis at Admission", principalDiagnosisAtAdmission.MainDiagnosis);
            }
        }

        private void HandleResponse(DischargeDiagnosis dischargeDiagnosis)
        {
            if (dischargeDiagnosis != null)
            {
                SetFieldSetWithOneField("Main (Principal) Diagnosis at Discharge", dischargeDiagnosis.MainDiagnosis); 
                SetFieldSetWithOneField("Other Diagnosis at Discharge", dischargeDiagnosis.OtherDiagnosis);
            }
        }

        private void HandleResponse(OtherConsultations otherConsultations)
        {
            if (otherConsultations != null)
            {
                SetFieldSetWithOneField("Consultations with other medical specialists", otherConsultations.ConsultationsWithSpecialists);
            }
        }

        private void HandleResponse(ProcedureSurgeries procedureSurgeries)
        {
            if (procedureSurgeries != null)
            {
                FieldSet fieldSet = FieldSets.Find(fS => fS.Label == "Procedure / Surgeries");
                if (fieldSet != null)
                {
                    List<string> procedures = GetListOfEmpty(procedureSurgeries.Procedures);
                    List<string> procedureDates = GetListOfEmpty(procedureSurgeries.ProcedureDates);

                    List<Dictionary<string, string>> fieldInstanceValuesWitninFieldSet = new List<Dictionary<string, string>>();
                    for (int i = 0; i < Math.Max(procedures.Count, procedureDates.Count); i++)
                    {
                        string procedure = procedures.ElementAtOrDefault(i) != null ? procedures[i] : "";
                        string procedureDate = procedureDates.ElementAtOrDefault(i) != null ? procedureDates[i] : "";
                        fieldInstanceValuesWitninFieldSet.Add(new Dictionary<string, string>
                        {
                            {"Medical Procedures Name", procedure },
                            {"Date of Medical Procedure", procedureDate }
                        });
                    }

                    AddMissingFieldSetRepetitionAndSetFields(fieldInstanceValuesWitninFieldSet, fieldSet);
                }
            }
        }

        private void HandleResponse(MedicalHistory medicalHistory)
        {
            if (medicalHistory != null)
            {
                SetFieldSetWithOneField("Medical History", "Medical History - Comorbidities", medicalHistory.Comorbidities);
            }
        }

        private void HandleResponse(ReasonForAdmission reasonForAdmission)
        {
            if (reasonForAdmission != null)
            {
                SetFieldSetWithOneField("Reason For Admission", "Reason Why the Patient Was Admitted to the Hospital", reasonForAdmission.Reasons);
            }
        }

        private void HandleResponse(HospitalCourse hospitalCourse)
        {
            if (hospitalCourse != null)
            {
                FieldSet fieldSet = FieldSets.Find(fS => fS.Label == "Hospital Course");

                if (fieldSet != null)
                {
                    if (hospitalCourse.VitalSigns != null)
                    {
                        SetField(fieldSet, "Vital Signs – Pulse", hospitalCourse.VitalSigns.Pulse);
                        SetField(fieldSet, "Vital Signs – Systolic Blood Pressure", hospitalCourse.VitalSigns.SystolicBloodPressure.ToString());
                        SetField(fieldSet, "Vital Signs – Diastolic Blood Pressure", hospitalCourse.VitalSigns.DiastolicBloodPressure.ToString());
                    }
                    
                    SetField(fieldSet, "Laboratory Test Results", hospitalCourse.LaboratoryTestResults);
                    SetField(fieldSet, "Discharge/Exitus Status", hospitalCourse.DischargeStatus);
                }
            }
        }

        private void HandleResponse(AdviceFollowingDischarge adviceFollowingDischarge)
        {
            if (adviceFollowingDischarge != null)
            {
                SetFieldSetWithOneField("Recommended Advice", adviceFollowingDischarge.RecommendedAdvice);
                FieldSet fieldSet2 = FieldSets.Find(fS => fS.Label == "Recommended Diet");
                if (fieldSet2 != null)
                {
                    SetField(fieldSet2, "Recommended Diet", adviceFollowingDischarge.RecommendedDiet);
                }
                FieldSet fieldSet3 = FieldSets.Find(fS => fS.Label == "Follow-Up Information");
                if (fieldSet3 != null)
                {
                    SetField(fieldSet3, "Follow-Up Information", adviceFollowingDischarge.FollowUpInformation);
                }
            }
        }

        private void HandleResponse(string fieldSetName, List<Medication> medications)
        {
            FieldSet fieldSet = FieldSets.Find(fS => fS.Label == fieldSetName);

            if (fieldSet != null)
            {
                List<Dictionary<string, string>> fieldInstanceValuesWitninFieldSet = medications.Select(medication => new Dictionary<string, string>
                {
                    {"Medication Name", medication.Name },
                    {"Medication Form", medication.Form },
                    {"Medication Dose", medication.Dose },
                    {"Medication Application Frequency", medication.Frequency }

                })
                .ToList();
                AddMissingFieldSetRepetitionAndSetFields(fieldInstanceValuesWitninFieldSet, fieldSet);
            }
        }

        private void AddMissingFieldSetRepetitionAndSetFields(List<Dictionary<string, string>> fieldInstanceValuesWitninFieldSet, FieldSet fieldSet)
        {
            FormInstance.AddMissingFieldSetRepetitions(fieldInstanceValuesWitninFieldSet.Count, fieldSet);
            SetFields(fieldInstanceValuesWitninFieldSet, fieldSet);
        }

        private void SetFieldSetWithOneField(string fieldSetAndFieldLabellName, List<string> repetitiveValues)
        {
            SetFieldSetWithOneField(fieldSetAndFieldLabellName, fieldSetAndFieldLabellName, repetitiveValues);
        }

        private void SetFieldSetWithOneField(string fieldSetName, string fieldLabellName, List<string> repetitiveValues)
        {
            FieldSet fieldSet = FieldSets.Find(fS => fS.Label == fieldSetName);

            if (fieldSet != null)
            {
                List<Dictionary<string, string>> fieldInstanceValuesWitninFieldSet = repetitiveValues.Select(repetitiveValue => new Dictionary<string, string>
                {
                    {fieldLabellName, repetitiveValue }
                })
                .ToList();

                AddMissingFieldSetRepetitionAndSetFields(fieldInstanceValuesWitninFieldSet, fieldSet);
            }
        }

        private void SetFields(List<Dictionary<string, string>> fieldInstanceValuesInFieldSets, FieldSet fieldSet)
        {
            List<string> fieldSetInstanceRepetitionIds = FormInstance.GetFieldSetInstanceRepetitionIds(fieldSet.Id);
            if (fieldSetInstanceRepetitionIds.Count == fieldInstanceValuesInFieldSets.Count)
            {
                for (int i = 0; i < fieldInstanceValuesInFieldSets.Count; i++)
                {
                    string repetitiveFieldSetInstanceRepetitionId = fieldSetInstanceRepetitionIds[i];
                    foreach (KeyValuePair<string, string> fieldInfo in fieldInstanceValuesInFieldSets[i])
                    {
                        SetField(fieldSet, fieldInfo.Key, fieldInfo.Value, repetitiveFieldSetInstanceRepetitionId);
                    }
                }
            }
        }

        private void SetField(FieldSet fieldSet, string fieldLabel, string valueFromJson, string fieldSetInstanceRepetitionId = null)
        {
            Field field = fieldSet.Fields.Find(f => f.Label == fieldLabel);
            if (field != null)
            {
                if (field is FieldRadio fieldRadio)
                {
                    valueFromJson = valueFromJson == "Pre angio profile were normal" ? "Normal" : valueFromJson;
                    valueFromJson = fieldRadio.Values?.Find(f => f.Label == valueFromJson)?.Id;
                }
                FormInstance.GetFieldInstanceByFieldId(field.Id, fieldSetInstanceRepetitionId)?.UpdateValue(valueFromJson);
            }
        }

        private List<T> GetListOfEmpty<T>(List<T> list)
        {
            return list ?? new List<T>();
        }
    }
}
