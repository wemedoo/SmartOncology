using sReportsV2.Common.Helpers;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Domain.Sql.Entities.Patient;
using System.Collections.Generic;
using System.Linq;
using sReportsV2.Domain.Entities.Common;
using RestSharp;
using sReportsV2.DTOs.DTOs.PocNlpIntegration.DTO;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Enums;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using sReportsV2.DTOs.DTOs.Oomnia.DTO;
using sReportsV2.DTOs.Organization;
using sReportsV2.DTOs.Patient;
using sReportsV2.Domain.Sql.Entities.Encounter;
using sReportsV2.HL7.Handlers.OutgoingHandlers;
using sReportsV2.HL7;
using sReportsV2.HL7.DTOs;
using sReportsV2.HL7.Constants;
using sReportsV2.DTOs.DTOs.PDF.DataOut;
using sReportsV2.DTOs.DTOs.PDF.DataIn;
using sReportsV2.DTOs.User.DTO;
using sReportsV2.Cache.Singleton;
using sReportsV2.BusinessLayer.Interfaces;
using System;
using RestSharp.Authenticators;
using sReportsV2.BusinessLayer.Helpers;
using sReportsV2.DAL.Sql.Sql;
using sReportsV2.DTOs.Common.DataOut;
using sReportsV2.DTOs.DTOs.Fhir.DataIn;
using System.Threading.Tasks;
using sReportsV2.Common.Extensions;
using Newtonsoft.Json;
using sReportsV2.DTOs.DTOs.FormInstance.DTO;
using Hl7.FhirPath.Sprache;

namespace sReportsV2.BusinessLayer.Implementations
{
    public partial class FormInstanceBLL
    {
        #region HL7

        public void SendHL7Message(FormInstance formInstance, UserCookieData userCookieData)
        {
            bool formHasOutboundAlias = SingletonDataContainer.Instance.GetOutboundAlias(
                    this.GetFormCodeRelationCodeId(formInstance.FormDefinitionId, userCookieData.OrganizationTimeZone),
                    HL7Constants.HL7_MESSAGES
                ) != null;
            bool messageCanBeSent = formInstance.PatientId > 0 && formHasOutboundAlias;
            if (messageCanBeSent)
            {
                PdfDocumentDataOut pdfDocument = pdfBLL.GenerateSynoptic(new PdfDocumentDataIn
                {
                    ResourceId = formInstance.Id,
                    UserCookieData = userCookieData
                });
                Patient patient = patientDAL.GetById(formInstance.PatientId);
                Encounter encounter = encounterDAL.GetById(formInstance.EncounterRef);
                string organizationAlias = organizationDAL.GetById(formInstance.OrganizationId)?.Alias;
                OutgoingMessageMetadataDTO arguments = GetArguments(
                patient,
                    encounter,
                    organizationAlias,
                    formInstance,
                    pdfDocument,
                    HL7Constants.ORU_R01
                );
                HL7OutgoingMessageHandler messageHandler = HL7OutgoingMessageHandlerFactory.GetHandler(arguments);
                messageHandler.ProcessMessage(dbContext);
            }
        }

        private OutgoingMessageMetadataDTO GetArguments(Patient patient, Encounter encounter, string organizationAlias, FormInstance formInstance, PdfDocumentDataOut pdfDocument, string hl7EventType)
        {
            return new OutgoingMessageMetadataDTO
            {
                Patient = patient,
                Encounter = encounter,
                OrganizationAlias = organizationAlias,
                FormInstance = formInstance,
                PdfDocument = pdfDocument,
                HL7EventType = hl7EventType,
                Configuration = configuration
            };
        }

        private int? GetFormCodeRelationCodeId(string formId, string organizationTimeZone)
        {
            return formCodeRelationDAL.GetFormCodeRelationByFormId(formId, organizationTimeZone)?.CodeCD;
        }

        #endregion /HL7

        #region POC NLP API
        public void PassDataToPocNLPApi(FormInstance formInstance)
        {
            string pocNlpAccessCredentials = "Basic c21hcmFnZC11c2VyOnJGbzduLEJQclE0bldMWmM2";
            //Form: Radiology Narrative Report
            if (formInstance.FormDefinitionId == "63da312d587552e575a02df6")
            {
                PocNlpDTO requestBody = GetPocNLPApiBody(formInstance);
                if (requestBody != null)
                {
                    _ = GetResponse(
                        new Common.Entities.RestRequestData
                        {
                            Body = requestBody,
                            BaseUrl = GetPocNLPApiUrl(),
                            Endpoint = "report",
                            ApiName = "PocNlp",
                            HeaderParameters = new Dictionary<string, string> { { "Authorization", pocNlpAccessCredentials } }
                        },
                        dbContext
                    );
                }
                else
                {
                    LogHelper.Error("Could not send data to Poc NLP API Integration server. Request body is empty.");
                }
            }
        }

        private string GetPocNLPApiUrl()
        {
            return configuration["PocNLPApiHostname"];
        }

        private PocNlpDTO GetPocNLPApiBody(FormInstance formInstance)
        {
            PocNlpDTO passDataToNLPApiBody = null;
            string patientIdentifier = GetPatientIdentifierForPocNlpApi(formInstance);
            if (!string.IsNullOrEmpty(patientIdentifier))
            {
                FieldInstance radiologyReportTextField = formInstance.FieldInstances.Find(f => f.FieldId == "4eeee7fea1ba496c9956fdf289a0a9a6");
                passDataToNLPApiBody = new PocNlpDTO
                {
                    patientId = patientIdentifier,
                    radiologyReportText = radiologyReportTextField.FieldInstanceValues.GetFirstValue()
                };
            }

            return passDataToNLPApiBody;
        }

        private string GetPatientIdentifierForPocNlpApi(FormInstance formInstance)
        {
            string patientIdentifier = string.Empty;

            Patient patient = patientDAL.GetById(formInstance.PatientId);
            int? medicalRecordCodeId = SingletonDataContainer.Instance.GetCodeId((int)CodeSetList.PatientIdentifierType, ResourceTypes.MedicalRecordNumber);

            if (patient == null)
            {
                string medicalRecordNumberFieldId = "5ecc00ee5d524d80a0acf6dc042cc732";
                FieldInstance patientIdentifierNumberField = formInstance.FieldInstances.Find(f => f.FieldId == medicalRecordNumberFieldId);
                string medicalRecordIdentifier = patientIdentifierNumberField.FieldInstanceValues.GetFirstValue();
                

                if (!string.IsNullOrEmpty(medicalRecordIdentifier) && medicalRecordCodeId.HasValue)
                {
                    PatientIdentifier medicalRecordQueryIdentifier = new PatientIdentifier(medicalRecordCodeId, medicalRecordIdentifier, null);
                    patient = patientDAL.GetByIdentifier(medicalRecordQueryIdentifier);
                }
            }

            if (patient != null)
            {
                patientIdentifier = patient.PatientIdentifiers.Find(i => i.IdentifierTypeCD == medicalRecordCodeId)?.IdentifierValue;
            }

            return patientIdentifier;
        }

        public bool SendIntegrationEngineRequest(string requestEndpoint, string port, Object requestBody) 
        {
            if (requestBody != null)
            {
                var response = GetResponse(
                    new Common.Entities.RestRequestData
                    {
                        Body = requestBody,
                        BaseUrl = configuration["IntegrationEngineUrl"] + ":" + port,
                        Endpoint = requestEndpoint,
                    },
                    dbContext,
                    new HttpBasicAuthenticator(configuration["IntegrationEngineUsername"], configuration["IntegrationEnginePassword"])
                );

                if (response.IsSuccessful)
                {
                    LogHelper.Info("Api Request successful.");
                }
                else
                {
                    LogHelper.Error("Api Request failed.");
                }
                return response.IsSuccessful;
            }
            else
            {
                LogHelper.Error("Could not send data to API Integration server. Request body is empty.");
                return false;   
            }
        }

        #endregion /POC NLP API

        #region Form Instance Triggers

        protected void ExecuteAdditionalFormInstanceTriggersAfterSave(FormInstance formInstance, UserCookieData userCookieData)
        {
            asyncRunner.Run<IFormInstanceBLL>((standaloneFormInstanceBLL) =>
                standaloneFormInstanceBLL.SendHL7Message(
                    formInstance,
                    userCookieData
                    )
            );
            asyncRunner.Run<IFormInstanceBLL>((standaloneFormInstanceBLL) =>
                standaloneFormInstanceBLL.PassDataToPocNLPApi(formInstance)
            );
        }

        private void ExecuteAdditionalFormInstanceTriggersAfterLock(LockActionToOomniaApiDTO lockAction, UserCookieData userCookieData)
        {
            if (lockAction.IsLocked)
            {
                if (!string.IsNullOrEmpty(GetOomniaApiUrl()))
                {
                    asyncRunner.Run<IFormInstanceBLL>((standaloneFormInstanceBLL) =>
                    standaloneFormInstanceBLL.PassDataToOomniaApi(lockAction, userCookieData)
                    );
                }
                else
                {
                    LogHelper.Warning("Invoking external data transfer, but no url is defined");
                }
            }
        }

        #endregion /Form Instance Triggers

        #region OOMNIA API
        public void PassDataToOomniaApi(LockActionToOomniaApiDTO lockAction, UserCookieData userCookieData)
        {
            try
            {
                PassFormInstanceToOomniaApiDTO body = GetOomniaBodyApi(lockAction);
                if (body != null)
                {
                    bool noPendingRequests = !FormInstanceExternalRequestsCache.Instance.HasPendingRequests(lockAction.FormInstanceId);
                    FormInstanceExternalRequestsCache.Instance.AddPendingRequest(lockAction.FormInstanceId, body);
                    if(noPendingRequests)
                    {
                        ProcessOomniaRequest(body, lockAction, userCookieData);
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogHelper.Error(@"Pass Data to Oomnia error: " + ex.GetExceptionStackMessages());
                LogHelper.Error(@"Pass Data to Oomnia error, stack trace: " + ex.StackTrace);
            }
        }

        private void ProcessOomniaRequest(PassFormInstanceToOomniaApiDTO body, LockActionToOomniaApiDTO lockAction, UserCookieData userCookieData)
        {
            if (body.RequestData?.ExternalDocumentInstanceId == null)
            {
                SetPatientIdentifiers(lockAction, body);
            }
            RestResponse restResponse = GetResponse(
                            new Common.Entities.RestRequestData
                            {
                                Body = body,
                                BaseUrl = GetOomniaApiUrl(),
                                Endpoint = "save-documents-from-esource",
                                ApiName = "OOMNIA",
                                HeaderParameters = new Dictionary<string, string> { { "Authorization", $"Bearer {GetOomniaApiToken()}" } }
                            },
                            dbContext
                        );
            HandleOomniaResponse(restResponse, lockAction, userCookieData);
        }

        private List<string> GetOomniaApiProperties()
        {
            return new List<string> { "OomniaDocumentInstanceExternalId", "PatientId" };
        }

        private string GetOomniaApiUrl()
        {
            return configuration["OomniaApiHostname"];
        }

        private string GetOomniaApiToken()
        {
            return configuration["OomniaApiToken"];
        }

        private PassFormInstanceToOomniaApiDTO GetOomniaBodyApi(LockActionToOomniaApiDTO lockAction)
        {
            FormInstance formInstance = formInstanceDAL.GetById(lockAction.FormInstanceId);
            Form formDefinition = formDAL.GetForm(formInstance.FormDefinitionId);
            OrganizationDataOut organization = mapper.Map<OrganizationDataOut>(organizationDAL.GetById(formInstance.OrganizationId));

            string externalOrganizationId = GetOomniaExternalId(
                organization.Identifiers, 
                (int)CodeSetList.OrganizationIdentifierType, 
                ResourceTypes.OomniaExternalId);

            if (string.IsNullOrEmpty(formDefinition.OomniaId) || string.IsNullOrEmpty(externalOrganizationId))
            {
                return null;
            }
            
            PassFormInstanceToOomniaApiDTO passFormInstanceToOomniaApiDTO = new PassFormInstanceToOomniaApiDTO(externalOrganizationId, formDefinition.OomniaId);

            SetFields(formInstance, formDefinition, passFormInstanceToOomniaApiDTO, lockAction);

            return passFormInstanceToOomniaApiDTO;
        }

        private string GetOomniaExternalId(List<IdentifierDataOut> identifiers, int codesetId, string codeName)
        {
            int? oomniaExternalCodeId = SingletonDataContainer.Instance.GetCodeId(codesetId, codeName);
            return identifiers?.Find(i => i.IdentifierTypeId == oomniaExternalCodeId)?.Value ?? string.Empty;
        }

        private void SetPatientIdentifiers(LockActionToOomniaApiDTO lockAction, PassFormInstanceToOomniaApiDTO passFormInstanceToOomniaApiDTO)
        {
            FormInstance formInstance = formInstanceDAL
                    .GetById(lockAction.FormInstanceId, GetOomniaApiProperties());
            PatientDataOut patient = mapper.Map<PatientDataOut>(patientDAL.GetById(formInstance.PatientId));
            List<IdentifierDataOut> patientIdentifiers = patient?.Identifiers;

            string participantId = GetOomniaExternalId(
                patientIdentifiers,
                (int)CodeSetList.PatientIdentifierType,
                ResourceTypes.OomniaExternalId);

            passFormInstanceToOomniaApiDTO.ParticipantIdentifier = participantId;
            passFormInstanceToOomniaApiDTO.RequestData.ExternalDocumentInstanceId = formInstance.OomniaDocumentInstanceExternalId;
        }

        private void SetFields(FormInstance formInstance, Form formDefinition, PassFormInstanceToOomniaApiDTO passFormInstanceToOomniaApiDTO, LockActionToOomniaApiDTO lockAction)
        {
            IEnumerable<FieldSet> fieldSets = GetFieldSets(lockAction, formDefinition, formInstance);
            IDictionary<string, bool> repetitiveFieldSetStatuses = fieldSets.ToDictionary(x => x.Id, x => x.IsRepetitive);
            IDictionary<string, Field> fieldDefinitions =
                fieldSets
                .SelectMany(fs => fs.Fields)
                .ToDictionary(f => f.Id, f => f);
            IDictionary<int, ThesaurusEntry> thesaurusesFromFormDefinition =
                thesaurusDAL
                .GetByIdsList(
                    fieldSets.SelectMany(fs => fs.GetAllThesaurusIds()).ToList()
                    )
                .ToDictionary(x => x.ThesaurusEntryId, x => x);

            int? oomniaCodeSystemId = GetOomniaExternalThesaurusCodeSystemId();

            IDictionary<string, int> fieldSetOrderNumbers = new Dictionary<string, int>();
            foreach (var fieldsInFieldset in formInstance.FieldInstances
                .Where(x => repetitiveFieldSetStatuses.Keys.Contains(x.FieldSetId))
                .GroupBy(f => new { f.FieldSetId, f.FieldSetInstanceRepetitionId })
                )
            {
                int? fieldsetSequenceNumber = GetFieldSetSequnceNumber(fieldSetOrderNumbers, fieldsInFieldset.Key.FieldSetId, repetitiveFieldSetStatuses);
                foreach (FieldInstance fieldInstance in fieldsInFieldset)
                {
                    if (thesaurusesFromFormDefinition.TryGetValue(fieldInstance.ThesaurusId, out ThesaurusEntry thesaurus))
                    {
                        O4CodeableConcept codeEntity = thesaurus.GetCodeByCodeSystem(oomniaCodeSystemId);
                        if (codeEntity != null)
                        {
                            for (int i = 0; i < fieldInstance.FieldInstanceValues.Count; i++)
                            {
                                FieldInstanceValue fieldInstanceValue = fieldInstance.FieldInstanceValues[i];
                                if (fieldInstanceValue.HasAnyValue() && fieldDefinitions.TryGetValue(fieldInstance.FieldId, out Field fieldDefinition))
                                {
                                    AddField(
                                            passFormInstanceToOomniaApiDTO,
                                            codeEntity.Code,
                                            fieldDefinition,
                                            fieldInstanceValue,
                                            thesaurusesFromFormDefinition,
                                            oomniaCodeSystemId,
                                            i + 1,
                                            fieldsetSequenceNumber);
                                }
                            }
                        }
                    }
                }
            }
        }

        private int? GetOomniaExternalThesaurusCodeSystemId()
        {
            return SingletonDataContainer.Instance.GetCodeSystems().Find(c => c.Label == ResourceTypes.OomniaExternalId)?.Id;
        }

        private IEnumerable<FieldSet> GetFieldSets(LockActionToOomniaApiDTO lockAction, Form form, FormInstance formInstance)
        {
            if (!string.IsNullOrEmpty(lockAction.FieldSetInstanceRepetitionId))
            {
                string fieldSetId = formInstance.FieldInstances
                    .Find(fI => fI.FieldSetInstanceRepetitionId == lockAction.FieldSetInstanceRepetitionId).FieldSetId;
                return form.GetListOfFieldSetsByFieldSetId(fieldSetId);
            }
            else if (!string.IsNullOrEmpty(lockAction.PageId))
            {
                return form.GetFieldSetsInPage(lockAction.ChapterId, lockAction.PageId);
            }
            else if (!string.IsNullOrEmpty(lockAction.ChapterId))
            {
                return form.GetFieldSetsInChapter(lockAction.ChapterId);
            }
            else
            {
                return form.GetAllFieldSets();
            }
        }

        private int? GetFieldSetSequnceNumber(IDictionary<string, int> fieldSetOrderNumbers, string fieldSetId, IDictionary<string, bool> repetitiveFieldSetStatuses)
        {
            repetitiveFieldSetStatuses.TryGetValue(fieldSetId, out bool isRepetitive);
            if (isRepetitive)
            {
                fieldSetOrderNumbers.TryGetValue(fieldSetId, out int previousFieldSetSequnceNumber);
                int fieldSetSequnceNumber = previousFieldSetSequnceNumber + 1;
                fieldSetOrderNumbers[fieldSetId] = fieldSetSequnceNumber;
                return fieldSetSequnceNumber;
            }
            else
            {
                return null;
            }
        }

        private void AddField(PassFormInstanceToOomniaApiDTO passFormInstanceToOomniaApiDTO, string oomniaVariableCodeName, Field fieldDefinition, FieldInstanceValue fieldInstanceValue, IDictionary<int, ThesaurusEntry> thesaurusesFromFormDefinition, int? omniaCodeSystemId, int? fieldSequenceNumber, int? fieldsetSequenceNumber)
        {
            passFormInstanceToOomniaApiDTO.RequestData.Fields.Add(
                new SaveFieldData
                {
                    Name = oomniaVariableCodeName,
                    Value = new FieldValue
                    {
                        Text = fieldDefinition.GetTextValueForOomniaApi(fieldInstanceValue),
                        SelectedOptions = fieldDefinition.GetSelectedValuesForOomniaApi(fieldInstanceValue.GetAllValues(), thesaurusesFromFormDefinition, omniaCodeSystemId)
                    },
                    FieldSequenceNumber = fieldDefinition.IsFieldRepetitive() ? fieldSequenceNumber : null,
                    GroupSequenceNumber = fieldsetSequenceNumber
                }
            );
        }

        private void HandleOomniaResponse(RestResponse restResponse, LockActionToOomniaApiDTO lockAction, UserCookieData userCookieData)
        {
            if (restResponse.StatusCode == System.Net.HttpStatusCode.Created)
            {
                SaveSReportsDocumentResponse saveDocumentsResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<SaveSReportsDocumentResponse>(restResponse.Content);
                SaveSReportsDocumentResponseItem savedSReportsDocument = saveDocumentsResponse.SavedDocuments.FirstOrDefault();

                FormInstance formInstance = UpdateFormInstanceIfNecessary(lockAction, savedSReportsDocument);
                UpdatePatientDataIfNecessary(formInstance.PatientId, savedSReportsDocument, userCookieData);
            }
            HandleCacheAfterOomniaResponse(lockAction, userCookieData);
        }

        private FormInstance UpdateFormInstanceIfNecessary(LockActionToOomniaApiDTO lockAction, SaveSReportsDocumentResponseItem savedSReportsDocument)
        {
            FormInstance formInstance = formInstanceDAL.GetById(lockAction.FormInstanceId, GetOomniaApiProperties());
            if (formInstance.OomniaDocumentInstanceExternalId.HasValue)
            {
                if (formInstance.OomniaDocumentInstanceExternalId.Value != savedSReportsDocument.ExternalDocumentInstanceId)
                {
                    throw new InvalidOperationException($"New external document instance is coming, old: {formInstance.OomniaDocumentInstanceExternalId.Value}, new: {savedSReportsDocument.ExternalDocumentInstanceId}");
                }
            }
            else
            {
                formInstance.OomniaDocumentInstanceExternalId = savedSReportsDocument.ExternalDocumentInstanceId;
                formInstanceDAL.UpdateOomniaExternalDocumentInstanceId(formInstance);
            }
            return formInstance;
        }

        private void UpdatePatientDataIfNecessary(int patientId, SaveSReportsDocumentResponseItem savedSReportsDocument, UserCookieData userCookieData)
        {
            Patient patient = patientDAL.GetById(patientId);
            if (patient != null)
            {
                bool newIdentifiersAdded = SetOomniaIdentifierTypeIds(
                    patient,
                    savedSReportsDocument,
                    userCookieData
                    );
                if (newIdentifiersAdded)
                {
                    patientDAL.InsertOrUpdate(patient, null);
                }
            }
        }

        private void HandleCacheAfterOomniaResponse(LockActionToOomniaApiDTO lockAction, UserCookieData userCookieData)
        {
            FormInstanceExternalRequestsCache.Instance.RemovePendingRequest(lockAction.FormInstanceId);
            if (FormInstanceExternalRequestsCache.Instance.HasPendingRequests(lockAction.FormInstanceId))
            {
                PassFormInstanceToOomniaApiDTO pendingRequest = FormInstanceExternalRequestsCache.Instance.GetPendingRequest(lockAction.FormInstanceId);
                if (pendingRequest != null)
                {
                    LogHelper.Info($"Pending request for form instance (id {lockAction.FormInstanceId}) has been pulled from external requests cache");
                    ProcessOomniaRequest(pendingRequest, lockAction, userCookieData);
                }
            }
        }

        private bool SetOomniaIdentifierTypeIds(Patient patient, SaveSReportsDocumentResponseItem saveSReportsDocument, UserCookieData userCookieData)
        {
            bool newIdentifiersAdded = false;
            IList<string> oomniaIdentifierNames = new List<string>
            {
                ResourceTypes.OomniaExternalId,
                ResourceTypes.OomniaScreeningNumber
            };

            IDictionary<string, string> oomniaIdentifierValues = new Dictionary<string, string>
            {
                { ResourceTypes.OomniaExternalId, saveSReportsDocument.ParticipantId},
                { ResourceTypes.OomniaScreeningNumber, saveSReportsDocument.ScreeningNumber }
            };

            foreach (string oomniaIdentifierName in oomniaIdentifierNames)
            {
                int? oomniaExternalCodeId = SingletonDataContainer.Instance.GetCodeId((int)CodeSetList.PatientIdentifierType, oomniaIdentifierName);
                string oomniaExternalidValue = oomniaIdentifierValues[oomniaIdentifierName];
                if (oomniaExternalCodeId.HasValue && !patient.PatientIdentifiers.Exists(x => x.IdentifierTypeCD == oomniaExternalCodeId) && !string.IsNullOrEmpty(oomniaExternalidValue))
                {
                    newIdentifiersAdded = true;
                    patient.PatientIdentifiers.Add(new PatientIdentifier(userCookieData.Id, userCookieData.OrganizationTimeZone)
                    {
                        IdentifierTypeCD = oomniaExternalCodeId,
                        IdentifierValue = oomniaExternalidValue
                    });
                }
            }

            return newIdentifiersAdded;
        }

        #endregion /OOMNIA API

        #region AI Extraction
        public async Task<bool> GenerateAIDataExtraction(DataExtractionDataIn dataExtractionDataIn, UserCookieData userCookieData)
        {
            dataExtractionDataIn = Ensure.IsNotNull(dataExtractionDataIn, nameof(dataExtractionDataIn));
            Ensure.IsNotNullOrWhiteSpace(dataExtractionDataIn.FormInstanceId, nameof(dataExtractionDataIn.FormInstanceId));
            Ensure.IsNotNullOrWhiteSpace(dataExtractionDataIn.FieldInstanceIdWithDataToExtract, nameof(dataExtractionDataIn.FieldInstanceIdWithDataToExtract));

            FormInstance formInstance = await this.GetByIdAsync(dataExtractionDataIn.FormInstanceId);
            string fileName = formInstance
                .FieldInstances
                .SelectMany(fI => fI.FieldInstanceValues)
                .FirstOrDefault(fIV => fIV.FieldInstanceRepetitionId == dataExtractionDataIn.FieldInstanceIdWithDataToExtract)
                ?.Values
                ?.FirstOrDefault();

            if (fileName != null)
            {
                SetFieldInstances(GetResponseContent(fileName), formInstance);
                this.InsertOrUpdateAsync(formInstance, formInstance.GetCurrentFormInstanceStatus(userCookieData?.Id), userCookieData);
            }

            return true;
        }

        private string GetResponseContent(string fileName)
        {
            string responseContent = null;
            if (fileName.EndsWith("PdfFromPhoto1"))
            {
                responseContent = @"{
                    ""advice_following_discharge"": {
                        ""follow_up_information"": ""Follow up with Dr. Rasesh Pothiwala after 30 days with prior appointment"",
                        ""recommended_advice"": [
                            ""Regular medication & follow up""
                        ],
                        ""recommended_diet"": ""Low fat diet""
                    },
                    ""discharge_diagnosis"": {
                        ""main_diagnosis"": [
                        ],
                        ""other_diagnosis"": [
                        ]
                    },
                    ""discharge_summary"": {
                        ""admission_date"": null,
                        ""admission_time"": null,
                        ""admitting_doctor"": ""Dr. Rasesh Pothiwala"",
                        ""age"": null,
                        ""discharge_date"": null,
                        ""patient_number"": null,
                        ""sex"": null
                    },
                    ""hospital_course"": {
                        ""discharge_status"": ""Stable haemodynamic condition"",
                        ""laboratory_test_results"": null,
                        ""vital_signs"": {
                            ""diastolic_blood_pressure"": null,
                            ""pulse"": null,
                            ""systolic_blood_pressure"": null
                        }
                    },
                    ""medical_history"": {
                        ""comorbidities"": [
                        ],
                        ""symptoms_progression"": [
                        ]
                    },
                    ""medication_at_discharge"": {
                        ""medications"": [
                            {
                                ""dose"": ""75"",
                                ""form"": ""Tablet"",
                                ""frequency"": ""1-0-1"",
                                ""name"": ""Clavix (Clopidogrel)""
                            },
                            {
                                ""dose"": ""75"",
                                ""form"": ""Tablet"",
                                ""frequency"": ""0-1-0"",
                                ""name"": ""Ecosprin (Aspirin)""
                            },
                            {
                                ""dose"": ""80"",
                                ""form"": ""Tablet"",
                                ""frequency"": ""0-0-1"",
                                ""name"": ""Lipicure (Atorvastatin)""
                            },
                            {
                                ""dose"": ""40 mg"",
                                ""form"": ""Tablet"",
                                ""frequency"": ""1-0-1"",
                                ""name"": ""Pantodac (Pantoprazole)""
                            },
                            {
                                ""dose"": null,
                                ""form"": ""Tablet"",
                                ""frequency"": ""0-1-0"",
                                ""name"": ""FDSON MP (Folic acid + methylcobalamine + Pyridoxin)""
                            },
                            {
                                ""dose"": ""5 mg"",
                                ""form"": ""Tablet"",
                                ""frequency"": ""Sublingual if chest pain (SOS)"",
                                ""name"": ""Sorbitrate (Isosorbide Dinitrate)""
                            }
                        ]
                    },
                    ""medication_during_hospitalization"": {
                        ""medications"": [
                            {
                                ""form"": ""Injection"",
                                ""name"": ""Efforlin""
                            },
                            {
                                ""form"": ""Injection"",
                                ""name"": ""Taxim""
                            },
                            {
                                ""form"": ""Injection"",
                                ""name"": ""Heparin""
                            },
                            {
                                ""form"": ""Injection"",
                                ""name"": ""NS""
                            },
                            {
                                ""form"": ""Injection"",
                                ""name"": ""Tirofiban""
                            },
                            {
                                ""form"": ""Injection"",
                                ""name"": ""Cort S""
                            },
                            {
                                ""form"": ""Injection"",
                                ""name"": ""Clexane""
                            },
                            {
                                ""form"": ""Tablet"",
                                ""name"": ""Droxyl""
                            },
                            {
                                ""form"": ""Tablet"",
                                ""name"": ""Clavix""
                            },
                            {
                                ""form"": ""Tablet"",
                                ""name"": ""Ecosprin""
                            },
                            {
                                ""form"": ""Tablet"",
                                ""name"": ""Lipicure""
                            },
                            {
                                ""form"": ""Tablet"",
                                ""name"": ""Pantodac""
                            },
                            {
                                ""form"": ""Tablet"",
                                ""name"": ""FDSON MP""
                            }
                        ]
                    },
                    ""other_consultations"": {
                        ""consultations_with_specialists"": [
                        ]
                    },
                    ""principal_diagnosis_at_admission"": {
                        ""main_diagnosis"": [
                        ]
                    },
                    ""procedure_surgeries"": {
                        ""procedure_dates"": [
                        ],
                        ""procedures"": [
                        ]
                    },
                    ""reason_for_admission"": {
                        ""reasons"": [
                        ]
                    }
                }";
            } 
            else if (fileName.EndsWith("PdfFromPhoto2"))
            {
                responseContent = @"{
                    ""advice_following_discharge"": {
                        ""follow_up_information"": null,
                        ""recommended_advice"": [
                        ],
                        ""recommended_diet"": null
                    },
                    ""discharge_diagnosis"": {
                        ""main_diagnosis"": [
                            ""Acute Coronary Syndrome"",
                            ""Coronary Artery Disease - Single Vessel Disease"",
                            ""Successful Denovo stenting of LCX lesion done using 1 DES""
                        ],
                        ""other_diagnosis"": [
                            ""None""
                        ]
                    },
                    ""discharge_summary"": {
                        ""admission_date"": ""18/05/2019"",
                        ""admission_time"": ""12:01 AM"",
                        ""admitting_doctor"": ""Dr. DIRECT . HOSPITAL"",
                        ""age"": ""38 Years 11 Months 7 Days"",
                        ""discharge_date"": ""20/05/2019"",
                        ""patient_number"": ""5461233"",
                        ""sex"": ""Male""
                    },
                    ""hospital_course"": {
                        ""discharge_status"": ""patient is being discharged in stable haemodynamic condition"",
                        ""laboratory_test_results"": ""Pre angio profile were normal"",
                        ""vital_signs"": {
                            ""diastolic_blood_pressure"": ""70"",
                            ""pulse"": ""70/min"",
                            ""systolic_blood_pressure"": ""110""
                        }
                    },
                    ""medical_history"": {
                        ""comorbidities"": [
                            ""nondiabetic"",
                            ""normotensive""
                        ],
                        ""symptoms_progression"": [
                            ""complaints of chest pain 13 hours prior to admission which eventually worsened""
                        ]
                    },
                    ""medication_at_discharge"": {
                        ""medications"": [
                        ]
                    },
                    ""medication_during_hospitalization"": {
                        ""medications"": [
                            {
                                ""form"": ""Inj"",
                                ""name"": ""Avil""
                            }
                        ]
                    },
                    ""other_consultations"": {
                        ""consultations_with_specialists"": [
                            ""None""
                        ]
                    },
                    ""principal_diagnosis_at_admission"": {
                        ""main_diagnosis"": [
                            ""Acute Coronary Syndrome""
                        ]
                    },
                    ""procedure_surgeries"": {
                        ""procedure_dates"": [
                            ""18/05/19"",
                            ""18/05/19""
                        ],
                        ""procedures"": [
                            ""Coronary Angiography"",
                            ""Denovo stenting of LCX lesion with 1 DES""
                        ]
                    },
                    ""reason_for_admission"": {
                        ""reasons"": [
                            ""Patient was admitted in Sterling Hospital for further cardiac management.""
                        ]
                    }
                }";
            }
            else
            {
                responseContent = @"{
                    ""discharge_summary"": {
                        ""sex"": ""Male"",
                        ""age"": ""38 Years 11 Months 7 Days"",
                        ""patient_number"": ""5461233"",
                        ""admission_date"": ""18/05/2019"",
                        ""admission_time"": ""12:01 AM"",
                        ""discharge_date"": ""20/05/2019"",
                        ""admitting_doctor"": ""Dr. DIRECT, HOSPITAL""
                    },
                    ""principal_diagnosis_at_admission"": {
                        ""main_diagnosis"": [
                            ""Acute Coronary Syndrome""
                        ]
                    },
                    ""discharge_diagnosis"": {
                        ""main_diagnosis"": [
                            ""Acute Coronary Syndrome"",
                            ""Coronary Artery Disease - Single Vessel Disease"",
                            ""Successful Denovo stenting of LCX lesion done using 1 DES""
                        ],
                        ""other_diagnosis"": [
                            ""None""
                        ]
                    },
                    ""other_consultations"": {
                        ""consultations_with_specialists"": [
                            ""None""
                        ]
                    },
                    ""procedure_surgeries"": {
                        ""procedures"": [
                            ""Coronary Angiography"",
                            ""Denovo stenting of LCX lesion with 1 DES""
                        ],
                        ""procedure_dates"": [
                            ""18/05/19"",
                            ""18/05/19""
                        ]
                    },
                    ""medical_history"": {
                        ""comorbidities"": [
                            ""nondiabetic"",
                            ""normotensive""
                        ],
                        ""symptoms_progression"": [
                            ""complaints of chest pain 13 hours prior to admission which eventually worsened""
                        ]
                    },
                    ""reason_for_admission"": {
                        ""reasons"": [
                            ""Patient was admitted in Sterling Hospital for further cardiac management.""
                        ]
                    },
                    ""hospital_course"": {
                        ""vital_signs"": {
                            ""pulse"": ""70/min"",
                            ""systolic_blood_pressure"": 110,
                            ""diastolic_blood_pressure"": 70
                        },
                        ""laboratory_test_results"": ""Pre angio profile were normal"",
                        ""discharge_status"": ""stable haemodynamic condition""
                    },
                    ""medication_during_hospitalization"": {
                        ""medications"": [
                            {
                                ""name"": ""Avil"",
                                ""form"": ""Inj""
                            },
                            {
                                ""name"": ""Effcorlin"",
                                ""form"": ""Inj""
                            },
                            {
                                ""name"": ""Taxim"",
                                ""form"": ""Inj""
                            },
                            {
                                ""name"": ""Heparin"",
                                ""form"": ""Inj""
                            },
                            {
                                ""name"": ""NS"",
                                ""form"": ""Inj""
                            },
                            {
                                ""name"": ""Tirofiban"",
                                ""form"": ""Inj""
                            },
                            {
                                ""name"": ""Cort S"",
                                ""form"": ""Inj""
                            },
                            {
                                ""name"": ""Clexane"",
                                ""form"": ""Inj""
                            },
                            {
                                ""name"": ""Droxyl"",
                                ""form"": ""Tab""
                            },
                            {
                                ""name"": ""Clavix"",
                                ""form"": ""Tab""
                            },
                            {
                                ""name"": ""Ecosprin"",
                                ""form"": ""Tab""
                            },
                            {
                                ""name"": ""Lipicure"",
                                ""form"": ""Tab""
                            },
                            {
                                ""name"": ""Pantodac"",
                                ""form"": ""Tab""
                            },
                            {
                                ""name"": ""FDSON MP"",
                                ""form"": ""Tab""
                            }
                        ]
                    },
                    ""advice_following_discharge"": {
                        ""recommended_advice"": [
                            ""Regular medication & follow up""
                        ],
                        ""recommended_diet"": ""Low fat diet"",
                        ""follow_up_information"": ""Follow up with Dr. Rasesh Pothiwala after 30 days with prior appointment""
                    },
                    ""medication_at_discharge"": {
                        ""medications"": [
                            {
                                ""name"": ""Clavix (Clopidogrel)"",
                                ""form"": ""Tab"",
                                ""dose"": ""75"",
                                ""frequency"": ""1-0-1""
                            },
                            {
                                ""name"": ""Ecosprin (Aspirin)"",
                                ""form"": ""Tab"",
                                ""dose"": ""75"",
                                ""frequency"": ""0-1-0""
                            },
                            {
                                ""name"": ""Lipicure (Atorvastatin)"",
                                ""form"": ""Tab"",
                                ""dose"": ""80"",
                                ""frequency"": ""0-0-1""
                            },
                            {
                                ""name"": ""Pantodac (Pantoprazole)"",
                                ""form"": ""Tab"",
                                ""dose"": ""40 mg"",
                                ""frequency"": ""1-0-1""
                            },
                            {
                                ""name"": ""FDSON MP (Folic acid + methylcobalamine + Pyridoxin)"",
                                ""form"": ""Tab"",
                                ""dose"": null,
                                ""frequency"": ""0-1-0""
                            },
                            {
                                ""name"": ""Sorbitrate (Isosorbide Dinitrate)"",
                                ""form"": ""Tab"",
                                ""dose"": ""5 mg"",
                                ""frequency"": ""Sublingual if chest pain (SOS)""
                            }
                        ]
                    }
                }";
            }
            
            return responseContent;
        }

        private void SetFieldInstances(string responseBody, FormInstance formInstance)
        {
            Form form = formDAL.GetForm(formInstance.FormDefinitionId);
            AIResponseBody deserializedResponse = JsonConvert.DeserializeObject<AIResponseBody>(responseBody);
            if (deserializedResponse != null)
            {
                new AIResponseParser(form.GetAllFieldSets(), formInstance, deserializedResponse).HandleResponse();
            }
        }
        #endregion /AI Extraction

        private RestResponse GetResponse(Common.Entities.RestRequestData restRequestData, SReportsContext sReportsContext, IAuthenticator authenticator=null)
        {
            return new RestRequestSender(sReportsContext).GetResponse(restRequestData, authenticator);
        }
    }
}
