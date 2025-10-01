using AutoMapper;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Helpers;
using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.Domain.Sql.Entities.Patient;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using sReportsV2.Domain.Sql.Entities.User;
using sReportsV2.DTOs.DTOs.FormInstance.DataIn;
using sReportsV2.DTOs.Form.DataOut;
using sReportsV2.DTOs.User.DataIn;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using sReportsV2.Common.Extensions;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using System.Net.Mime;
using Hl7.Fhir.Serialization;
using sReportsV2.DTOs.DTOs.Fhir.DataIn;
using sReportsV2.DTOs.User.DTO;
using sReportsV2.BusinessLayer.Helpers;
using System.Collections.Specialized;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;

namespace sReportsV2.BusinessLayer.Implementations
{
    public class FhirBLL : IFhirBLL
    {
        private readonly IFormDAL formDAL;
        private readonly IPersonnelDAL userDAL;
        private readonly IThesaurusDAL thesaurusDAL;
        private readonly ICodeDAL codeDAL;
        private readonly IEncounterDAL encounterDAL;
        private readonly IEpisodeOfCareDAL episodeOfCareDAL;
        private readonly IPatientDAL patientDAL;
        private readonly IFormInstanceDAL formInstanceDAL;
        private readonly IFormInstanceBLL formInstanceBLL; 
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;
        private readonly IHttpContextAccessor httpContextAccessor;

        public FhirBLL(IFormDAL formDAL,
            IPersonnelDAL userDAL,
            IThesaurusDAL thesaurusDAL,
            ICodeDAL codeDAL,
            IPatientDAL patientDAL,
            IFormInstanceDAL formInstanceDAL,
            IEncounterDAL encounterDAL,
            IEpisodeOfCareDAL episodeOfCareDAL,
            IFormInstanceBLL formInstanceBLL, 
            IMapper mapper,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor
            )
        {
            this.formDAL = formDAL;
            this.userDAL = userDAL;
            this.thesaurusDAL = thesaurusDAL;
            this.codeDAL = codeDAL;
            this.patientDAL = patientDAL;
            this.formInstanceDAL = formInstanceDAL;
            this.encounterDAL = encounterDAL;
            this.episodeOfCareDAL = episodeOfCareDAL;
            this.formInstanceBLL = formInstanceBLL;
            this.mapper = mapper;
            this.configuration = configuration;
            this.httpContextAccessor = httpContextAccessor;
        }

        #region Questionnaire(Response)
        public Questionnaire ExportFormToQuestionnaire(string formId)
        {
            FormDataOut form = mapper.Map<FormDataOut>(formDAL.GetForm(formId));
            return new QuestionnaireExportHelper(form, UrlHelper.GetBaseUrl(httpContextAccessor?.HttpContext?.Request), mapper).CreateQuestionnaire();
        }

        public async System.Threading.Tasks.Task CreateOrUpdateFormInstanceFromQuestionnaireResponse(QuestionnaireResponse questionnaireResponse, UserCookieData userCookieData)
        {
            questionnaireResponse = Ensure.IsNotNull(questionnaireResponse, nameof(questionnaireResponse));

            Uri questionnaireURI = new Uri(questionnaireResponse?.Questionnaire);  // Getting form TITLE from Questionnaire Field (e.g. http://fhir-server-url:8080/smaragd/fhir/Questionnaire?title=Example)
            NameValueCollection queryDictionary = System.Web.HttpUtility.ParseQueryString(questionnaireURI?.Query);  // Handles special characters like %20
            string formTitle = queryDictionary?.Get("title");
            formTitle = Ensure.IsNotNullOrWhiteSpace(formTitle, nameof(formTitle));

            List<Form> forms = await formDAL.GetByTitle(formTitle).ConfigureAwait(false);
            Form form = forms.FirstOrDefault();
            
            if (form != null)
            {
                bool isUpdate = false;

                FormInstance formInstance = null;
                if (!string.IsNullOrWhiteSpace(questionnaireResponse.Identifier?.Value))
                {
                    formInstance = formInstanceBLL.GetById(questionnaireResponse.Identifier.Value);
                    isUpdate = true;
                }
                else
                {
                    formInstance = formInstanceBLL.GetFormInstanceSet(form, formInstanceDataIn: null, userCookieData, setFieldsFromRequest: false);
                }

                BuildFormInstanceFromQuestionnaireResponse(form, formInstance, questionnaireResponse);

                if (isUpdate)
                    _ = formInstanceBLL.InsertOrUpdateManyFieldHistoriesAsync(formInstance)
                        .ContinueWith(task => task.LogTaskFailure("Error while updating QuestionnaireResponse field history: " + task.Exception.ToString())
                        ,TaskContinuationOptions.OnlyOnFaulted); 
            }
            else
            {
                throw new ArgumentNullException($"Form couldn't be retrieved from the specified Questionnaire : {questionnaireResponse?.Questionnaire}");
            }
        }


        private void BuildFormInstanceFromQuestionnaireResponse(Form form, FormInstance formInstance, QuestionnaireResponse questionnaireResponse)
        {
            int? userId;
            if (formInstance.UserId > 0)
                userId = formInstance.UserId;
            else
                userId = CreateCustomUserIfDoesNotExist(new UserDataIn { Username = "fhir.user", FirstName = "Fhir", LastName = "User", Email = "fhir.user@test.com" });

            UpdateBasicInfoFromQuestionnaireResponse(form, formInstance, questionnaireResponse, userId);
            SetFieldValuesFormQuestionnaireResponse(form, formInstance, questionnaireResponse);
            formInstanceDAL.InsertOrUpdate(formInstance, formInstance.GetCurrentFormInstanceStatus(userId));
        }

        private void UpdateBasicInfoFromQuestionnaireResponse(Form form, FormInstance formInstance, QuestionnaireResponse questionnaireResponse, int? userId)
        {
            formInstance.UserId = userId.GetValueOrDefault();

            TrySetPatientFromQuestionnaireResponse(formInstance, questionnaireResponse);    
            TrySetEncounterFromQuestionnaireResponse(formInstance, questionnaireResponse);

            formInstance.FormState = GetFormState(questionnaireResponse?.Status.ToString());
            formInstance.Date = GetDateWhenAnsweredAreGathered(questionnaireResponse.Authored);

            if (formInstance.OrganizationId <= 0)
                formInstance.OrganizationId = form.OrganizationIds.FirstOrDefault();
        }

        private void TrySetEncounterFromQuestionnaireResponse(FormInstance formInstance, QuestionnaireResponse questionnaireResponse)
        {
            int? encounterId = questionnaireResponse.Encounter?.Reference.ExtractIntFromString();
            if (encounterId.HasValue && encounterId.Value > 0)
            {
                Domain.Sql.Entities.Encounter.Encounter encounter = encounterDAL.GetById(encounterId.Value);
                if (encounter != null && encounter.PatientId == formInstance.PatientId)
                {
                    formInstance.EpisodeOfCareRef = encounter.EpisodeOfCareId;
                    formInstance.EncounterRef = encounter.EncounterId;
                }
            }
        }

        private void TrySetPatientFromQuestionnaireResponse(FormInstance formInstance, QuestionnaireResponse questionnaireResponse)
        {
            int? patientId = questionnaireResponse.Subject?.Reference.ExtractIntFromString();
            if (patientId.HasValue && patientId.Value > 0)
            {
                Domain.Sql.Entities.Patient.Patient patient = patientDAL.GetById(patientId.Value);
                if (patient != null)
                {
                    formInstance.PatientId = patient.PatientId;
                }
            }
        }

        private FormState? GetFormState(string questionnaireResponseStatus)
        {
            FormState? formState = null;

            if (!string.IsNullOrEmpty(questionnaireResponseStatus))
            {
                if (questionnaireResponseStatus == QuestionnaireResponse.QuestionnaireResponseStatus.InProgress.ToString())
                {
                    formState = FormState.OnGoing;
                }
                else
                {
                    formState = FormState.Finished;
                }
            }

            return formState;
        }

        private DateTime GetDateWhenAnsweredAreGathered(string authored)
        {
            if (DateTime.TryParse(authored, out DateTime dateWhenAnswersGathered))
            {
                return dateWhenAnswersGathered;
            }
            else
            {
                return DateTime.Now;
            }
        }

        private void SetFieldValuesFormQuestionnaireResponse(Form form, FormInstance formInstance, QuestionnaireResponse questionnaireResponse)
        {
            IList<Field> fields = form.GetAllFields();
            List<FieldInstance> fieldInstances = new List<FieldInstance>();

            foreach(QuestionnaireResponse.ItemComponent chapterItem in questionnaireResponse.Item)
            {
                foreach (QuestionnaireResponse.ItemComponent pageItem in chapterItem.Item)
                {
                    foreach (QuestionnaireResponse.ItemComponent fieldsetItem in pageItem.Item)
                    {
                        string fieldSetInstanceRepetitionId = GuidExtension.NewGuidStringWithoutDashes();
                        foreach (QuestionnaireResponse.ItemComponent fieldItem in fieldsetItem.Item)
                        {
                            Field field = fields.FirstOrDefault(x => x.Id == fieldItem.LinkId);
                            if (field != null)
                            {
                                FieldInstance fieldInstance = new FieldInstance(field, fieldsetItem.LinkId, fieldSetInstanceRepetitionId);
                                PopulateFieldInstanceFromQuestionnaireItem(fieldInstance, field, fieldItem);
                                fieldInstances.Add(fieldInstance);
                            }
                        }
                    }
                }
            }

            formInstance.FieldInstances = fieldInstances;
        }

        private void PopulateFieldInstanceFromQuestionnaireItem(FieldInstance fieldInstance, Field field, QuestionnaireResponse.ItemComponent fieldItem)
        {
            if (field is FieldCheckbox fieldCheckbox)  
            {
                IEnumerable<string> selectedLabels = fieldItem.Answer.Select(a => (a?.Value as Coding)?.Display);  // FHIR doesn't have "multiple answers" option => CheckBox Answers considered as Repetitive Values.
                fieldInstance.AddValue(GetValueForSelectableInput(fieldCheckbox, String.Join(",", selectedLabels)));
            }
            else
            {
                foreach (var answer in fieldItem.Answer)
                {
                    fieldInstance.AddValue(GetFieldInstanceValueFromQuestionnaireAnswer(field, answer));
                }
            }
        }


        private FieldInstanceValue GetFieldInstanceValueFromQuestionnaireAnswer(Field field, QuestionnaireResponse.AnswerComponent answer)
        {
            FieldInstanceValue fieldInstanceValue = null;
            if (answer != null)
            {
                if (field is FieldSelectable fieldSelectable && answer.Value is Coding valueCoding)
                {
                    string selectableLabel = valueCoding?.Display;
                    fieldInstanceValue = GetValueForSelectableInput(fieldSelectable, selectableLabel);
                }
                else
                {
                    fieldInstanceValue = new FieldInstanceValue(answer.Value?.ToString());
                }
            }

            return fieldInstanceValue;
        }

        #endregion /Questionnaire(Response)

        #region Mimacom
        public FormInstanceJsonDTO ExportFormToMimacom(string formId)
        {
            Form form = formDAL.GetForm(formId);

            return new FormInstanceJsonDTO()
            {
                FormId = formId,
                MedicalRecordNumber = "ENTER_VALUE",
                Fields = GetFieldsInMimacom(form)
            };
        }

        public void InsertFromJson(Form form, FormInstance formInstance, FormInstanceJsonDTO formInstanceJsonInput)
        {
            int? mimacomUserId = CreateCustomUserIfDoesNotExist(new UserDataIn { Username = "mimacom.user", FirstName = "Mimacom", LastName = "User", Email = "mimacom.user@test.com" });
            formInstance.UserId = mimacomUserId.GetValueOrDefault();
            formInstance.PatientId = GetPatientIdFromMedicalRecordIdentifier(formInstanceJsonInput.MedicalRecordNumber);
            formInstance.OrganizationId = 1;
            TrySetEncounterId(formInstance);
            SetFieldValuesFormJson(form, formInstance, formInstanceJsonInput.Fields);
            formInstanceDAL.InsertOrUpdate(formInstance, formInstance.GetCurrentFormInstanceStatus(mimacomUserId));
        }

        private void SetFieldValuesFormJson(Form form, FormInstance formInstance, Dictionary<string, string> formInstanceFromJsonInput)
        {
            IList<Field> fields = form.GetAllFields();
            (Dictionary<string, string> fieldToFieldSetMapping, Dictionary<string, string> fieldSetInstanceRepetitionIds) = form.GetFieldToFieldSetMapping();
            List<FieldInstance> fieldInstances = new List<FieldInstance>();
            foreach (KeyValuePair<string, string> fieldInput in formInstanceFromJsonInput)
            {
                int thesaurusId = thesaurusDAL.GetThesaurusIdThatHasCodeableConcept(fieldInput.Key);
                if (thesaurusId > 0)
                {
                    Field field = fields.FirstOrDefault(f => f.ThesaurusId == thesaurusId);
                    if (field != null)
                    {
                        string fieldSetId = fieldToFieldSetMapping[field.Id];
                        fieldInstances.Add(new FieldInstance(
                            field,
                            fieldSetId,
                            fieldSetInstanceRepetitionIds[fieldSetId],
                            GetFieldValueFromJson(field, fieldInput.Value)
                            ));
                    }
                }
            }
            formInstance.FieldInstances = fieldInstances;
        }

        private FieldInstanceValue GetFieldValueFromJson(Field field, string fieldInputValue)
        {
            FieldInstanceValue fieldInstanceValue = null;

            if (!string.IsNullOrEmpty(fieldInputValue))
            {
                switch (field.Type)
                {
                    case FieldTypes.Checkbox:
                    case FieldTypes.Select:
                    case FieldTypes.Radio:
                        fieldInstanceValue = GetValueForSelectableInput(field as FieldSelectable, fieldInputValue);
                        break;
                    case FieldTypes.Date:
                        if (DateTime.TryParseExact(
                            fieldInputValue,
                            DateTimeConstants.DateFormat,
                            CultureInfo.InvariantCulture,
                            DateTimeStyles.None,
                            out DateTime parsedDate))
                        {
                            fieldInstanceValue = new FieldInstanceValue(parsedDate.GetDateTimeDisplay(DateTimeConstants.UTCDatePartFormat, excludeTimePart: true));
                        }
                        break;
                    default:
                        fieldInstanceValue = new FieldInstanceValue(fieldInputValue);
                        break;
                }
            }

            return fieldInstanceValue;
        }

        private FieldInstanceValue GetValueForSelectableInput(FieldSelectable fieldSelectable, string fieldInputValue)
        {
            List<string> selectedOptionsIds = new List<string>();
            if (!string.IsNullOrEmpty(fieldInputValue))
            {
                foreach (string selectedOptionLabel in fieldInputValue.Split(','))
                {
                    FormFieldValue formFieldValue = fieldSelectable.Values.Find(x => x.Label == selectedOptionLabel);
                    if (formFieldValue != null)
                    {
                        selectedOptionsIds.Add(formFieldValue.Id);
                    }
                }
            }

            return fieldSelectable.CreateFieldInstanceValue(selectedOptionsIds);
        }

        private int GetPatientIdFromMedicalRecordIdentifier(string medicalRecordIdentifier)
        {
            int patientId = 0;
            if (!string.IsNullOrEmpty(medicalRecordIdentifier))
            {
                int? medicalRecordThesaurusId = codeDAL.GetByPreferredTerm(ResourceTypes.MedicalRecordNumber, (int)CodeSetList.PatientIdentifierType)?.CodeId;
                PatientIdentifier medicalRecordQueryIdentifier = new PatientIdentifier(medicalRecordThesaurusId, medicalRecordIdentifier, null);
                Domain.Sql.Entities.Patient.Patient patient = patientDAL.GetByIdentifier(medicalRecordQueryIdentifier);
                if (patient != null)
                {
                    patientId = patient.PatientId;
                }
            }
            return patientId;
        }

        private Dictionary<string, string> GetFieldsInMimacom(Form form)
        {
            IList<Field> fields = form.GetAllFields();
            List<ThesaurusEntry> thesauruses = GetFieldThesauruses(fields);
            Dictionary<string, string> mimacomFields = new Dictionary<string, string>();

            foreach (var field in fields)
            {
                string mimacomKey = "";
                ThesaurusEntry thesaurusForField = thesauruses.Find(th => th.ThesaurusEntryId == field.ThesaurusId);
                if (thesaurusForField != null)
                {
                    O4CodeableConcept code = thesaurusForField.Codes.Find(c => !c.IsDeleted);
                    if (code != null)
                    {
                        mimacomKey = code.Value;
                    }
                    if (!string.IsNullOrEmpty(mimacomKey))
                    {
                        mimacomFields.Add(mimacomKey, GetMimacomValue(field));
                    }
                }
            }

            return mimacomFields;
        }

        private List<ThesaurusEntry> GetFieldThesauruses(IList<Field> fields)
        {
            IEnumerable<int> thesaurusIds = fields.Select(x => x.ThesaurusId);
            return thesaurusDAL.GetAllByIds(thesaurusIds);
        }

        private string GetMimacomValue(Field field)
        {
            switch (field.Type)
            {
                case FieldTypes.Radio:
                    return string.Join(" | ", ((FieldSelectable)field).Values.Select(value => value.Label));
                case FieldTypes.Checkbox:
                    return string.Join(",", ((FieldSelectable)field).Values.Select(value => value.Label));
                case FieldTypes.Date:
                    return DateTimeConstants.DateFormat;
                default:
                    return "ENTER_VALUE";
            }
        }

        private void TrySetEncounterId(FormInstance formInstance)
        {
            Domain.Sql.Entities.EpisodeOfCare.EpisodeOfCare episodeOfCare = episodeOfCareDAL.GetByPatientId(formInstance.PatientId).FirstOrDefault();
            if (episodeOfCare != null)
            {
                Domain.Sql.Entities.Encounter.Encounter encounter = encounterDAL.GetAllByEocId(episodeOfCare.EpisodeOfCareId).FirstOrDefault();
                if (encounter != null)
                {
                    formInstance.EpisodeOfCareRef = episodeOfCare.EpisodeOfCareId;
                    formInstance.EncounterRef = encounter.EncounterId;
                }
            }
        }

        #endregion /Mimacom

        #region FhirDocumentReference
        public async Task<bool> GenerateDocumentReferenceForDataExtraction(DataExtractionDataIn dataExtractionDataIn)
        {
            dataExtractionDataIn = Ensure.IsNotNull(dataExtractionDataIn, nameof(dataExtractionDataIn));
            Ensure.IsNotNullOrWhiteSpace(dataExtractionDataIn.FormInstanceId, nameof(dataExtractionDataIn.FormInstanceId));
            Ensure.IsNotNullOrWhiteSpace(dataExtractionDataIn.FieldIdWithDataToExtract, nameof(dataExtractionDataIn.FieldIdWithDataToExtract));
            Ensure.IsNotNullOrWhiteSpace(dataExtractionDataIn.FieldInstanceIdWithDataToExtract, nameof(dataExtractionDataIn.FieldInstanceIdWithDataToExtract));

            FormInstance formInstance = await formInstanceBLL.GetByIdAsync(dataExtractionDataIn.FormInstanceId);

            DocumentReference documentReference = CreateDocumentReferenceRequestBody(formInstance, dataExtractionDataIn.FieldIdWithDataToExtract, dataExtractionDataIn.FieldInstanceIdWithDataToExtract);

            return formInstanceBLL.SendIntegrationEngineRequest(configuration["DocumentReferenceDataExtractionEndPoint"], configuration["IntegrationEnginePort"], documentReference.ToJson());
        }


        private DocumentReference CreateDocumentReferenceRequestBody(FormInstance formInstance, string fieldIdWithDataToExtract, string fieldInstanceIdWithDataToExtract)
        {
            string textToExtract = formInstance?.FieldInstances?
                .Find(f => f.FieldId == fieldIdWithDataToExtract)?.FieldInstanceValues?
                .Find(fiv => fiv.FieldInstanceRepetitionId == fieldInstanceIdWithDataToExtract)?.ValueLabel;

            string requestUrl = configuration["IntegrationEngineUrl"] + ":" + configuration["FhirServerPort"];
            string questionnaireDataExtractionEndPoint = configuration["QuestionnaireDataExtractionEndPoint"];
            string questionnaireResponseDataExtractionEndPoint = configuration["QuestionnaireResponseDataExtractionEndPoint"];

            DocumentReference documentReference = new DocumentReference()
            {
                Identifier = new List<Identifier>() { new Identifier(null, formInstance.Id) },
                Status = DocumentReferenceStatus.Current,
                Contained = new List<Resource>()
                {
                    new Hl7.Fhir.Model.Patient(){ Id = formInstance.PatientId.ToString() },
                    new Hl7.Fhir.Model.Encounter() { Id =  formInstance.EncounterRef.ToString() }
                },
                Content = new List<DocumentReference.ContentComponent>()
                {
                    new DocumentReference.ContentComponent() { Attachment  = new Attachment() { ContentType = MediaTypeNames.Text.Plain, Data = textToExtract.Base64Encode() } }
                },
                Extension = new List<Extension>()
                {
                    new Extension(FhirResourcesConstant.QuestionnaireDefinitionUrl, new FhirUrl($"{requestUrl}{questionnaireDataExtractionEndPoint}?title={formInstance.Title}")),
                    new Extension(FhirResourcesConstant.QuestionnaireResponseDefinitionUrl, new FhirUrl($"{requestUrl}{questionnaireResponseDataExtractionEndPoint}"))
                }
            };

            return documentReference;
        }
        #endregion

        private int? CreateCustomUserIfDoesNotExist(UserDataIn userDataIn)
        {
            string mimacomUsername = userDataIn.Username;
            Personnel user = userDAL.GetByUsername(mimacomUsername);

            if (user == null)
            {
                string salt = PasswordHelper.CreateSalt(10);
                user = new Personnel(mimacomUsername, PasswordHelper.Hash(ResourceTypes.DefaultPass, salt), salt, userDataIn.Email, userDataIn.FirstName, userDataIn.LastName, DateTime.Now);
                userDAL.InsertOrUpdate(user);
            }

            return user?.PersonnelId;
        }

    }
}