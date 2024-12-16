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
                asyncRunner.Run<IFormInstanceBLL>((standaloneFormInstanceBLL) =>
                    standaloneFormInstanceBLL.PassDataToOomniaApi(lockAction, userCookieData)
                );
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
                LogHelper.Error(@"Pass Data to Oomnia error: " + ex.Message);
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
                                Endpoint = "save-documents-from-sreports",
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
            OrganizationDataOut organization = Mapper.Map<OrganizationDataOut>(organizationDAL.GetById(formInstance.OrganizationId));

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
            PatientDataOut patient = Mapper.Map<PatientDataOut>(patientDAL.GetById(formInstance.PatientId));
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
            IEnumerable<FieldSet> fieldSets = GetFieldSets(lockAction, formDefinition);
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

        private IEnumerable<FieldSet> GetFieldSets(LockActionToOomniaApiDTO lockAction, Form form)
        {
            if (!string.IsNullOrEmpty(lockAction.PageId))
            {
                return form.GetFieldSetIdsInPage(lockAction.ChapterId, lockAction.PageId);
            }
            else if (!string.IsNullOrEmpty(lockAction.ChapterId))
            {
                return form.GetFieldSetIdsInChapter(lockAction.ChapterId);
            }
            else
            {
                return new List<FieldSet>();
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

        private RestResponse GetResponse(Common.Entities.RestRequestData restRequestData, SReportsContext sReportsContext, IAuthenticator authenticator=null)
        {
            return new RestRequestSender(sReportsContext).GetResponse(restRequestData, authenticator);
        }
    }
}
