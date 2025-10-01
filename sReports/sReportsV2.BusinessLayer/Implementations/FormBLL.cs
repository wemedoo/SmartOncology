using AutoMapper;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Newtonsoft.Json;
using sReportsV2.BusinessLayer.Components.Interfaces;
using sReportsV2.BusinessLayer.Helpers;
using sReportsV2.BusinessLayer.Helpers.TabularExportGenerator;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Cache.Resources;
using sReportsV2.Cache.Singleton;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Entities.User;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Exceptions;
using sReportsV2.Common.Extensions;
using sReportsV2.Common.File;
using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Entities.DocumentProperties;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Domain.MongoDb.Entities.FormInstance;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.Domain.Sql.Entities.CodeEntry;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Domain.Sql.Entities.OrganizationEntities;
using sReportsV2.Domain.Sql.Entities.TaskEntry;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using sReportsV2.Domain.Sql.Entities.User;
using sReportsV2.DTOs.Autocomplete;
using sReportsV2.DTOs.Common.DataOut;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.CTCAE.DataIn;
using sReportsV2.DTOs.DTOs.Form.DataIn;
using sReportsV2.DTOs.DTOs.Form.DataOut;
using sReportsV2.DTOs.DTOs.FormInstance.DataIn;
using sReportsV2.DTOs.DTOs.QueryManagement.DataOut;
using sReportsV2.DTOs.Field.DataIn;
using sReportsV2.DTOs.Field.DataOut;
using sReportsV2.DTOs.Form;
using sReportsV2.DTOs.Form.DataIn;
using sReportsV2.DTOs.Form.DataOut;
using sReportsV2.DTOs.Form.DataOut.Tree;
using sReportsV2.DTOs.FormInstance.DataOut;
using sReportsV2.DTOs.Organization;
using sReportsV2.DTOs.Pagination;
using sReportsV2.DTOs.User.DataOut;
using sReportsV2.DTOs.User.DTO;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace sReportsV2.BusinessLayer.Implementations
{
    public partial class FormBLL : IFormBLL
    {
        private readonly IPersonnelDAL userDAL;
        private readonly IOrganizationDAL organizationDAL;
        private readonly IFormDAL formDAL;
        private readonly IFormInstanceDAL formInstanceDAL;
        private readonly IThesaurusDAL thesaurusDAL;
        private readonly ITaskDAL taskDAL;
        private readonly ICodeDAL codeDAL;
        private readonly ICodeAssociationDAL codeAssociationDAL;
        private readonly IFormCodeRelationDAL formCodeRelationDAL;
        private readonly IAsyncRunner asyncRunner;
        private readonly IMapper mapper;
        private readonly IEmailSender emailSender;
        private readonly ICacheRefreshService cacheRefreshService;
        private readonly IQueryManagementDAL queryManagementDAL;

        public FormBLL(IPersonnelDAL userDAL, IOrganizationDAL organizationDAL, IFormDAL formDAL, IThesaurusDAL thesaurusDAL, ITaskDAL taskDAL, ICodeDAL codeDAL, ICodeAssociationDAL codeAssociationDAL, IFormCodeRelationDAL formCodeRelationDAL, IAsyncRunner asyncRunner, IMapper mapper, IEmailSender emailSender, IFormInstanceDAL formInstanceDAL, ICacheRefreshService cacheRefreshService, IQueryManagementDAL queryManagementDAL)
        {
            this.organizationDAL = organizationDAL;
            this.userDAL = userDAL;
            this.formDAL = formDAL;
            this.thesaurusDAL = thesaurusDAL;
            this.taskDAL = taskDAL;
            this.codeDAL = codeDAL;
            this.codeAssociationDAL = codeAssociationDAL;
            this.formCodeRelationDAL = formCodeRelationDAL;
            this.asyncRunner = asyncRunner;
            this.mapper = mapper;
            this.emailSender = emailSender;
            this.formInstanceDAL = formInstanceDAL;
            this.cacheRefreshService = cacheRefreshService;
            this.queryManagementDAL = queryManagementDAL;
        }

        #region Get Filtered Forms
        public List<FormInstancePerDomainDataOut> GetFormInstancePerDomain(string activeLanguage)
        {
            return mapper.Map<List<FormInstancePerDomainDataOut>>(this.formDAL.GetFormInstancePerDomain(),
                opts => opts.Items["Language"] = activeLanguage)
                .Where(f => !string.IsNullOrEmpty(f.Label))
                .ToList();
        }

        public PaginationDataOut<FormDataOut, FormFilterDataIn> ReloadData(FormFilterDataIn dataIn, UserCookieData userCookieData)
        {
            FormFilterData filterData = GetFormFilterData(dataIn, userCookieData);
            PaginationDataOut<FormDataOut, FormFilterDataIn> result = new PaginationDataOut<FormDataOut, FormFilterDataIn>()
            {
                Count = (int)this.formDAL.GetAllFormsCount(filterData),
                Data = mapper.Map<List<FormDataOut>>(this.formDAL.GetAll(filterData)),
                DataIn = dataIn
            };
            return result;
        }

        public List<FieldDataOut> GetPlottableFields(string formId)
        {
            List<BsonDocument> bsonFields = formDAL.GetPlottableFields(formId);
            List<Field> fields = new List<Field>();
            BsonElement fieldsDict;

            if (bsonFields != null && bsonFields.Count > 0)
            {
                foreach (var bson in bsonFields)
                {
                    if (bson.TryGetElement("Fields", out fieldsDict))
                    {
                        fields.Add(BsonSerializer.Deserialize<Field>((BsonDocument)fieldsDict.Value));
                    }
                }
            }

            return mapper.Map<List<FieldDataOut>>(fields);
        }

        public async Task<AutocompleteResultDataOut> GetTitleDataForAutocomplete(AutocompleteDataIn dataIn, UserCookieData userCookieData)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));

            FormFilterData formFilterData = new FormFilterData
            {
                Title = dataIn.Term,
                OrganizationId = userCookieData.ActiveOrganization,
                ActiveLanguage = userCookieData.ActiveLanguage,
                Page = dataIn.Page,
                PageSize = FilterConstants.DefaultPageSize
            };
            Task<List<Form>> formsTask = formDAL.GetByTitleForAutoComplete(formFilterData);
            Task<long> countTask = formDAL.CountByTitle(formFilterData);

            await System.Threading.Tasks.Task.WhenAll(formsTask, countTask);

            List<AutocompleteDataOut> autocompleteDataDataOuts = formsTask.Result
                .Select(x => new AutocompleteDataOut()
                {
                    id = x.Id,
                    text = x.GetTitleWithVersion(),
                })
                .ToList();

            AutocompleteResultDataOut result = new AutocompleteResultDataOut()
            {
                results = autocompleteDataDataOuts,
                pagination = new AutocompletePaginatioDataOut() { more = dataIn.ShouldLoadMore((int)countTask.Result) }
            };

            return result;
        }

        public long GetThesaurusAppereanceCount(int o4mtId, string searchTerm, int? organizationId = null)
        {
            return formDAL.GetThesaurusAppereanceCount(o4mtId, searchTerm, organizationId);
        }

        public DocumentProperties GetDocumentProperties(string formId)
        {
            return formDAL.GetDocumentProperties(formId);
        }

        private FormFilterData GetFormFilterData(FormFilterDataIn formDataIn, UserCookieData userCookieData)
        {
            FormFilterData result = mapper.Map<FormFilterData>(formDataIn);
            PopulateFormStates(result);
            result.OrganizationId = userCookieData.ActiveOrganization;
            result.ActiveLanguage = userCookieData.ActiveLanguage;
            return result;
        }

        private void PopulateFormStates(FormFilterData dataIn)
        {
            foreach (var state in (FormDefinitionState[])Enum.GetValues(typeof(FormDefinitionState)))
                dataIn.FormStates.Add(TextLanguage.ResourceManager.GetString(state.ToString()));
        }
        #endregion /Get Filtered Forms

        #region Get Form
        public Form GetFormById(string formId)
        {
            return formDAL.GetForm(formId);
        }

        public async Task<Form> GetFormByIdAsync(string formId)
        {
            return await formDAL.GetFormAsync(formId).ConfigureAwait(false);
        }


        public TreeDataOut GetTreeDataOut(int thesaurusId, int thesaurusPageNum, string searchTerm, UserCookieData userCookieData = null)
        {
            var forms = formDAL.GetFilteredDocumentsByThesaurusAppeareance(thesaurusId, searchTerm, thesaurusPageNum, userCookieData?.ActiveOrganization);
            TreeDataOut result = new TreeDataOut()
            {
                Forms = forms.Count != 0 ? mapper.Map<List<FormTreeDataOut>>(forms) : new List<FormTreeDataOut>(),
                O4MtId = thesaurusId
            };

            foreach (FormTreeDataOut form in result.Forms)
            {
                form.ThesaurusAppearances = forms.Find(x => x.Id == form.Id).GetAllThesaurusIds().Count(t => t == thesaurusId);
            }

            return result;
        }
        private Form GetForm(FormInstance formInstance, UserCookieData userCookieData)
        {
            Form form = null;
            if (!formInstance.Language.Equals(userCookieData.ActiveLanguage))
            {
                form = this.formDAL.GetFormByThesaurusAndLanguageAndVersionAndOrganization(formInstance.ThesaurusId, userCookieData.ActiveOrganization, userCookieData.ActiveLanguage, formInstance.Version.Id);
            }
            if (form == null)
            {
                form = this.GetFormById(formInstance.FormDefinitionId);
            }


            return new Form(formInstance, form);
        }

        public FormDataOut GetFormDataOut(FormInstance formInstance, UserCookieData userCookieData, FormInstanceReloadDataIn formInstanceReloadData)
        {
            Form form = this.GetForm(formInstance, userCookieData);
            form.LastUpdate = formInstance.LastUpdate;

            FormDataOut data = this.SetFormDependablesAndReferrals(
                form, 
                new FormInstanceReferralDTO
                {
                    ActiveLanguage = userCookieData.ActiveLanguage,
                    FormInstance = formInstance,
                    SetReferralValues = formInstanceReloadData.SetReferrableValues
                }
            );

            PopulateFieldQueries(data, queryManagementDAL, mapper);

            data.Organizations = new List<OrganizationDataOut> { mapper.Map<OrganizationDataOut>(organizationDAL.GetById(formInstance.OrganizationId)) };

            data.SetDoesAllMandatoryFieldsHaveValue();
            FormInstanceItemLockingStatus formInstanceLockingStatus = formInstance.ExamineIfChaptersAndPagesAndFieldsetsAreLocked();
            data.SetIfChaptersAndPagesAreLocked(formInstanceLockingStatus);
            data.SetActiveChapterAndPageId(formInstanceReloadData);

            return data;
        }
        public Form GetFormByThesaurusAndLanguage(int thesaurusId, string language)
        {
            return this.formDAL.GetFormByThesaurusAndLanguage(thesaurusId, language);
        }

        public Form GetFormByThesaurusAndLanguageAndVersionAndOrganization(int thesaurusId, int organizationId, string activeLanguage, string versionId)
        {
            return this.formDAL.GetFormByThesaurusAndLanguageAndVersionAndOrganization(thesaurusId, organizationId, activeLanguage, versionId);
        }

        public FormDataOut GetFormDataOutById(string formId, UserCookieData userCookieData)
        {
            Form form = GetFormById(formId);
            return GetFormDataOut(form, userCookieData);
        }

        public FormDataOut GetFormDataOut(Form form, UserCookieData userCookieData)
        {
            FormDataOut dataOut = mapper.Map<FormDataOut>(form);
            dataOut.Organizations = mapper.Map<List<OrganizationDataOut>>(organizationDAL.GetByIds(form.OrganizationIds));
            SetFormWorkflowHistory(form, dataOut);
            return dataOut;
        }

        public async Task<FormDataOut> GetFormForGeneralInfoAsync(FormDataIn dataIn)
        {
            FormDataOut formDataOut = (!string.IsNullOrWhiteSpace(dataIn.Id) && dataIn.Id != "formIdPlaceHolder") ? mapper.Map<FormDataOut>(await GetFormByIdAsync(dataIn.Id).ConfigureAwait(false)) : mapper.Map<FormDataOut>(dataIn);
            formDataOut.Organizations = mapper.Map<List<OrganizationDataOut>>(organizationDAL.GetByIds(dataIn.OrganizationIds));
            if (formDataOut.CustomHeaderFields == null || formDataOut.CustomHeaderFields.Count == 0)
                formDataOut.CustomHeaderFields = CustomHeaderFieldDataOut.GetDefaultHeaders();

            return formDataOut;
        }

        public FormFieldSetDataOut AddFieldsetRepetition(string formId, string fieldSetId, List<FieldInstance> fieldInstances)
        {
            Form form = GetFormById(formId);
            string fieldSetInstanceRepetitionIdForNew = AddFieldSetRepetition(fieldSetId, fieldInstances);
            form.SetFieldInstances(
                fieldInstances
            );
            FormDataOut formDataOut = this.SetFormDependablesAndReferrals(form);
            FormFieldSetDataOut fieldSetDataOut = formDataOut.GetAllFieldSets().Find(fs => fs.FieldSetInstanceRepetitionId == fieldSetInstanceRepetitionIdForNew);
            fieldSetDataOut.SetParentFieldInstanceDependencies(formDataOut);

            return fieldSetDataOut;
        }

        private void SetFormWorkflowHistory(Form form, FormDataOut dataOut)
        {
            if (form.WorkflowHistory != null)
            {
                dataOut.WorkflowHistory = new List<FormStatusDataOut>();
                List<int> userIds = form.WorkflowHistory.Select(history => history.UserId).Distinct().ToList();
                Dictionary<int, UserShortInfoDataOut> users = userDAL.GetAllByIds(userIds).ToDictionary
                (
                    u => u.PersonnelId,
                    u => new UserShortInfoDataOut(u.FirstName, u.LastName)
                );
                foreach (FormStatus status in form.WorkflowHistory)
                {
                    users.TryGetValue(status.UserId, out UserShortInfoDataOut user);
                    dataOut.WorkflowHistory.Add(new FormStatusDataOut()
                    {
                        Created = status.Created,
                        Status = status.Status,
                        User = user
                    });
                }
            }
        }

        public async Task<FormGenerateNewLanguageDataOut> GetGenerateNewLanguage(string formId, int activeOrganization)
        {
            Form form = await formDAL.GetFormAsync(formId).ConfigureAwait(false);
            if (form.Invalid) throw new UserAdministrationException((int)HttpStatusCode.Conflict, "Cannot generate new translation! Form is invalid!");
            List<string> alreadyGeneratedLanguages = await formDAL.GetGeneratedLanguages(form.ThesaurusId, activeOrganization, form.Version).ConfigureAwait(false);
            return new FormGenerateNewLanguageDataOut
            {
                FormId = formId,
                Title = form.Title,
                PossibleLanguages = SingletonDataContainer.Instance.GetLanguages().Where(l => !alreadyGeneratedLanguages.Contains(l.Value)).ToList()
            };
        }

        public async System.Threading.Tasks.Task GenerateReport(string formId, UserCookieData userCookieData)
        {
            Form form = await formDAL.GetFormAsync(formId);
            Stream stream = new MemoryStream();
            ExcelWriter excelWriter = new ExcelWriter(stream);
            List<FormThesaurusReportView> formThesaurusReport = await thesaurusDAL.GetFormThesaurusReportView(form.ThesaurusIdsList);
            FormThesaurusReportExportGenerator formThesaurusReportExport = new FormThesaurusReportExportGenerator(
                new TabularExportGeneratorInputParams
                {
                    FileWriter = excelWriter,
                    CurrentForm = form,
                    DateFormat = "dd-MMM-yyyy"
                }, 
                formThesaurusReport
            );
            formThesaurusReportExport.GenerateReportInExcel();
            excelWriter.FinalizeWriting(stream);

            string reportFileName = $"Thesaurus Code Report {form.Title}".GetSafeFileName();
            Dictionary<string, Stream> files = new Dictionary<string, Stream>()
            {
                { reportFileName, stream }
            };

            string emailContent = EmailHelpers.GetExportEmailContent(userCookieData, files.Keys);
            emailSender.SendAsync(new EmailDTO(userCookieData.Email, emailContent, $"Ready for Download: {reportFileName}")
            {
                Attachments = files,
                IsCsv = false
            });
        }

        public IEnumerable<FieldSet> GetFieldSetsByFieldLabels(string formId, FormFieldSetDataIn fieldset)
        {
            var firstFieldSet = fieldset?.ListOfFieldSets?.FirstOrDefault()?.Fields;
            List<string> fieldLabels = firstFieldSet?.Select(x => x.Label).ToList() ?? new List<string>();
            string fieldType = firstFieldSet?.Select(x => x.Type).FirstOrDefault();

            return fieldLabels.Any()
                ? formDAL.GetFieldSetsByFieldLabels(formId, fieldLabels, fieldType)
                : Enumerable.Empty<FieldSet>();
        }

        private string AddFieldSetRepetition(string fieldSetId, List<FieldInstance> fieldInstances)
        {
            string fieldsetRepetitionIdForNew = GuidExtension.NewGuidStringWithoutDashes();
            fieldInstances.Add(new FieldInstance
            {
                FieldSetInstanceRepetitionId = fieldsetRepetitionIdForNew,
                FieldSetId = fieldSetId
            });
            return fieldsetRepetitionIdForNew;
        }

        public string GetFormJson(FormDataIn formDataIn)
        {
            FormDataOut dataOut = mapper.Map<FormDataOut>(formDataIn);
            return dataOut.GetCustomMarkedPropertiesObject().JsonSerialize(true);
        }

        public bool ExistsFormByThesaurus(int thesaurusId)
        {
            return formDAL.ExistsFormByThesaurus(thesaurusId);
        }

        public bool ExistsForm(string formId)
        {
            return formDAL.ExistsForm(formId);
        }

        public Form GetFormByThesaurus(int thesaurusId)
        {
            return formDAL.GetFormByThesaurus(thesaurusId);
        }

        public Form GetFormByThesaurusAndVersion(int thesaurusId, string versionId)
        {
            return formDAL.GetFormByThesaurusAndVersion(thesaurusId, versionId);
        }

        #endregion /Get Form

        #region Save Actions
        public async Task<bool> Delete(string formId, DateTime lastUpdate, string organizationTimeZone)
        {
            bool deleted = formDAL.Delete(formId, lastUpdate);
            await formCodeRelationDAL.SetFormCodeRelationAndCodeToInactive(formId, organizationTimeZone);
            SetCodeAndTaskDocumentToInactive(formId, organizationTimeZone);

            return deleted;
        }

        public Form InsertOrUpdate(Form form, UserCookieData userCookieData, bool executeBackgroundTask = true)
        {
            var addedForm = formDAL.InsertOrUpdate(form, mapper.Map<UserData>(userCookieData));
            if (executeBackgroundTask)
            {
                ExecuteFormBackgroundTasks(addedForm, new FormBackgroundTaskDataIn
                {
                    CreatedById = userCookieData.Id,
                    AddCodeRelation = true,
                    AddTaskDocument = form.State == FormDefinitionState.ReadyForDataCapture && form.AvailableForTask,
                    UpdateTaskDocument = form.State != FormDefinitionState.ReadyForDataCapture || !form.AvailableForTask
                }, userCookieData);
            }

            return addedForm;
        }

        public async Task<string> InsertOrUpdateCustomHeaderFieldsAsync(string formId, List<CustomHeaderFieldDataIn> customHeaderFieldsDataIn, UserCookieData userCookieData)
        {
            formId = Ensure.IsNotNull(formId, nameof(formId));
            customHeaderFieldsDataIn = Ensure.IsNotNull(customHeaderFieldsDataIn, nameof(customHeaderFieldsDataIn));
            userCookieData = Ensure.IsNotNull(userCookieData, nameof(userCookieData));

            Form form = GetFormById(formId);
            if (form == null)
            {
                throw new EntryPointNotFoundException();
            }
            form.CustomHeaderFields = mapper.Map<List<CustomHeaderField>>(customHeaderFieldsDataIn);

            return await formDAL.InsertOrUpdateCustomHeaderFieldsAsync(form, mapper.Map<UserData>(userCookieData)).ConfigureAwait(false);
        }

        public ResourceCreatedDTO UpdateFormState(UpdateFormStateDataIn updateFormStateDataIn, UserCookieData userCookieData)
        {
            Form form = GetFormById(updateFormStateDataIn.Id);
            if (form == null)
            {
                throw new EntryPointNotFoundException();
            }

            ExecuteFormBackgroundTasks(form, new FormBackgroundTaskDataIn
            {
                CreatedById = userCookieData.Id,
                AddCodeRelation = false,
                AddTaskDocument = form.State != updateFormStateDataIn.State && updateFormStateDataIn.State == FormDefinitionState.ReadyForDataCapture && form.AvailableForTask,
                UpdateTaskDocument = form.State != updateFormStateDataIn.State && form.State == FormDefinitionState.ReadyForDataCapture
            }, userCookieData);

            form.State = updateFormStateDataIn.State;
            formDAL.InsertOrUpdate(form, mapper.Map<UserData>(userCookieData));

            return new ResourceCreatedDTO()
            {
                Id = form.Id,
                LastUpdate = form.LastUpdate.Value.ToString("o")
            };
        }

        public void DisableActiveFormsIfNewVersion(Form form, UserCookieData userCookieData)
        {
            if (!string.IsNullOrEmpty(form.Id))
            {
                Form formFromDatabase = GetFormById(form.Id);

                if (form.IsVersionChanged(formFromDatabase))
                {
                    form.Id = null;
                    form.Version.Id = Guid.NewGuid().ToString();
                    //set all common form state to disabled
                    formDAL.DisableFormsByThesaurusAndLanguageAndOrganization(formFromDatabase.ThesaurusId, userCookieData.ActiveOrganization, userCookieData.ActiveLanguage);
                }
            }
        }

        public bool TryGenerateNewLanguage(string formId, string language, UserCookieData userCookieData)
        {
            Form form = GetFormById(formId);
            if (form == null)
            {
                return false;
            }
            List<int> thesaurusList = form.GetAllThesaurusIds();
            UserData userData = mapper.Map<UserData>(userCookieData);
            List<ThesaurusEntry> entries = thesaurusDAL.GetByIdsList(thesaurusList);
            if (entries.Count.Equals(0))
            {
                form.Id = null;
                form.Language = language;
            }
            else
            {
                form.Id = null;
                form.GenerateTranslation(entries, language, form.Language);
            }
            formDAL.InsertOrUpdate(form, userData, false);

            return true;
        }
        #endregion /Save Actions

        #region Designer Copy-Paste

        public async Task<FormDataOut> PasteElements<T>(List<T> elements, string destinationFormId, string destinationElementId, bool afterDestination, UserCookieData userCookieData)
        {
            Task<Form> formTask = formDAL.GetFormTask(destinationFormId);
            Task<List<T>> updateIdsTask = AssignNewIds(elements);

            await System.Threading.Tasks.Task.WhenAll(formTask, updateIdsTask).ConfigureAwait(false);

            Form result = PasteNewElements(updateIdsTask.Result, formTask.Result, destinationElementId, afterDestination);
            return GetFormDataOut(InsertOrUpdate(result, userCookieData), userCookieData);
        }

        private Task<List<T>> AssignNewIds<T>(List<T> elements)
        {
            return System.Threading.Tasks.Task.Run(() =>
            {
                List<string> oldIds = new List<string>();
                GatherIds(elements, oldIds);

                string jsonElements = JsonConvert.SerializeObject(elements);

                foreach (string oldId in oldIds)
                {
                    string newId = oldId.Contains("-") ? Guid.NewGuid().ToString() : GuidExtension.NewGuidStringWithoutDashes();
                    jsonElements = jsonElements.Replace(oldId, newId);
                }

                return JsonConvert.DeserializeObject<List<T>>(jsonElements);
            });
        }

        private void GatherIds<T>(List<T> elements, List<string> ids)
        {
            foreach (T element in elements)
            {
                switch (element)
                {
                    case FormChapterDataIn chapter:
                        ids.Add(chapter.Id);
                        GatherIds(chapter.Pages, ids);
                        break;

                    case FormPageDataIn page:
                        ids.Add(page.Id);
                        GatherIds(page.ListOfFieldSets, ids);
                        break;

                    case List<FormFieldSetDataIn> fieldsets:
                        ids.Add(fieldsets.FirstOrDefault()?.Id);
                        GatherIds(fieldsets.FirstOrDefault()?.Fields, ids);
                        break;

                    case FieldDataIn field:
                        ids.Add(field.Id);
                        if (field is FieldSelectableDataIn fieldSelectable)
                        {
                            GatherIds(fieldSelectable.Values, ids);
                        }
                        break;
                    case FormFieldValueDataIn fieldFieldValue:
                        ids.Add(fieldFieldValue.Id);
                        break;

                    default:
                        break;
                }
            }
        }

        private Form PasteNewElements<T>(List<T> elements, Form destinationForm, string destinationElementId, bool afterDestination)
        {
            string[] destinationIdList = destinationElementId.Split('_');
            Dictionary<string, string> idsDict = new Dictionary<string, string>() {
                { ElementsIdConstants.ChapterId, destinationIdList.FirstOrDefault()},
                { ElementsIdConstants.PageId, destinationIdList.ElementAtOrDefault(1)},
                { ElementsIdConstants.FieldSetId, destinationIdList.ElementAtOrDefault(2)},
                { ElementsIdConstants.FieldId, destinationIdList.ElementAtOrDefault(3)},
            };

            int? index = -1;
            switch (elements)
            {
                case List<FormChapterDataIn> chapters:
                    index = idsDict[ElementsIdConstants.ChapterId] == ElementsIdConstants.First ? 0 : destinationForm.SearchChapterIndex(idsDict[ElementsIdConstants.ChapterId]);
                    if (index != null && index != -1)
                        destinationForm.InsertChapters(mapper.Map<List<FormChapter>>(chapters), (int)index, afterDestination);
                    break;
                case List<FormPageDataIn> pages:
                    index = idsDict[ElementsIdConstants.PageId] == ElementsIdConstants.First ? 0 : destinationForm.SearchPageIndex(idsDict[ElementsIdConstants.ChapterId], idsDict[ElementsIdConstants.PageId]);
                    if (index != null && index != -1)
                        destinationForm.InsertPages(mapper.Map<List<FormPage>>(pages), idsDict[ElementsIdConstants.ChapterId], (int)index, afterDestination);
                    break;

                case List<List<FormFieldSetDataIn>> fieldsets:
                    index = idsDict[ElementsIdConstants.FieldSetId] == ElementsIdConstants.First ? 0 : destinationForm.SearchFieldSetIndex(idsDict[ElementsIdConstants.ChapterId], idsDict[ElementsIdConstants.PageId], idsDict[ElementsIdConstants.FieldSetId]);
                    if (index != null && index != -1)
                        destinationForm.InsertFieldSets(mapper.Map<List<List<FieldSet>>>(fieldsets), idsDict[ElementsIdConstants.ChapterId], idsDict[ElementsIdConstants.PageId], (int)index, afterDestination);
                    break;

                case List<FieldDataIn> fields:
                    index = idsDict[ElementsIdConstants.FieldId] == ElementsIdConstants.First ? 0 : destinationForm.SearchFieldIndex(idsDict[ElementsIdConstants.ChapterId], idsDict[ElementsIdConstants.PageId], idsDict[ElementsIdConstants.FieldSetId], idsDict[ElementsIdConstants.FieldId]);
                    if (index != null && index != -1)
                        destinationForm.InsertFields(mapper.Map<List<Field>>(fields), idsDict[ElementsIdConstants.ChapterId], idsDict[ElementsIdConstants.PageId], idsDict[ElementsIdConstants.FieldSetId], (int)index, afterDestination);
                    break;

                default:
                    break;
            }
            return destinationForm;
        }

        #endregion /Designer Copy-Paste

        #region Null Flavors
        public bool IsNullFlavorUsedInAnyField(string formId, int nullFlavorId)
        {
            return formDAL.IsNullFlavorUsedInAnyField(formId, nullFlavorId);
        }

        public List<int> GetFormNullFlavors(string formId)
        {
            return formDAL.GetFormNullFlavors(formId);
        }
        #endregion /Null Flavors

        #region Code Methods
        private void AddFormToTaskDocumentTable(Form form, UserCookieData userCookieData)
        {
            int codeId = InsertTaskDocumentCode(form.ThesaurusId, userCookieData);
            cacheRefreshService.RefreshCache(codeId, ModifiedResourceType.Code);

            InsertTaskDocument(new TaskDocument(userCookieData.Id, userCookieData.OrganizationTimeZone)
            {
                TaskDocumentCD = codeId,
                FormId = form.Id,
                FormTitle = form.Title
            }, userCookieData.OrganizationTimeZone
            );
        }

        private int InsertTaskDocumentCode(int thesaurusId, UserCookieData userCookieData)
        {
            Code code = new Code(userCookieData.Id, userCookieData.OrganizationTimeZone)
            {
                ThesaurusEntryId = thesaurusId,
                CodeSetId = (int)CodeSetList.TaskDocument
            }
            ;

            return codeDAL.InsertOrUpdateTaskDocumentCode(code, userCookieData.OrganizationTimeZone);
        }

        private void InsertTaskDocument(TaskDocument taskDocument, string organizationTimeZone)
        {
            taskDAL.InsertTaskDocument(taskDocument, organizationTimeZone);
        }

        private void SetCodeAndTaskDocumentToInactive(string formId, string organizationTimeZone)
        {
            TaskDocument taskDocument = taskDAL.GetTaskDocumentByFormId(formId);
            taskDAL.SetTaskDocumentToInactive(taskDocument, organizationTimeZone);
            codeDAL.SetCodeToInactive(taskDocument.TaskDocumentCD, organizationTimeZone);
            cacheRefreshService.RefreshCache(taskDocument.TaskDocumentCD, ModifiedResourceType.Code);
        }

        private void AddFormCodeRelation(Form form, UserCookieData userCookieData)
        {
            if (!formCodeRelationDAL.HasFormCodeRelationByFormId(form.Id, userCookieData.OrganizationTimeZone))
            {
                int codeId = codeDAL.Insert(new Code(userCookieData.Id, userCookieData.OrganizationTimeZone)
                {
                    CodeSetId = (int)CodeSetList.Document,
                    ThesaurusEntryId = form.ThesaurusId,
                }, userCookieData.OrganizationTimeZone);
                formCodeRelationDAL.InsertFormCodeRelation(
                    new FormCodeRelation(userCookieData.Id, userCookieData.OrganizationTimeZone)
                    {
                        FormId = form.Id,
                        CodeCD = codeId
                    }, userCookieData.OrganizationTimeZone
                );
            }
        }
        #endregion /Code Methods

        #region Background Tasks
        public void ExecuteFormBackgroundTasksAfterSave(Form form, FormBackgroundTaskDataIn formBackgroundTask, UserCookieData userCookieData)
        {
            if (formBackgroundTask.AddCodeRelation)
            {
                AddFormCodeRelation(form, userCookieData);
            }
            if (formBackgroundTask.AddTaskDocument)
            {
                AddFormToTaskDocumentTable(form, userCookieData);
            }
            if (formBackgroundTask.UpdateTaskDocument && taskDAL.ExistTaskDocument(form.Id))
            {
                SetCodeAndTaskDocumentToInactive(form.Id, userCookieData.OrganizationTimeZone);
            }
        }

        private void ExecuteFormBackgroundTasks(Form form, FormBackgroundTaskDataIn formBackgroundTask, UserCookieData userCookieData)
        {
            asyncRunner.Run<IFormBLL>((standaloneFormBLL) =>
                standaloneFormBLL.ExecuteFormBackgroundTasksAfterSave(form, formBackgroundTask, userCookieData)
            );
        }
        #endregion /Background Tasks

        #region Referrals
        public List<Form> GetFormsFromReferrals(List<FormInstance> referrals)
        {
            List<Form> forms = new List<Form>();
            foreach (FormInstance referral in referrals)
            {
                Form form = GetFormById(referral.FormDefinitionId);
                form.SetFieldInstances(referral.FieldInstances);
                form.Id = referral.Id;
                form.UserId = referral.UserId;
                form.OverrideOrganizationId(referral.OrganizationId);
                forms.Add(form);
            }
            return forms;
        }
        private List<ReferralInfoDTO> GetReferrableFields(Form form, List<Form> referrals, string activeLanguage)
        {
            List<ReferralInfoDTO> result = new List<ReferralInfoDTO>();
            if (referrals != null)
            {
                Dictionary<int, Dictionary<int, string>> missingValuesDict = codeAssociationDAL.InitializeMissingValueList(activeLanguage);
                result = MapReferralInfo(form.GetValuesFromReferrals(referrals, missingValuesDict));
            }
            return result;
        }

        private List<ReferralInfoDTO> MapReferralInfo(List<ReferalInfo> referrals)
        {
            List<ReferralInfoDTO> result = new List<ReferralInfoDTO>();

            foreach (ReferalInfo referralInfo in referrals)
            {
                Personnel user = userDAL.GetById(referralInfo.UserId);
                Organization organization = organizationDAL.GetById(referralInfo.OrganizationId);

                ReferralInfoDTO referralInfoDTO = mapper.Map<ReferralInfoDTO>(referralInfo);
                referralInfoDTO.User = user != null ? new UserDataOut { FirstName = user.FirstName, LastName = user.LastName } : new UserDataOut();
                referralInfoDTO.Organization = organization != null ? new OrganizationDataOut { Name = organization.Name } : new OrganizationDataOut();
                result.Add(referralInfoDTO);
            }

            return result;
        }

        public FormInstance SetFormInstanceIdForUpdate(CTCAEPatient patient, FormInstance formInstance)
        {
            FormInstance formInstanceForUpdate = formInstanceDAL.GetById(patient.FormInstanceId);
            Form formForUpdate = formDAL.GetForm(formInstanceForUpdate.FormDefinitionId);
            formForUpdate.SetValuesFromReferrals(this.GetFormsFromReferrals(new List<FormInstance>() { formInstance, formInstanceForUpdate }));

            foreach (List<FieldSet> fieldSets in formForUpdate.GetAllListOfFieldSets())
            {
                foreach (FieldSet fieldSet in fieldSets)
                {
                    foreach (Field field in fieldSet.Fields)
                    {
                        field.FieldSetId = fieldSet.Id;
                    }
                }
            }

            SetFormInstanceForUpdateFields(formInstanceForUpdate, formForUpdate);

            return formInstanceForUpdate;
        }

        private void SetFormInstanceForUpdateFields(FormInstance formInstanceForUpdate, Form formForUpdate)
        {
            formInstanceForUpdate.FieldInstances = formForUpdate
                .GetAllFields()
                .Where(x => x.FieldInstanceValues != null)
                .Select(x => new FieldInstance(x, x.FieldSetId, x.FieldSetInstanceRepetitionId)
                {
                    FieldInstanceValues = x.FieldInstanceValues.ToList()
                }
            ).ToList();
        }
        #endregion /Referrals

        #region Query
        public void PopulateFieldQueries(FormDataOut data, IQueryManagementDAL queryManagementDAL, IMapper mapper)
        {
            var allFieldInstanceIds = data.GetAllFields()
                .SelectMany(f => f.FieldInstanceValues)
                .Select(fiv => fiv.FieldInstanceRepetitionId)
                .ToList();

            var allQueries = queryManagementDAL.GetByFieldIds(allFieldInstanceIds);
            var allQueriesMapped = allQueries.Select(q => mapper.Map<QueryDataOut>(q)).ToList();

            var queriesByFieldId = allQueriesMapped
                .GroupBy(q => q.FieldId)
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var field in data.GetAllFields())
            {
                foreach (var fiv in field.FieldInstanceValues)
                {
                    if (queriesByFieldId.TryGetValue(fiv.FieldInstanceRepetitionId, out var queries))
                    {
                        fiv.Queries = queries;
                    }
                    else
                    {
                        fiv.Queries = new List<QueryDataOut>();
                    }
                }
            }

        }
        #endregion /Query

    }
}
