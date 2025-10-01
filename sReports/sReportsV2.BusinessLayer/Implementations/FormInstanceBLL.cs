using AutoMapper;
using MongoDB.Bson;
using sReportsV2.BusinessLayer.Helpers.TabularExportGenerator;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Extensions;
using sReportsV2.Common.File;
using sReportsV2.Common.File.Implementations;
using sReportsV2.Common.File.Interfaces;
using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.Domain.Sql.Entities.User;
using sReportsV2.DTOs.DTOs.FormInstance.DataOut;
using sReportsV2.DTOs.Field.DataIn;
using sReportsV2.DTOs.Field.DataOut;
using sReportsV2.DTOs.FormInstance;
using sReportsV2.DTOs.FormInstance.DataIn;
using sReportsV2.DTOs.FormInstance.DataOut;
using sReportsV2.DTOs.Pagination;
using sReportsV2.DTOs.Patient.DataOut;
using sReportsV2.DTOs.User.DataOut;
using sReportsV2.DTOs.User.DTO;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using sReportsV2.DTOs.DTOs.FormInstance.DataIn;
using sReportsV2.BusinessLayer.Components.Interfaces;
using sReportsV2.Common.Exceptions;
using sReportsV2.DTOs.DTOs.Oomnia.DTO;
using sReportsV2.DTOs.DTOs.ProjectManagement.DataOut;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.BusinessLayer.Helpers;
using sReportsV2.Cache.Resources;
using sReportsV2.DTOs.Form.DataOut;
using sReportsV2.DTOs;
using sReportsV2.Cache.Singleton;
using Microsoft.Extensions.Configuration;
using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.DTOs.DTOs.FormInstanceChart.DataOut;
using sReportsV2.DTOs.DTOs.Autocomplete.DataOut;
using sReportsV2.Common.Entities.User;
using sReportsV2.DTOs.CTCAE.DataIn;
using sReportsV2.Domain.Sql.Entities.Patient;
using sReportsV2.DTOs.Encounter;
using sReportsV2.DTOs.EpisodeOfCare;
using sReportsV2.Domain.Sql.Entities.EpisodeOfCare;
using sReportsV2.Domain.Sql.Entities.Encounter;
using Chapters;
using sReportsV2.Domain.Sql.Entities.QueryManagement;

namespace sReportsV2.BusinessLayer.Implementations
{
    public partial class FormInstanceBLL : IFormInstanceBLL
    {
        private readonly IPersonnelDAL userDAL;
        private readonly IOrganizationDAL organizationDAL;
        private readonly IThesaurusDAL thesaurusDAL;
        private readonly IPatientDAL patientDAL;
        private readonly IFormInstanceDAL formInstanceDAL;
        private readonly IFormDAL formDAL;
        private readonly IFieldInstanceHistoryDAL fieldInstanceHistoryDAL;
        private readonly IEmailSender emailSender;
        private readonly IEncounterDAL encounterDAL;
        private readonly IEpisodeOfCareDAL episodeOfCareDAL;
        private readonly IPdfBLL pdfBLL;
        private readonly IFormCodeRelationDAL formCodeRelationDAL;
        private readonly IAsyncRunner asyncRunner;
        private readonly IProjectManagementDAL projectManagementDAL;
        private readonly ICodeAssociationDAL codeAssociationDAL;
        private readonly IQueryManagementDAL queryManagementDAL;
        private readonly ICodeDAL codeDAL;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;
        private readonly ISkosConnector skosConnector;
        private readonly SReportsContext dbContext;


        public FormInstanceBLL(IPersonnelDAL userDAL,
            IFormInstanceDAL formInstanceDAL,
            IPatientDAL patientDAL,
            IFormDAL formDAL,
            IFieldInstanceHistoryDAL fieldInstanceHistoryDAL,
            IEmailSender emailSender,
            IOrganizationDAL organizationDAL,
            IThesaurusDAL thesaurusDAL,
            IEncounterDAL encounterDAL,
            IPdfBLL pdfBLL,
            IFormCodeRelationDAL formCodeRelationDAL,
            IAsyncRunner asyncRunner,
            IProjectManagementDAL projectManagementDAL,
            ICodeAssociationDAL codeAssociationDAL,
            IEpisodeOfCareDAL episodeOfCareDAL,
            IQueryManagementDAL queryManagementDAL,
            ICodeDAL codeDAL,
            IMapper mapper, IConfiguration configuration, SReportsContext dbContext, ISkosConnector skosConnector)
        {
            this.userDAL = userDAL;
            this.patientDAL = patientDAL;
            this.formInstanceDAL = formInstanceDAL;
            this.formDAL = formDAL;
            this.fieldInstanceHistoryDAL = fieldInstanceHistoryDAL;
            this.emailSender = emailSender;
            this.organizationDAL = organizationDAL;
            this.thesaurusDAL = thesaurusDAL;
            this.encounterDAL = encounterDAL;
            this.episodeOfCareDAL = episodeOfCareDAL;
            this.pdfBLL = pdfBLL;
            this.formCodeRelationDAL = formCodeRelationDAL;
            this.asyncRunner = asyncRunner;
            this.projectManagementDAL = projectManagementDAL;
            this.codeAssociationDAL = codeAssociationDAL;
            this.queryManagementDAL = queryManagementDAL;
            this.codeDAL = codeDAL;
            this.mapper = mapper;
            this.configuration = configuration;
            this.dbContext = dbContext;
            this.skosConnector = skosConnector;
        }

        #region Basic actions

        public async Task<PaginationDataOut<FormInstanceTableDataOut, FormInstanceFilterDataIn>> ReloadData(FormInstanceFilterDataIn dataIn, UserCookieData userCookieData)
        {
            (FormInstanceFilterData filterData, List<Field> customHeaderFields) = PrepareFormInstanceFilter(dataIn, userCookieData);
            PaginationData<FormInstancePreview> filteredInstances = await formInstanceDAL.GetFormInstancesFilteredAsync(filterData);
            return new PaginationDataOut<FormInstanceTableDataOut, FormInstanceFilterDataIn>()
            {
                Count = filteredInstances.Count,
                Data = LoadFormInstancesDataOut(filteredInstances.Data, customHeaderFields, filterData),
                DataIn = dataIn
            };
        }

        public async Task<string> InsertOrUpdateAsync(FormInstance formInstance, FormInstanceStatus formInstanceStatus, UserCookieData userCookieData)
        {
            bool isUpdate = !string.IsNullOrEmpty(formInstance.Id);
            string formInstanceId = await formInstanceDAL.InsertOrUpdateAsync(formInstance, formInstanceStatus).ConfigureAwait(false);
            if (isUpdate)
            {
                await InsertOrUpdateManyFieldHistoriesAsync(formInstance).ConfigureAwait(false);
                await InsertQueriesAsync(formInstance).ConfigureAwait(false);
            }

            ExecuteAdditionalFormInstanceTriggersAfterSave(formInstance, userCookieData);

            return formInstanceId;
        }

        public void InsertListOfFormInstances(List<FormInstance> formInstances)
        {
            int skip = 0;
            int take = 50;
            while (skip < formInstances.Count)
            {
                formInstanceDAL.InsertMany(formInstances.Skip(skip).Take(take).ToList());
                skip += take;
            }
        }

        public FormInstance GetById(string id)
        {
            if (!string.IsNullOrWhiteSpace(id) && id != "placeholder" && id != "undefined")
                return formInstanceDAL.GetById(id);
            else
                return null;
        }

        public async Task<FormInstance> GetByIdAsync(string id)
        {
            if (!string.IsNullOrWhiteSpace(id) && id != "placeholder" && id != "undefined")
                return await formInstanceDAL.GetByIdAsync(id).ConfigureAwait(false);
            else
                return null;
        }

        public List<FormInstance> GetByIds(List<string> ids)
        {
            return formInstanceDAL.GetByIds(ids).ToList();
        }

        public async Task<List<FormInstance>> GetByIdsAsync(List<string> ids)
        {
            return await formInstanceDAL.GetByIdsAsync(ids).ConfigureAwait(false);
        }

        public void Delete(string formInstanceId, DateTime lastUpdate, int userId)
        {
            formInstanceDAL.Delete(formInstanceId, lastUpdate);
        }

        public int CountByDefinition(string id)
        {
            return formInstanceDAL.CountByDefinition(id);
        }

        public async Task DeleteAsync(string formInstanceId, DateTime lastUpdate, int userId)
        {
            await formInstanceDAL.DeleteAsync(formInstanceId, lastUpdate);
            await UpdateManyFieldHistoriesOnDeleteAsync(formInstanceId, userId);
        }

        public List<AutocompleteOptionDataOut> SearchByTitle(int episodeOfCareId, string title, UserCookieData userCookieData)
        {
            return formInstanceDAL.SearchByTitle(episodeOfCareId, title).Select(x => new AutocompleteOptionDataOut(x, userCookieData)).ToList();
        }

        public void LockUnlockFormInstance(FormInstanceLockUnlockRequest formInstanceSign, UserCookieData userCookieData)
        {
            formInstanceDAL.LockUnlockFormInstance(formInstanceSign);
            ExecuteAdditionalFormInstanceTriggersAfterLock(new LockActionToOomniaApiDTO { 
                IsLocked = formInstanceSign.IsLocked(),
                FormInstanceId  = formInstanceSign.FormInstanceId
            }
            ,userCookieData
            );
        }

        public IList<FormInstanceStatusDataOut> GetWorkflowHistory(List<FormInstanceStatus> formInstanceStatuses)
        {
            IList<FormInstanceStatusDataOut> workflowHistory = new List<FormInstanceStatusDataOut>();

            if (formInstanceStatuses != null)
            {
                List<int> createdByIds = formInstanceStatuses.Select(x => x.CreatedById).Distinct().ToList();
                Dictionary<int, Personnel> createdByUsers = userDAL.GetAllByIds(createdByIds).ToDictionary(u => u.PersonnelId, u => u);
                foreach (FormInstanceStatus formInstanceStatus in formInstanceStatuses.OrderByDescending(x => x.CreatedOn))
                {
                    createdByUsers.TryGetValue(formInstanceStatus.CreatedById, out Personnel createdBy);
                    if (createdBy != null)
                    {
                        workflowHistory.Add(new FormInstanceStatusDataOut()
                        {
                            CreatedById = createdBy.PersonnelId,
                            CreatedBy = new UserShortInfoDataOut(createdBy.FirstName, createdBy.LastName),
                            CreatedByActiveOrganization = createdBy.PersonnelConfig?.ActiveOrganization?.Name,
                            CreatedOn = formInstanceStatus.CreatedOn,
                            FormInstanceStatus = formInstanceStatus.Status,
                            IsSigned = formInstanceStatus.IsSigned
                        });
                    }
                }
            }

            return workflowHistory;
        }

        public FormInstanceMetadataDataOut GetFormInstanceKeyDataFirst(int createdById)
        {
            return mapper.Map<FormInstanceMetadataDataOut>(formInstanceDAL.GetFormInstanceKeyDataFirst(createdById));
        }

        public void LockUnlockChapterOrPageOrFieldSet(FormInstancePartialLock formInstancePartialLock, UserCookieData userCookieData)
        {
            formInstanceDAL.LockUnlockChapterOrPageOrFieldSet(formInstancePartialLock);
            ExecuteAdditionalFormInstanceTriggersAfterLock(new LockActionToOomniaApiDTO
                {
                    IsLocked = formInstancePartialLock.IsLockAction(),
                    FormInstanceId = formInstancePartialLock.FormInstanceId,
                    ChapterId = formInstancePartialLock.ChapterId,
                    PageId = formInstancePartialLock.PageId,
                    FieldSetInstanceRepetitionId = formInstancePartialLock.FieldSetInstanceRepetitionId
                }
                , userCookieData
            );
        }

        private List<FormInstanceTableDataOut> LoadFormInstancesDataOut(List<FormInstancePreview> formInstancesFromDb, List<Field> customHeaderFields, FormInstanceFilterData filterData)
        {
            var formInstances = formInstancesFromDb.Select(formInstance =>
            {
                var fields = formInstance.FieldInstancesToDisplay
                    .Select(formInstancePreview =>
                    {
                        var field = customHeaderFields.Find(f => f.Id == formInstancePreview.FieldId);
                        field.FieldInstanceValues = formInstancePreview.FieldInstanceValues;
                        return field;
                    }).ToList();

                formInstance.FieldsToDisplay = fields;
                return mapper.Map<FormInstanceTableDataOut>(formInstance);
            }).ToList();

            return PopulateUsersAndPatients(formInstances, filterData);
        }

        private List<FormInstanceTableDataOut> PopulateUsersAndPatients(List<FormInstanceTableDataOut> formInstances, FormInstanceFilterData filterData)
        {
            List<int> userIds = formInstances.Select(x => x.UserId).Distinct().ToList();
            Dictionary<int, UserShortInfoDataOut> users = userDAL.GetAllByIds(userIds)
                .ToDictionary(
                    u => u.PersonnelId,
                    u => new UserShortInfoDataOut(u.FirstName, u.LastName)
                );

            List<int> patientIds = formInstances.Select(x => x.PatientId).ToList();
            List<PatientTableDataOut> patients = mapper.Map<List<PatientTableDataOut>>(patientDAL.GetAllByIds(patientIds));

            List<int?> projectIds = formInstances.Select(x => x.ProjectId).ToList();
            List<ProjectTableDataOut> projects = mapper.Map< List<ProjectTableDataOut>>(projectManagementDAL.GetAllByIds(projectIds));

            var orderedFormInstances = GetOrderedFormInstances(formInstances, filterData, users, patients, projects);

            return orderedFormInstances
                .Select(formInstance =>
                {
                    UserShortInfoDataOut user;
                    if (users.TryGetValue(formInstance.UserId, out user))
                        formInstance.User = user;
                    formInstance.Patient = patients.Find(x => x.Id == formInstance.PatientId);
                    formInstance.Project = projects.Find(x => x.ProjectId == formInstance.ProjectId);
                    formInstance.SpecialValues = filterData.SpecialValues;
                    return formInstance;
                })
                .ToList();
        }

        private IEnumerable<FormInstanceTableDataOut> GetOrderedFormInstances(List<FormInstanceTableDataOut> formInstances, FormInstanceFilterData filterData, Dictionary<int, UserShortInfoDataOut> users, List<PatientTableDataOut> patients, List<ProjectTableDataOut> projects)
        {
            if (filterData.ColumnName != null)
            {
                switch (filterData.ColumnName)
                {
                    case AttributeNames.User:
                        return OrderFormInstancesByUser(formInstances, filterData, users)
                            .Skip(filterData.GetHowManyElementsToSkip())
                            .Take(filterData.PageSize);
                    case AttributeNames.Patient:
                        return OrderFormInstancesByPatient(formInstances, filterData, patients)
                            .Skip(filterData.GetHowManyElementsToSkip())
                            .Take(filterData.PageSize);
                    case AttributeNames.ProjectName:
                        return OrderFormInstancesByProject(formInstances, filterData, projects)
                            .Skip(filterData.GetHowManyElementsToSkip())
                            .Take(filterData.PageSize);
                    default:
                        return formInstances;
                }
            }
            return formInstances;
        }

        private IEnumerable<FormInstanceTableDataOut> OrderFormInstancesByUser(List<FormInstanceTableDataOut> formInstances, FormInstanceFilterData filterData, Dictionary<int, UserShortInfoDataOut> users)
        {
            return filterData.IsAscending ?
                formInstances.OrderBy(fi => users.ContainsKey(fi.UserId) ? users[fi.UserId].FirstName : "")
                             .ThenBy(fi => users.ContainsKey(fi.UserId) ? users[fi.UserId].LastName : "") :
                formInstances.OrderByDescending(fi => users.ContainsKey(fi.UserId) ? users[fi.UserId].FirstName : "")
                             .ThenByDescending(fi => users.ContainsKey(fi.UserId) ? users[fi.UserId].LastName : "");
        }

        private IEnumerable<FormInstanceTableDataOut> OrderFormInstancesByPatient(List<FormInstanceTableDataOut> formInstances, FormInstanceFilterData filterData, List<PatientTableDataOut> patients)
        {
            Func<string, string> replaceEmptyFirstName = firstName => string.IsNullOrWhiteSpace(firstName) ? "Unknown" : firstName;

            return filterData.IsAscending ?
                formInstances.OrderBy(fi => replaceEmptyFirstName(patients.Find(x => x.Id == fi.PatientId)?.FirstName))
                             .ThenBy(fi => patients.Find(x => x.Id == fi.PatientId)?.LastName) :
                formInstances.OrderByDescending(fi => replaceEmptyFirstName(patients.Find(x => x.Id == fi.PatientId)?.FirstName))
                             .ThenByDescending(fi => patients.Find(x => x.Id == fi.PatientId)?.LastName);
        }

        private IEnumerable<FormInstanceTableDataOut> OrderFormInstancesByProject(List<FormInstanceTableDataOut> formInstances, FormInstanceFilterData filterData, List<ProjectTableDataOut> projects)
        {
            return filterData.IsAscending ?
                formInstances.OrderBy(fi => projects.Find(x => x.ProjectId == fi.ProjectId)?.ProjectName) :
                formInstances.OrderByDescending(fi => projects.Find(x => x.ProjectId == fi.ProjectId)?.ProjectName);
        }

        private List<Field> GetCustomHeaderFields(string formId)
        {
            return formDAL.GetForm(formId)?.GetFieldsByCustomHeader();
        }

        private (FormInstanceFilterData, List<Field>) PrepareFormInstanceFilter(FormInstanceFilterDataIn dataIn, UserCookieData userCookieData)
        {
            List<Field> customHeaderFields = GetCustomHeaderFields(dataIn.FormId);
            dataIn.CustomHeaderFields = mapper.Map<List<FieldDataIn>>(customHeaderFields);
            FormInstanceFilterData filterData = mapper.Map<FormInstanceFilterData>(dataIn);
            filterData.Languages = SingletonDataContainer.Instance.GetLanguages().ToDictionary(x => x.Value, x => x.Label);
            var nullFlavors = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.NullFlavor);
            filterData.SpecialValues = nullFlavors.ToDictionary(x => x.Id.ToString(), x => x.Thesaurus.GetPreferredTermByTranslationOrDefault(userCookieData.ActiveLanguage));
            filterData.PersonnelProjectsIds = projectManagementDAL.GetAllProjectsIdsFor(userCookieData.Id);

            return (filterData, customHeaderFields);
        }

        private async Task InsertQueriesAsync(FormInstance formInstance)
        {
            foreach (var field in formInstance.FieldInstances)
            {
                foreach (var fieldValue in field.FieldInstanceValues) 
                {
                    if (fieldValue.ValidationError != null)
                    {
                        var query = new Query
                        {
                            FieldId = fieldValue.ValidationError.FieldId,
                            FormInstanceId = formInstance.Id,
                            Title = fieldValue.ValidationError.Title,
                            Description = fieldValue.ValidationError.Description,
                            ReasonCD = codeDAL.GetByCodeSetIdAndPreferredTerm((int)CodeSetList.QueryReason, CodeAttributeNames.DataValidation),
                            StatusCD = codeDAL.GetByCodeSetIdAndPreferredTerm((int)CodeSetList.QueryStatus, CodeAttributeNames.New)
                        };

                        await queryManagementDAL.Create(query).ConfigureAwait(false);
                    }
                }
            }
        }

        #endregion /Basic actions

        #region Set data for FormInstance from input

        public FormInstance GetFormInstanceSet(Form form, FormInstanceDataIn formInstanceDataIn, UserCookieData userCookieData, bool setFieldsFromRequest = true)
        {
            FormInstance formInstanceFromDatabase = formInstanceDAL.GetById(formInstanceDataIn?.FormInstanceId);
            string notes = formInstanceDataIn?.Notes ?? string.Empty;
            string date = formInstanceDataIn?.Date ?? string.Empty;
            string formState = formInstanceDataIn?.FormState;

            FormInstance parsedFormInstanceFromInput = new FormInstance(form)
            {
                UserId = (userCookieData?.Id).GetValueOrDefault(),
                OrganizationId = (userCookieData?.ActiveOrganization).GetValueOrDefault(),
                PatientId = formInstanceFromDatabase != null ? formInstanceFromDatabase.PatientId : formInstanceDataIn != null ? formInstanceDataIn.PatientId : 0,
                EpisodeOfCareRef = formInstanceFromDatabase != null ? formInstanceFromDatabase.EpisodeOfCareRef : formInstanceDataIn != null ? formInstanceDataIn.EpisodeOfCareId : 0,
                EncounterRef = formInstanceFromDatabase != null ? formInstanceFromDatabase.EncounterRef : formInstanceDataIn != null ? formInstanceDataIn.EncounterId : 0,
                Notes = notes,
                Date = string.IsNullOrWhiteSpace(date) ? DateTime.Now : DateTime.ParseExact(date, DateTimeConstants.DateFormat, CultureInfo.InvariantCulture).ToLocalTime(),
                FormState = string.IsNullOrWhiteSpace(formState) ? FormState.OnGoing : (FormState)Enum.Parse(typeof(FormState), formState),
                Id = formInstanceDataIn?.FormInstanceId,
                Referrals = formInstanceDataIn?.Referrals ?? new List<string>(),
                ProjectId = formInstanceFromDatabase != null ? formInstanceFromDatabase.ProjectId : formInstanceDataIn?.ProjectId
            };
            parsedFormInstanceFromInput.ParseOrAddLastUpdate(formInstanceDataIn?.LastUpdate);

            if (setFieldsFromRequest)
            {
                parsedFormInstanceFromInput.FieldInstances = ParseFormInstanceFields(formInstanceDataIn.FieldInstances);
            }

            return parsedFormInstanceFromInput;
        }

        public List<FieldInstance> ParseFormInstanceFields(List<FieldInstanceDTO> fieldInstances)
        {
            List<FieldInstance> fieldValues = new List<FieldInstance>();

            foreach (var groupedByFieldSet in fieldInstances.GroupBy(f => new {f.FieldSetId, f.FieldSetInstanceRepetitionId}))
            {
                string fieldSetInstanceRepetitionId = GetFieldSetInstanceRepetitionIdOrCreateNewIfEmpty(groupedByFieldSet.FirstOrDefault()?.FieldSetInstanceRepetitionId);

                foreach (var groupedByField in groupedByFieldSet.GroupBy(x => x.FieldId))
                {
                    List<FieldInstanceValue> fieldInstanceValues = new List<FieldInstanceValue>();
                    foreach (var fieldDataIn in groupedByField)
                    {
                        fieldInstanceValues.Add(new FieldInstanceValue(fieldDataIn.GetCleanedValue(), fieldDataIn.FlatValueLabel, fieldDataIn.FieldInstanceRepetitionId, fieldDataIn.IsSpecialValue, fieldDataIn.ConnectedFieldInstanceRepetitionId, fieldDataIn.ValidationError));
                    }

                    fieldValues.Add(new FieldInstance()
                    {
                        ThesaurusId = groupedByField.FirstOrDefault().ThesaurusId,
                        FieldInstanceValues = fieldInstanceValues,
                        Type = groupedByField.FirstOrDefault()?.Type,
                        FieldId = groupedByField.Key,
                        FieldSetId = groupedByFieldSet.Key.FieldSetId,
                        FieldSetInstanceRepetitionId = fieldSetInstanceRepetitionId,
                    });
                }
            }
            return fieldValues;
        }

        private string GetFieldSetInstanceRepetitionIdOrCreateNewIfEmpty(string existingFieldSetInstanceRepetitionId)
        {
            return string.IsNullOrEmpty(existingFieldSetInstanceRepetitionId) ? GuidExtension.NewGuidStringWithoutDashes() : existingFieldSetInstanceRepetitionId;
        }

        #endregion /Set data for FormInstance from input

        #region Export

        public void SendExportedFiles(List<FormInstanceDownloadData> formInstancesForDownload, UserCookieData userCookieData, string tableFormat, string fileFormat, bool callAsyncRunner = false)
        {
            try
            {
                if (callAsyncRunner)
                {
                    asyncRunner.Run<IFormInstanceBLL>((standaloneFormInstanceBLL) =>
                        standaloneFormInstanceBLL.SendExportedFiles(formInstancesForDownload, userCookieData, tableFormat, fileFormat)
                    );
                }
                else
                {
                    List<Form> forms = formDAL.GetByFormIdsList(formInstancesForDownload.Select(x => x.FormId).ToList());
                    KeyValuePair<int, string> organization = new KeyValuePair<int, string>(userCookieData.ActiveOrganization, organizationDAL.GetById(userCookieData.ActiveOrganization)?.Name ?? string.Empty);
                    Dictionary<string, Stream> files = GetFilesForDownload(
                        forms,
                        organization,
                        userCookieData,
                        DateTimeConstants.DateFormat,
                        tableFormat,
                        fileFormat);
                    string emailContent = EmailHelpers.GetExportEmailContent(userCookieData, files.Keys);
                    emailSender.SendAsync(new EmailDTO(userCookieData.Email, emailContent, $"Export Ready for Download: {EmailSenderNames.SoftwareName} Data Capture")
                    {
                        UserTimezone = userCookieData.TimeZoneOffset,
                        Attachments = files,
                        IsCsv = fileFormat == FormInstanceConstants.CsvFormat
                    });
                }
            }
            catch (Exception e)
            {
                throw new TabularExportException(e.Message, e);
            }
        }

        private Dictionary<string, Stream> GetFilesForDownload(List<Form> formsForDownload, KeyValuePair<int, string> organization, UserCookieData userCookieData, string dateFormat, string tableFormat, string fileFormat = FormInstanceConstants.CsvFormat)
        {
            Dictionary<string, Stream> files = new Dictionary<string, Stream>();

            foreach (Form form in formsForDownload)
            {
                if (fileFormat == FormInstanceConstants.CsvFormat)
                {
                    files.Add(form.Title, CreateCsvTableStream(form, organization, userCookieData, dateFormat, tableFormat));
                }
                else if (fileFormat == FormInstanceConstants.XlsxFormat)
                {
                    files.Add(form.Title, CreateXlsxTableStream(form, organization, userCookieData, dateFormat, tableFormat));
                }
            }
            return files;
        }

        private Stream CreateCsvTableStream(Form form, KeyValuePair<int, string> organization, UserCookieData userCookieData, string dateFormat, string tableFormat = FormInstanceConstants.LongFormat)
        {
            Stream stream = new MemoryStream();
            CsvWriter csvWriter = new CsvWriter(stream);

            switch (tableFormat)
            {
                case FormInstanceConstants.WideFormat:
                    CreateWideTableStream(csvWriter, form, organization, userCookieData, dateFormat);
                    break;
                default:
                    CreateLongTableStream(csvWriter, form, organization, userCookieData, dateFormat);
                    break;
            }

            csvWriter.FinalizeWriting(stream);
            return stream;
        }

        private Stream CreateXlsxTableStream(Form form, KeyValuePair<int, string> organization, UserCookieData userCookieData, string dateFormat, string tableFormat = FormInstanceConstants.LongFormat)
        {
            Stream stream = new MemoryStream();
            ExcelWriter excelWriter = new ExcelWriter(stream);

            switch (tableFormat)
            { 
                case FormInstanceConstants.WideFormat:
                    CreateWideTableStream(excelWriter, form, organization, userCookieData, dateFormat);
                    break;
                default:
                    CreateLongTableStream(excelWriter, form, organization, userCookieData, dateFormat);
                    break;
            }

            excelWriter.FinalizeWriting(stream);
            return stream;
        }

        private void CreateLongTableStream(FileWriter fileWriter, Form form, KeyValuePair<int, string> organization, UserCookieData userCookieData, string dateFormat)
        {
            var missingValuesDict = codeAssociationDAL.InitializeMissingValueList(userCookieData.ActiveLanguage);

            TabularExportGeneratorInputParams inputParams = new TabularExportGeneratorInputParams(fileWriter, form, userCookieData, dateFormat, organization, missingValuesDict);
            LongTableExportGenerator tableExportGenerator = new LongTableExportGenerator(formInstanceDAL, patientDAL, inputParams);
            
            tableExportGenerator.CreateTabularExport(); 
        }

        private void CreateWideTableStream(FileWriter fileWriter, Form form, KeyValuePair<int, string> organization, UserCookieData userCookieData, string dateFormat)
        {
            var missingValuesDict = codeAssociationDAL.InitializeMissingValueList(userCookieData.ActiveLanguage);

            TabularExportGeneratorInputParams inputParams = new TabularExportGeneratorInputParams(fileWriter, form, userCookieData, dateFormat, organization, missingValuesDict);
            TabularExportGenerator tableExportGenerator;

            (bool moreThanOneRepetitiveElement, object singleRepetitiveElement) = HasFormMoreThanOneRepetitiveElement(form);

            if (moreThanOneRepetitiveElement)
            {
                tableExportGenerator = new WideTableExporter(formInstanceDAL, patientDAL, inputParams);
            }
            else
            {
                inputParams.RepetitiveElement = singleRepetitiveElement;

                if (singleRepetitiveElement is Field)
                    tableExportGenerator = new WideTableRepetitiveFieldExporter(formInstanceDAL, patientDAL, inputParams);
                else if (singleRepetitiveElement is FieldSet)
                    tableExportGenerator = new WideTableRepetitiveFieldSetExporter(formInstanceDAL, patientDAL, inputParams);
                else
                    tableExportGenerator = new WideTableExporter(formInstanceDAL, patientDAL, inputParams);
            }
            tableExportGenerator.CreateTabularExport();
        }

        /// <summary>
        ///     If Repetitive Elements == 0 returns (false, null). Else If Repetitive Elements > 1 returns (true, null). Else If Repetitive Elements == 1 returns (false, RepetitiveElement)
        /// </summary>
        private Tuple<bool, object> HasFormMoreThanOneRepetitiveElement(Form form)
        {
            object repetitiveElement = null;
            int repetitiveElementsCount = 0;
            foreach (FormChapter chapter in form.Chapters)
            {
                foreach (FormPage page in chapter.Pages)
                {
                    foreach (var listOfFieldSets in page.ListOfFieldSets)
                    {
                        foreach (FieldSet fieldSet in listOfFieldSets)
                        {
                            if (fieldSet.IsRepetitive)
                            {
                                repetitiveElementsCount++;
                                if (repetitiveElementsCount > 1)
                                    return Tuple.Create(true, (object)null);
                                if (repetitiveElementsCount == 1)
                                    repetitiveElement = fieldSet;
                            }

                            var repetitiveFields = fieldSet.Fields.Where(x => x is FieldString && (x as FieldString).IsRepetitive);
                            if(repetitiveFields.Any())
                            {
                                repetitiveElementsCount += repetitiveFields.Count();
                                if (repetitiveElementsCount > 1)
                                    return Tuple.Create(true, (object)null);
                                if (repetitiveElementsCount == 1)
                                    repetitiveElement = repetitiveFields.FirstOrDefault();

                            }
                        }
                    }
                }
            }
            return Tuple.Create(false, repetitiveElement);
        }

        public void WriteFieldsAndMetadataToStream(FormInstance formInstance, TextWriter tw, string language, string dateFormat)
        {
            WriteFieldsToStream(formInstance, tw, language);
            WriteNotesToStream(formInstance, tw);
            WriteDateToStream(formInstance, tw, dateFormat);
            WriteFormStateToStream(formInstance, tw);
        }

        private void WriteFieldsToStream(FormInstance formInstance, TextWriter tw, string language)
        {
            List<FieldDataOut> fields = GetMappedFields(formInstance);
            var missingValues = codeAssociationDAL.InitializeMissingValueList(language);

            foreach (FieldDataOut formField in fields)
            {
                string textExportValue = formField.GetValueForTextExport(missingValues);
                tw.WriteLine($"{formField.Label}");
                tw.WriteLine($"{textExportValue}");
                tw.WriteLine();
            }
        }

        private List<FieldDataOut> GetMappedFields(FormInstance formInstance)
        {
            Form form = formDAL.GetForm(formInstance.FormDefinitionId);
            form.SetFieldInstances(formInstance.FieldInstances);
            return mapper.Map<List<FieldDataOut>>(form.GetAllFields());
        }

        private void WriteNotesToStream(FormInstance formInstance, TextWriter tw)
        {
            tw.WriteLine("Notes:");
            tw.WriteLine($"{formInstance.Notes}");
            tw.WriteLine();
        }

        private void WriteDateToStream(FormInstance formInstance, TextWriter tw, string dateFormat)
        {
            tw.WriteLine("Date:");
            tw.WriteLine(formInstance.Date.GetDateTimeDisplay(dateFormat, excludeTimePart: true));
            tw.WriteLine();
        }

        private void WriteFormStateToStream(FormInstance formInstance, TextWriter tw)
        {
            tw.WriteLine("Form state:");
            tw.WriteLine(TextLanguage.ResourceManager.GetString(formInstance.FormState.GetValueOrDefault(FormState.OnGoing).ToString()));
            tw.WriteLine();
        }

        #endregion /Export

        #region Plot

        public FormInstanceChartDataOut GetPlottableFieldsByThesaurusId(FormInstancePlotDataIn dataIn, List<FieldDataOut> fieldsDataOut)
        {
            FormInstanceChartDataOut chartUtilityDataStructure = new FormInstanceChartDataOut();
            foreach (int fieldThesaurusId in dataIn.FieldThesaurusIds)
            {
                List<BsonDocument> bsonDocuments = formInstanceDAL.GetPlottableFieldsByThesaurusId(dataIn.FormDefinitionId, dataIn.OrganizationId, fieldThesaurusId);
                foreach (BsonDocument bsonDocument in bsonDocuments)
                {
                    BsonValue dateTimeToPlot = GetBsonValueHelper(bsonDocument, "Date") ?? GetBsonValueHelper(bsonDocument, "EntryDateTimeValue");
                    if (dateTimeToPlot.ToUniversalTime() >= dataIn.DateTimeFrom.GetValueOrDefault().ToUniversalTime() &&
                         dateTimeToPlot.ToUniversalTime() <= dataIn.DateTimeTo.GetValueOrDefault().ToUniversalTime())
                    {
                        BsonValue selectedOptionsIdsArray = GetBsonValueHelper(bsonDocument, "FieldInstanceValue");
                        string selectedOptionId = null;

                        if (selectedOptionsIdsArray is BsonArray && selectedOptionsIdsArray.AsBsonArray.Count > 0)
                            selectedOptionId = selectedOptionsIdsArray.AsBsonArray[0].ToString();

                        var field = fieldsDataOut.Find(f => f.ThesaurusId == fieldThesaurusId);
                        FormFieldValueDataOut selectedOption = (field as FieldSelectableDataOut).Values.Find(y => y.Id == selectedOptionId);

                        chartUtilityDataStructure.AddToKeyIfExists(
                            field.Label,
                            selectedOption?.NumericValue,
                            (long)(dateTimeToPlot.ToUniversalTime().Subtract(DateTime.UnixEpoch)).TotalMilliseconds);
                    }

                }
            }
            return chartUtilityDataStructure;
        }

        private BsonValue GetBsonValueHelper(BsonDocument bson, string value)
        {
            bson.TryGetValue(value, out BsonValue bsonValue);
            return !(bsonValue is BsonNull) ? bsonValue : null;
        }

        #endregion /Plot

        #region PatientRelatedData
        public void SetFormInstanceAdditionalData(Form form, FormInstance formInstance, UserCookieData userCookieData)
        {
            if (!form.DisablePatientData)
            {
                UserData userData = mapper.Map<UserData>(userCookieData);
                int patientId = ParseAndInsertPatient(form, userCookieData);
                int eocId = InsertEpisodeOfCare(patientId, form.EpisodeOfCare, "Simulator", DateTimeExtension.GetCurrentDateTime(userCookieData.OrganizationTimeZoneIana), userData);
                formInstance.PatientId = patientId;
                formInstance.EpisodeOfCareRef = eocId;
                formInstance.EncounterRef = InsertEncounter(eocId);
            }

            formInstance.UserId = userCookieData.Id;
            formInstance.OrganizationId = userCookieData.ActiveOrganization;
            formInstance.Copy(null, new FormInstanceStatus(FormState.OnGoing, formInstance.UserId, isSigned: false));
            formInstance.InitOrUpdateChapterPageFieldSetWorkflowHistory(form, formInstance.UserId);
        }

        public void SetCTCAEPatient(Form form, FormInstance formInstance, CTCAEPatient patient, UserCookieData userCookieData)
        {
            if (!form.DisablePatientData)
            {
                UserData userData = mapper.Map<UserData>(userCookieData);
                int patientId = 0;
                Patient patientEntity = patientDAL.GetById(patient.PatientId);
                if (patientEntity == null)
                {
                    patientEntity = new Patient("Unknown", "Unknown");
                    patient.PatientId = 0;
                    patientEntity.PatientId = patient.PatientId;
                    patientEntity.OrganizationId = userData.ActiveOrganization.GetValueOrDefault();
                    patientDAL.InsertOrUpdate(patientEntity, null);
                }

                formInstance.PatientId = patientEntity.PatientId;
                int eocId = InsertEpisodeOfCare(patientId, form.EpisodeOfCare, "Engine", DateTimeExtension.GetCurrentDateTime(userCookieData.OrganizationTimeZoneIana), userData);
                int encounterId = InsertEncounter(eocId);
                formInstance.EpisodeOfCareRef = eocId;
                formInstance.EncounterRef = encounterId;
            }
        }

        public int GetEncounterFromRequestOrCreateDefault(int episodeOfCareId, int encounterId)
        {
            if (encounterId == 0)
            {
                encounterId = InsertEncounter(episodeOfCareId);
            }

            return encounterId;
        }

        protected int InsertPatient(Patient patient, UserCookieData userCookieData)
        {
            //TO DO FIX THIS FUNCTION
            Patient patientDb = patient == null || patient.PatientIdentifiers == null || patient.PatientIdentifiers.Count <= 0 ?
                patient
                :
                patientDAL.GetByIdentifier(patient.PatientIdentifiers[0]);

            if (patientDb?.PatientId == 0)
            {
                patientDb.CreatedById = userCookieData.Id;
                patientDAL.InsertOrUpdate(patientDb, null);
            }

            return patientDb != null ? patientDb.PatientId : 0;
        }

        private int ParseAndInsertPatient(Form form, UserCookieData userCookieData)
        {
            PatientParser patientParser = new PatientParser();
            Patient patient = patientParser.ParsePatientChapter(form.Chapters.Find(x => x.ThesaurusId.ToString().Equals(ResourceTypes.PatientThesaurus)));
            patient.OrganizationId = GetOrganizationId(form, userCookieData);

            return InsertPatient(patient, userCookieData);
        }

        private int GetOrganizationId(Form form, UserCookieData userCookieData)
        {
            return form.GetActiveOrganizationId(userCookieData.ActiveOrganization);
        }

        private int InsertEpisodeOfCare(int patientId, FormEpisodeOfCare episodeOfCare, string source, DateTime startDate, UserData user)
        {
            startDate = startDate.Date;
            EpisodeOfCare eoc;
            if (episodeOfCare != null)
            {
                eoc = mapper.Map<EpisodeOfCare>(episodeOfCare);
                eoc.Period = new Domain.Sql.Entities.Common.PeriodDatetime() { Start = startDate };
                eoc.Description = $"Generated from {source}";
                eoc.PatientId = patientId;
                eoc.OrganizationId = 1;
            }
            else
            {
                eoc = mapper.Map<EpisodeOfCare>(new EpisodeOfCareDataIn()
                {
                    Description = $"Generated from {source}",
                    PatientId = patientId,
                    StatusCD = (int)EocStatus.Active,
                    Period = new PeriodDTO() { StartDate = startDate }
                }
                );
                eoc.OrganizationId = 1;
            }

            return episodeOfCareDAL.InsertOrUpdate(eoc, user);

        }

        private int InsertEncounter(int episodeOfCareId)
        {
            Encounter encounterEntity = mapper.Map<Encounter>(new EncounterDataIn()
            {
                ClassCD = 12246,
                Period = new PeriodOffsetDTO
                {
                    StartDate = DateTimeOffset.Now,
                    EndDate = DateTimeOffset.Now
                },
                StatusCD = 12218,
                TypeCD = 12208,
                ServiceTypeCD = 11087
            }
            );
            encounterEntity.EpisodeOfCareId = episodeOfCareId;
            return encounterDAL.InsertOrUpdate(encounterEntity);
        }

        #endregion
    }
}