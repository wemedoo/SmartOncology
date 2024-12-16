using AutoMapper;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Common.Enums;
using sReportsV2.Domain.Extensions;
using sReportsV2.DTOs.Field.DataOut;
using sReportsV2.DTOs.CTCAE.DataIn;
using sReportsV2.DTOs.Form;
using sReportsV2.DTOs.Form.DataOut;
using sReportsV2.DTOs.FormInstance;
using sReportsV2.DTOs.FormInstance.DataIn;
using sReportsV2.DTOs.FormInstance.DataOut;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.DTOs;
using sReportsV2.Common.Extensions;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Entities.User;
using sReportsV2.SqlDomain.Interfaces;
using sReportsV2.Domain.Sql.Entities.Patient;
using sReportsV2.Common.Constants;
using sReportsV2.DTOs.DTOs.FormInstance.DataIn;
using sReportsV2.Common.Helpers;
using System.Data;
using sReportsV2.DTOs.DTOs.Form.DataOut;
using sReportsV2.DTOs.DTOs.FormInstance.DataOut;
using sReportsV2.Common.JsonModelBinder;
using sReportsV2.Cache.Resources;
using sReportsV2.Cache.Singleton;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.DTOs.DTOs.FieldInstance.DataIn;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace sReportsV2.Controllers
{
    //[Authorize]
    public class FormInstanceController : FormCommonController
    {
        private readonly IMapper Mapper;
        #region Before Hackaton
        public FormInstanceController(IPatientDAL patientDAL, 
            IEpisodeOfCareDAL episodeOfCareDAL, 
            IEncounterDAL encounterDAL, 
            IUserBLL userBLL, 
            IOrganizationBLL organizationBLL, 
            ICodeBLL codeBLL, 
            IFormInstanceBLL formInstanceBLL, 
            IFormBLL formBLL, 
            IThesaurusDAL thesaurusDAL, 
            IAsyncRunner asyncRunner, 
            IPdfBLL pdfBLL,
            IMapper mapper,            
            IHttpContextAccessor httpContextAccessor, 
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            ICodeAssociationBLL codeAssociationBLL,
            IProjectManagementBLL projectManagementBLL) : 
            base(patientDAL, episodeOfCareDAL, encounterDAL, userBLL, organizationBLL, codeBLL, formInstanceBLL, formBLL, thesaurusDAL, asyncRunner, pdfBLL, mapper, httpContextAccessor, serviceProvider, configuration, codeAssociationBLL, projectManagementBLL)
        {
            Mapper = mapper;
        }

        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Engine)]
        //[Authorize]
        public ActionResult GetAllByFormThesaurus(FormInstanceFilterDataIn dataIn)
        {
            return GetAllFormInstances(dataIn);
        }

        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Engine)]
        public ActionResult GetAllByProject(FormInstanceFilterDataIn dataIn)
        {
            return GetAllFormInstances(dataIn);
        }

        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Engine)]
        public ActionResult GetAllByUserProject(FormInstanceFilterDataIn dataIn)
        {
            return GetAllFormInstances(dataIn, true);
        }

        [SReportsAuthorize]
        public async Task<ActionResult> ReloadByFormThesaurusTable(FormInstanceFilterDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));

            if (!this.formDAL.ExistsFormByThesaurus(dataIn.ThesaurusId))
            {
                return NotFound(TextLanguage.FormForThesaurusIdNotExists, dataIn.ThesaurusId.ToString());
            }

            if (ViewBag.IsDateCaptureMode)
            {
                dataIn.UserIds.Add(userCookieData.Id);
            }

            ViewBag.FilterFormInstanceDataIn = dataIn;
            ViewBag.DocumentPropertiesEnums = Mapper.Map<Dictionary<string, List<EnumDTO>>>(this.codeBLL.GetDocumentPropertiesEnums());
            ViewBag.FormInstanceTitle = dataIn.Title;
            var customHeaders = Mapper.Map<List<CustomHeaderFieldDataOut>>((await formDAL.GetFormAsync(dataIn.FormId).ConfigureAwait(false))?.CustomHeaderFields);
            ViewBag.CustomHeaders = customHeaders != null && customHeaders.Count > 0 ? customHeaders : CustomHeaderFieldDataOut.GetDefaultHeaders();
            SetProjectViewBags(dataIn, dataIn.ShowUserProjects);

            return PartialView("FormInstancesByFormThesaurusTable", await formInstanceBLL.ReloadData(dataIn, ViewBag.Languages as List<EnumDTO>, userCookieData).ConfigureAwait(false));
        }

        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Engine)]
        public ActionResult GetAllFormDefinitions(FormFilterDataIn dataIn)
        {
            ViewBag.DocumentPropertiesEnums = Mapper.Map<Dictionary<string, List<EnumDTO>>>(this.codeBLL.GetDocumentPropertiesEnums());
            ViewBag.ClinicalDomains = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.ClinicalDomain);

            ViewBag.FilterData = dataIn;
            return View();
        }

        public ActionResult GetDocumentsPerDomain()
        {
            List<FormInstancePerDomainDataOut> result = formBLL.GetFormInstancePerDomain(userCookieData.ActiveLanguage);

            return Json(result);
        }

        [SReportsAuditLog]
        [HttpPost]
        [SReportsAuthorize(Permission = PermissionNames.Create, Module = ModuleNames.Engine)]
        public async Task<ActionResult> Create([ModelBinder(typeof(JsonNetModelBinder))] FormInstanceDataIn formInstanceDataIn)
        {
            return await CreateOrEdit(formInstanceDataIn).ConfigureAwait(false);
        }

        [SReportsAuditLog]
        [HttpPost]
        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.Engine)]
        public async Task<ActionResult> Edit([ModelBinder(typeof(JsonNetModelBinder))] FormInstanceDataIn formInstanceDataIn)
        {
            return await CreateOrEdit(formInstanceDataIn).ConfigureAwait(false);
        }

        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.Create, Module = ModuleNames.Engine)]
        public ActionResult Create(FormInstanceFilterDataIn filter)
        {
            return GetFormInstances(filter);
        }

        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.Create, Module = ModuleNames.Engine)]
        public ActionResult CreateForProject(FormInstanceFilterDataIn filter)
        {
            return GetFormInstances(filter);
        }

        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.Create, Module = ModuleNames.Engine)]
        public ActionResult CreateForUserProject(FormInstanceFilterDataIn filter)
        {
            return GetFormInstances(filter, true);
        }

        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.Engine)]
        public ActionResult Edit(FormInstanceFilterDataIn filter)
        {
            return GetEditFormInstance(filter);
        }

        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.Engine)]
        public ActionResult EditForProject(FormInstanceFilterDataIn filter)
        {
            return GetEditFormInstance(filter);
        }

        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.Engine)]
        public ActionResult EditForUserProject(FormInstanceFilterDataIn filter)
        {
            return GetEditFormInstance(filter, "", "", true);
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Engine)]
        public ActionResult View(FormInstanceFilterDataIn filter)
        {
            return GetEditFormInstance(filter);
        }

        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.Engine)]
        public ActionResult DataCaptureViewMode()
        {
            FormInstanceMetadataDataOut formInstaceMetadata = formInstanceBLL.GetFormInstanceKeyDataFirst(userCookieData.Id);
            if (formInstaceMetadata != null)
            {
                return RedirectToAction("Edit", "FormInstance", new
                {
                    formInstaceMetadata.VersionId,
                    formInstaceMetadata.FormInstanceId
                });
            }
            else
            {
                return RedirectToAction("GetAllFormDefinitions", "FormInstance");
            }
        }

        [HttpDelete]
        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.Delete, Module = ModuleNames.Engine)]
        public async Task<ActionResult> Delete(string formInstanceId, DateTime lastUpdate)
        {
            return await DeleteFormInstance(formInstanceId, lastUpdate).ConfigureAwait(false);
        }

        public ActionResult GetFormInstanceContent(FormInstanceReloadDataIn dataIn)
        {
            switch (dataIn.ViewMode)
            {
                case FormInstanceViewMode.SynopticView:
                    return GetEditFormInstance(new FormInstanceFilterDataIn { FormInstanceId = dataIn.FormInstanceId }, partialViewName: "SynopticView");
                default:
                    SetReadOnlyAndDisabledViewBag(dataIn.IsReadOnlyViewMode);
                    SetIsHiddenFieldsShown(dataIn.HiddenFieldsShown);
                    return GetEditFormInstance(
                        new FormInstanceFilterDataIn
                        {
                            FormInstanceId = dataIn.FormInstanceId,
                            ActiveChapterId = dataIn.ActiveChapterId,
                            ActivePageId = dataIn.ActivePageId,
                            ActivePageLeftScroll = dataIn.ActivePageLeftScroll
                        },
                        partialViewName: "~/Views/FormInstance/FormInstanceContent.cshtml",
                        actionUrl: "/FormInstance/Edit"
                    );
            }
        }

        public ActionResult GetFieldsToPlot(string formId)
        {
            FormDataOut formDataOut = formBLL.GetFormDataOutById(formId, userCookieData);

            List<FieldDataOut> fieldsDataOut = formBLL.GetPlottableFields(formId);
            ViewBag.FormID = formId;
            ViewBag.FormTitle = formDataOut.Title;
            ViewBag.FieldDataOutList = fieldsDataOut;

            return PartialView("~/Views/FormInstance/ChartFiltersPartial.cshtml");
        }

        public ActionResult GetFormInstanceFieldsById(FormInstancePlotDataIn dataIn)
        {
            dataIn.OrganizationId = userCookieData.ActiveOrganization;
            DataCaptureChartUtility chartUtilityDataStructure = formInstanceBLL.GetPlottableFieldsByThesaurusId(dataIn, formBLL.GetPlottableFields(dataIn.FormDefinitionId));


            var result = Json(chartUtilityDataStructure);

            return result;
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Engine)]
        public ActionResult ShowMissingValuesModal(int thesaurusId, string versionId, List<string> fieldsIds, bool canSaveWithoutValue)
        {
            Form form = formBLL.GetFormByThesaurusAndLanguageAndVersionAndOrganization(thesaurusId, userCookieData.ActiveOrganization, userCookieData.ActiveLanguage, versionId);
            if (form == null)
            {
                return NotFound(TextLanguage.FormNotExists, thesaurusId.ToString());
            }
            ViewBag.NullFlavors = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.NullFlavor);
            FormDataOut data = GetDataOutForCreatingNewFormInstance(form, formReferrals: null);
            data.RequiredFieldsWithoutValue = canSaveWithoutValue ? data.GetAllFieldsWhichCanSaveWithoutValue(fieldsIds) : data.GetAllFieldsWhichCannotSaveWithoutValue(fieldsIds);

            return PartialView("~/Views/FormInstance/MissingValuesModal.cshtml", data);
        }

        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.Engine)]
        [SReportsAuditLog]
        [HttpPost]
        public ActionResult AddFieldsetRepetition([ModelBinder(typeof(JsonNetModelBinder))] AddFieldSetRepetitionDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));

            List<FieldInstance> fieldInstances = formInstanceBLL.ParseFormInstanceFields(dataIn.FieldInstances);
            FormFieldSetDataOut fieldSetDataOut = formBLL.AddFieldsetRepetition(dataIn.FormId, dataIn.FieldsetId, fieldInstances);

            ViewBag.IsLastFieldsetOnPage = dataIn.IsLastFieldsetOnPage;
            ViewBag.FsNumsInRepetition = dataIn.FsNumsInRepetition;
            ViewBag.IsLastFieldsetInRepetition = true;
            ViewBag.AddFieldsetRepetition = true;
            ViewBag.Arguments = new Dictionary<string, bool>
            {
                { FormInstanceConstants.IsChapterReadOnly, false },
                { FormInstanceConstants.ReadOnlyOrLocked, false },
            };
            SetIsHiddenFieldsShown(dataIn.HiddenFieldsShown);
            return PartialView("~/Views/FormInstance/FormInstanceFieldSet.cshtml", fieldSetDataOut);
        }

        private ActionResult GetAllFormInstances(FormInstanceFilterDataIn dataIn, bool showUserProjects = false)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));

            if (!this.formDAL.ExistsFormByThesaurus(dataIn.ThesaurusId))
            {
                return NotFound(TextLanguage.FormForThesaurusIdNotExists, dataIn.ThesaurusId.ToString());
            }

            Form form = this.formBLL.GetFormByThesaurusAndLanguageAndVersionAndOrganization(dataIn.ThesaurusId, userCookieData.ActiveOrganization, dataIn.Language ?? userCookieData.ActiveLanguage, dataIn.VersionId);
            if (form == null)
            {
                return RedirectToAction("GetAllFormDefinitions");
            }

            //filter data
            dataIn.Title = form.Title;
            dataIn.FormId = form.Id;
            dataIn.OrganizationId = userCookieData.ActiveOrganization;

            ViewBag.FilterFormInstanceDataIn = dataIn;
            ViewBag.Language = dataIn.Language;
            SetProjectViewBags(dataIn, showUserProjects);

            return View("GetAllByFormThesaurus", dataIn.IsSimplifiedLayout ? "_Crf_Layout" : "_Layout");
        }

        private ActionResult GetFormInstances(FormInstanceFilterDataIn filter, bool showUserProjects = false)
        {
            filter = Ensure.IsNotNull(filter, nameof(filter));

            Form form = formBLL.GetFormByThesaurusAndLanguageAndVersionAndOrganization(filter.ThesaurusId, userCookieData.ActiveOrganization, userCookieData.ActiveLanguage, filter.VersionId);
            if (form == null)
            {
                return NotFound(TextLanguage.FormNotExists, filter.ThesaurusId.ToString());
            }

            FormDataOut data = GetDataOutForCreatingNewFormInstance(form, formReferrals: null);

            ViewBag.FilterFormInstanceDataIn = filter;
            ViewBag.Title = $"{TextLanguage.Create} {data.Title}";
            SetEngineCommonViewBags(filter, showUserProjects);

            return View("~/Views/FormInstance/FormInstance.cshtml", data);
        }

        #region Export

        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.Download, Module = ModuleNames.Engine)]
        public ActionResult ExportToTxt(FormInstanceFilterDataIn filter)
        {
            filter = Ensure.IsNotNull(filter, nameof(filter));

            FormInstance formInstance = this.formInstanceDAL.GetById(filter.FormInstanceId);

            if (formInstance == null)
            {
                return NotFound();
            }

            MemoryStream memoryStream = new MemoryStream();
            TextWriter tw = new StreamWriter(memoryStream);

            formInstanceBLL.WriteFieldsAndMetadataToStream(formInstance, tw, userCookieData.ActiveLanguage, ViewBag.DateFormat as string);

            tw.Flush();
            tw.Close();

            SetCustomResponseHeaderForMultiFileDownload();

            return File(memoryStream.ToArray(), "text/plain", formInstance.Title);
        }

        [SReportsAuthorize(Permission = PermissionNames.Download, Module = ModuleNames.Engine)]
        public ActionResult ExportToCSV(List<FormInstanceDownloadData> formInstancesForDownload)
        {
            formInstanceBLL.SendExportedFiles(
                formInstancesForDownload,
                userCookieData,
                FormInstanceConstants.LongFormat,
                FormInstanceConstants.CsvFormat,
                callAsyncRunner: true);

            return Ok();
        }

        [SReportsAuthorize(Permission = PermissionNames.Download, Module = ModuleNames.Engine)]
        public ActionResult ExportToXLSX(List<FormInstanceDownloadData> formInstancesForDownload)
        {
            formInstanceBLL.SendExportedFiles(
                formInstancesForDownload,
                userCookieData,
                FormInstanceConstants.LongFormat,
                FormInstanceConstants.XlsxFormat,
                callAsyncRunner: true);

            return Ok();
        }

        #endregion /Export

        #region (Un)Lock actions
        public ActionResult GetLockUnlockDocumentModel(FormState formInstanceNextState)
        {
            return PartialView("LockUnlockDocumentModalForm", new FormInstanceLockUnlockRequestDataIn { FormInstanceNextState = formInstanceNextState });
        }

        [SReportsAuditLog(new string[] { "Password" })]
        [HttpPost]
        [SReportsAuthorize(Permission = PermissionNames.ChangeFormInstanceState, Module = ModuleNames.Engine)]
        public ActionResult LockOrUnlockDocument(FormInstanceLockUnlockRequestDataIn formInstanceLockOrUnlockDataIn)
        {
            Ensure.IsNotNull(formInstanceLockOrUnlockDataIn, nameof(formInstanceLockOrUnlockDataIn));

            if (ModelState.IsValid)
            {
                var user = userBLL.IsValidUser(new DTOs.User.DataIn.UserLoginDataIn { Username = userCookieData.Username, Password = formInstanceLockOrUnlockDataIn.Password });
                if (user != null)
                {
                    formInstanceBLL.LockUnlockFormInstance(MapFormInstanceLock(formInstanceLockOrUnlockDataIn), userCookieData);
                    return Json($"Form instance is successfully {(formInstanceLockOrUnlockDataIn.IsLocked() ? "locked" : "unlocked")}.");
                }
                else
                {
                    ModelState.AddModelError("Password", "Incorrect Password");
                }
            }
            return PartialView("LockUnlockDocumentModalForm", formInstanceLockOrUnlockDataIn);
        }

        [SReportsAuthorize(Permission = PermissionNames.LockPage, Module = ModuleNames.Engine)]
        public ActionResult GetLockPageInstancePartially(string chapterId, string pageId)
        {
            return GetLockOrUnlockInstancePartially(chapterId, pageId, FormItemLevel.Page, true);
        }

        [SReportsAuditLog(new string[] { "Password" })]
        [HttpPost]
        [SReportsAuthorize(Permission = PermissionNames.LockPage, Module = ModuleNames.Engine)]
        public ActionResult LockPageInstancePartially(FormInstancePartialLockOrUnlockDataIn formInstancePartialLockDataIn)
        {
            return LockOrUnlockInstancePartially(formInstancePartialLockDataIn, FormItemLevel.Page);
        }

        [SReportsAuthorize(Permission = PermissionNames.UnlockPage, Module = ModuleNames.Engine)]
        public ActionResult GetUnLockPageInstancePartially(string chapterId, string pageId)
        {
            return GetLockOrUnlockInstancePartially(chapterId, pageId, FormItemLevel.Page, false);
        }

        [SReportsAuditLog(new string[] { "Password" })]
        [HttpPost]
        [SReportsAuthorize(Permission = PermissionNames.UnlockPage, Module = ModuleNames.Engine)]
        public ActionResult UnLockPageInstancePartially(FormInstancePartialLockOrUnlockDataIn formInstancePartialLockOrUnlockDataIn)
        {
            return LockOrUnlockInstancePartially(formInstancePartialLockOrUnlockDataIn, FormItemLevel.Page);
        }

        [SReportsAuthorize(Permission = PermissionNames.LockChapter, Module = ModuleNames.Engine)]
        public ActionResult GetLockChapterInstancePartially(string chapterId, string pageId)
        {
            return GetLockOrUnlockInstancePartially(chapterId, pageId, FormItemLevel.Chapter, true);
        }

        [SReportsAuditLog(new string[] { "Password" })]
        [HttpPost]
        [SReportsAuthorize(Permission = PermissionNames.LockChapter, Module = ModuleNames.Engine)]
        public ActionResult LockChapterInstancePartially(FormInstancePartialLockOrUnlockDataIn formInstancePartialLockDataIn)
        {
            return LockOrUnlockInstancePartially(formInstancePartialLockDataIn, FormItemLevel.Chapter);
        }

        [SReportsAuthorize(Permission = PermissionNames.UnlockChapter, Module = ModuleNames.Engine)]
        public ActionResult GetUnLockChapterInstancePartially(string chapterId, string pageId)
        {
            return GetLockOrUnlockInstancePartially(chapterId, pageId, FormItemLevel.Chapter, false);
        }

        [SReportsAuditLog(new string[] { "Password" })]
        [HttpPost]
        [SReportsAuthorize(Permission = PermissionNames.UnlockChapter, Module = ModuleNames.Engine)]
        public ActionResult UnLockChapterInstancePartially(FormInstancePartialLockOrUnlockDataIn formInstancePartialLockOrUnlockDataIn)
        {
            return LockOrUnlockInstancePartially(formInstancePartialLockOrUnlockDataIn, FormItemLevel.Chapter);
        }

        private ActionResult GetLockOrUnlockInstancePartially(string chapterId, string pageId, FormItemLevel itemType, bool isLockAction)
        {
            return PartialView("LockUnlockDocumentModalForm", new FormInstancePartialLockOrUnlockDataIn
            {
                ChapterId = chapterId,
                PageId = pageId,
                ActionEndpoint = GetPartialLockUnlockActionEndpoint(isLockAction, itemType),
                ChapterPageNextState = isLockAction ? ChapterPageState.Locked : ChapterPageState.DataEntryOnGoing
            });
        }

        private ActionResult LockOrUnlockInstancePartially(FormInstancePartialLockOrUnlockDataIn formInstancePartialLockOrUnlockDataIn, FormItemLevel itemType)
        {
            formInstancePartialLockOrUnlockDataIn = Ensure.IsNotNull(formInstancePartialLockOrUnlockDataIn, nameof(formInstancePartialLockOrUnlockDataIn));
            bool isLockAction = formInstancePartialLockOrUnlockDataIn.IsLockAction();
            if (ModelState.IsValid)
            {
                var user = userBLL.IsValidUser(new DTOs.User.DataIn.UserLoginDataIn { Username = userCookieData.Username, Password = formInstancePartialLockOrUnlockDataIn.Password });
                if (user != null)
                {
                    formInstanceBLL.LockUnlockChapterOrPage(MapFormInstancePartialLock(formInstancePartialLockOrUnlockDataIn), userCookieData);
                    return Json($"{itemType} is successfully {(isLockAction ? "locked" : "unlocked")}.");
                }
                else
                {
                    ModelState.AddModelError("Password", "Incorrect Password");
                }
            }
            formInstancePartialLockOrUnlockDataIn.ActionEndpoint = GetPartialLockUnlockActionEndpoint(isLockAction, itemType);
            return PartialView("LockUnlockDocumentModalForm", formInstancePartialLockOrUnlockDataIn);
        }

        private FormInstancePartialLock MapFormInstancePartialLock(FormInstancePartialLockOrUnlockDataIn formInstancePartialLockOrUnlockDataIn)
        {
            FormInstancePartialLock formInstancePartialLock = Mapper.Map<FormInstancePartialLock>(formInstancePartialLockOrUnlockDataIn);
            formInstancePartialLock.CreateById = userCookieData?.Id;
            formInstancePartialLock.IsSigned = true;

            return formInstancePartialLock;
        }

        private FormInstanceLockUnlockRequest MapFormInstanceLock(FormInstanceLockUnlockRequestDataIn formInstanceSignDataIn)
        {
            FormInstanceLockUnlockRequest formInstanceSign = Mapper.Map<FormInstanceLockUnlockRequest>(formInstanceSignDataIn);
            formInstanceSign.CreatedById = userCookieData?.Id;

            return formInstanceSign;
        }

        private string GetPartialLockUnlockActionEndpoint(bool isLock, FormItemLevel itemType)
        {
            return string.Format("/FormInstance/{0}Lock{1}InstancePartially", isLock ? string.Empty : "Un", itemType);
        }
        #endregion /(Un)Lock actions

        #region Handle Dependency
        [HttpPost]
        public ActionResult ExecuteDependenciesFormulas([ModelBinder(typeof(JsonNetModelBinder))] FieldInstanceDependenciesDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            return Json(formBLL.ExecuteDependenciesFormulas(dataIn));
        }
        #endregion /Handle Dependency

        #region CTCAE

        [HttpPost]
        public ActionResult CreateCTCAE(CTCAEPatient patient, FormInstanceDataIn formInstanceDataIn)
        {
            UserData userData = Mapper.Map<UserData>(userCookieData);

            if (patient == null)
            {
                return NotFound(TextLanguage.FormInstanceNotExists, "Patient cannot be null.");
            }
            Form form = formDAL.GetFormByThesaurus(15120);
            FormInstance formInstance = formInstanceBLL.GetFormInstanceSet(form, formInstanceDataIn, userCookieData);

            SetFormInstanceFields(form, formInstance, patient);


            for (int i = 0; i < patient.ReviewModels.Count; i++)
            {
                SetRepetitiveFields(form, formInstance, patient.ReviewModels[i]);
            }

            if (patient.FormInstanceId != null)
            {
                FormInstance formInstanceForUpdate = SetFormInstanceIdForUpdate(patient, formInstance);
                formInstanceDAL.InsertOrUpdate(formInstanceForUpdate, formInstance.GetCurrentFormInstanceStatus(userCookieData?.Id));
            }
            else
            {
                SetCTCAEPatient(form, formInstance, patient, userData);
                formInstanceDAL.InsertOrUpdate(formInstance, formInstance.GetCurrentFormInstanceStatus(userCookieData?.Id));
            }

            return StatusCode(StatusCodes.Status201Created);
        }

        private string GetFieldValue(CTCAEPatient patient, int fieldIndex)
        {
            switch (fieldIndex)
            {
                case 0:
                    return patient.PatientId.ToString();
                case 1:
                    return patient.VisitNo;
                case 2:
                    return patient.Date?.ToString(DateConstants.UTCDatePartFormat);
                case 3:
                    return patient.Title;
                default:
                    return string.Empty;
            }
        }

        private void SetRepetitiveFields(Form form, FormInstance formInstance, ReviewModel reviewModel)
        {
            FieldSet clonedFieldSet = form.Chapters[0].Pages[0].ListOfFieldSets[0][0].Clone();
            clonedFieldSet.FieldSetInstanceRepetitionId = clonedFieldSet.FieldSetInstanceRepetitionId.GenerateGuidIfNotDefined();

            formInstance.FieldInstances.Add(new FieldInstance(
                clonedFieldSet.Fields[0], 
                clonedFieldSet.Id,
                clonedFieldSet.FieldSetInstanceRepetitionId,
                new FieldInstanceValue(reviewModel.MedDRACode)
                ));
            formInstance.FieldInstances.Add(new FieldInstance(
                clonedFieldSet.Fields[1],
                clonedFieldSet.Id,
                clonedFieldSet.FieldSetInstanceRepetitionId,
                new FieldInstanceValue(reviewModel.CTCAETerms)
            ));
            formInstance.FieldInstances.Add(new FieldInstance(
                clonedFieldSet.Fields[2],
                clonedFieldSet.Id,
                clonedFieldSet.FieldSetInstanceRepetitionId,
                new FieldInstanceValue(
                        ((FieldSelectable)clonedFieldSet.Fields[2])
                            .Values
                            .Find(
                                x => x.Label == reviewModel.Grades).Id
                            )
                
                    ));
            formInstance.FieldInstances.Add(new FieldInstance(
                clonedFieldSet.Fields[3],
                clonedFieldSet.Id,
                clonedFieldSet.FieldSetInstanceRepetitionId,
                new FieldInstanceValue(reviewModel.GradeDescription)
                ));
        }

        private void SetCTCAEPatient(Form form, FormInstance formInstance, CTCAEPatient patient, UserData user)
        {
            if (!form.DisablePatientData)
            {
                int patientId = 0;
                Patient patientEntity = patientDAL.GetById(patient.PatientId);
                if (patientEntity == null)
                {
                    patientEntity = new Patient("Unknown", "Unknown");
                    patient.PatientId = 0;
                    patientEntity.PatientId = patient.PatientId;
                    patientEntity.OrganizationId = user.ActiveOrganization.GetValueOrDefault();
                    patientDAL.InsertOrUpdate(patientEntity, null);
                }

                formInstance.PatientId = patientEntity.PatientId;
                int eocId = InsertEpisodeOfCare(patientId, form.EpisodeOfCare, "Engine", DateTime.Now, user);
                int encounterId = InsertEncounter(eocId);
                formInstance.EpisodeOfCareRef = eocId;
                formInstance.EncounterRef = encounterId;
            }
        }

        private void SetFormInstanceFields(Form form, FormInstance formInstance, CTCAEPatient patient)
        {
            formInstance.FieldInstances = new List<FieldInstance>();
            foreach (FieldSet fieldSet in form.Chapters[0].Pages[0].ListOfFieldSets[1])
            {
                fieldSet.FieldSetInstanceRepetitionId = fieldSet.FieldSetInstanceRepetitionId.GenerateGuidIfNotDefined();
                for (int i = 0; i < fieldSet.Fields.Count; i++)
                {
                    formInstance.FieldInstances.Add(
                        new FieldInstance(
                            fieldSet.Fields[i],
                            fieldSet.Id,
                            fieldSet.FieldSetInstanceRepetitionId,
                            new FieldInstanceValue(GetFieldValue(patient, i))
                    ));
                }
            }
        }

        private FormInstance SetFormInstanceIdForUpdate(CTCAEPatient patient, FormInstance formInstance)
        {
            FormInstance formInstanceForUpdate = formInstanceDAL.GetById(patient.FormInstanceId);
            Form formForUpdate = formDAL.GetForm(formInstanceForUpdate.FormDefinitionId);
            formForUpdate.SetValuesFromReferrals(formBLL.GetFormsFromReferrals(new List<FormInstance>() { formInstance, formInstanceForUpdate }));

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
        #endregion /CTCAE

        #region Covid Filter

        public ActionResult GetAllFormInstance(FormInstanceCovidFilterDataIn filter)
        {
            return FormatCovidResponse(formInstanceDAL.GetAllByCovidFilter(Mapper.Map<FormInstanceCovidFilter>(filter)));
        }

        public async Task<ActionResult> GetTest()
        {
            return FormatCovidResponse(await formInstanceDAL.GetAllFieldsByCovidFilter().ConfigureAwait(false));
        }

        private ContentResult FormatCovidResponse(List<FormInstance> resultData)
        {
            var jsonData = JsonConvert.SerializeObject(resultData, new JsonSerializerSettings
            {
                MaxDepth = Int32.MaxValue,
                Formatting = Formatting.Indented,
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            });

            return new ContentResult
            {
                Content = jsonData,
                ContentType = "application/json"
            };
        }
        #endregion /Covid Filter

        #endregion

        //    #region Hackaton mode
        //    /** HACKATON CODE **************************************************************************************************************************/
        //        private static ObjectCache cache = MemoryCache.Default;
        //    private async Task<List<FormInstance>> GetFormsAsync(bool invalidateCache=false)
        //    {
        //        List<FormInstance> forms = cache["forms"] as List<FormInstance>;
        //        if (forms == null || invalidateCache==true)
        //        {
        //            CacheItemPolicy policy = new CacheItemPolicy() { AbsoluteExpiration = DateTimeOffset.Now.AddDays(330.0) };
        //            FormInstanceCovidFilter filter = new FormInstanceCovidFilter() { ThesaurusId = "14573" };
        //            forms = await formInstanceService.GetAllFieldsByCovidFilter().ConfigureAwait(false);
        //            cache.Set("forms", forms, policy);
        //        }
        //        return forms;
        //    }

        //    private ContentResult GetResult(GFeed feed)
        //    {
        //        var serializer = new JavaScriptSerializer();
        //        var result = new ContentResult
        //        {
        //            Content = serializer.Serialize(feed.GetContent()),
        //            ContentType = "application/json"
        //        };
        //        return result;
        //    }

        //    public async void InvalidateFormsCache()
        //    {
        //        await GetFormsAsync(true).ConfigureAwait(false);
        //    }

        //    //http://localhost:55524/FormInstance/GetSelection?ThesaurusId=14583
        //    //[OutputCache(Duration=200000, VaryByParam="thesaurusId") ]
        //    public async Task<ActionResult> GetSelectionAsync(string thesaurusId)
        //    {
        //        //string thesaurusId = "14586";
        //        List<FormInstance> forms = await GetFormsAsync();
        //        FormInstance firstForm = forms[0];
        //        Domain.Entities.FieldEntity.Field field = firstForm.GetFieldByThesaurus(thesaurusId);
        //        GFeedTable t = (new GFeed(field.Label)).FirstTable;
        //        t.AddRow(new List<string>() { field.Label, field.Label });
        //        List<Domain.Entities.FieldEntity.Field> allFields = forms.SelectMany(x => x.GetAllFields().Where(f => f.ThesaurusId == thesaurusId)).ToList();

        //        if (field is Domain.Entities.FieldEntity.FieldNumeric)
        //        {
        //            CountYears(allFields, t);
        //        }
        //        else
        //        {
        //            foreach (FormFieldValue fv in (field as Domain.Entities.FieldEntity.FieldSelectable).Values)
        //            {
        //                int repCount;
        //                if (field.Type == "radio")
        //                    repCount = allFields.Count(g => g.Value?[0] == fv.ThesaurusId);
        //                else if (field.Type == "checkbox")
        //                    repCount = allFields.Count(g => g.Value.Contains(fv.Value));
        //                else
        //                    repCount = allFields.Count(g => g.Value?[0] == fv.Value);

        //                t.AddRow(new List<string>() { fv.Label, "" + repCount });
        //            }
        //        }
        //        return GetResult(t.feed);
        //    }

        //    public async Task<FileContentResult> GetData()
        //    {
        //        Dictionary<string, Dictionary<string, int>> fValues = new Dictionary<string, Dictionary<string, int>>();
        //        List<FormInstance> forms = await GetFormsAsync();

        //        StringBuilder sb = new StringBuilder();
        //        List<Domain.Entities.FieldEntity.Field> allFields2 = forms.FirstOrDefault()?.GetAllFields();

        //        sb.Append(",");
        //        foreach (Domain.Entities.FieldEntity.Field fld in allFields2)
        //        {
        //            sb.Append(fld.Label.Replace(","," ") + ",");
        //        }
        //        sb.Append(Environment.NewLine);
        //        //Dictionary<PatientId, Dictionary<FieldThesaurusId, AnswerVal>
        //        Dictionary<string, Dictionary<string, string>> answers = new Dictionary<string, Dictionary<string, string>>();

        //        foreach (FormInstance frm in forms)
        //        {
        //            sb.Append(frm.Id + ",");
        //            answers[frm.Id] = new Dictionary<string, string>();
        //            foreach (Domain.Entities.FieldEntity.Field fld in frm.GetAllFields())
        //            {
        //                answers[frm.Id][fld.ThesaurusId] = GetFieldVal(fld);
        //                sb.Append(GetFieldVal(fld) + ",");
        //            }
        //            sb.Append(Environment.NewLine);
        //        }

        //        return File(new System.Text.UTF8Encoding().GetBytes(sb.ToString()), "text/csv", "wemedoo.com.csv");
        //    }

        //    //http://localhost:55524/FormInstance/GetMultiSelection?tableName=as&tableHeader=YES,NO,Unknown&thesaurusIdList=14585,14586,14584,14583
        //    //[OutputCache(Duration = 200000, VaryByParam = "tableName;tableHeader;thesaurusIdList")]
        //    public async Task<ActionResult> GetMultiSelectionAsync(string tableName, string tableHeader, string thesaurusIdList)
        //    {
        //        Dictionary<string, Dictionary<string, int>> fValues = new Dictionary<string, Dictionary<string, int>>();
        //        List<FormInstance> forms = await GetFormsAsync();

        //        /*
        //        StringBuilder sb = new StringBuilder();
        //        List<FormField> allFields2 = forms.FirstOrDefault()?.GetAllFields();
        //        foreach (FormField field in allFields2)
        //        {
        //            sb.Append("\"" + field.Label + "\", ");
        //            sb.Append("\"" + field.Type + "\", ");
        //            sb.Append("\"" + field.ThesaurusId + "\", ");
        //            sb.Append("\"" + GetFieldVal(field) + "\", ");
        //            sb.Append(Environment.NewLine);
        //        }
        //        */

        //        GFeedTable t = (new GFeed(tableName)).FirstTable;
        //        List<string> headerLabels = tableHeader.Split(',').ToList();
        //        headerLabels.Insert(0, tableName);
        //        t.AddRow(headerLabels);

        //        var thListTemp = thesaurusIdList.Split(',').ToList();

        //        foreach (string thId in thListTemp)
        //        {
        //            List<Domain.Entities.FieldEntity.Field> allFields = forms.SelectMany(x => x.GetAllFields().Where(f => f.ThesaurusId == thId)).ToList();
        //            foreach (Domain.Entities.FieldEntity.Field field in allFields)
        //            {
        //                if (!fValues.ContainsKey(field.Label))
        //                    fValues[field.Label] = new Dictionary<string, int>();

        //                string fieldValue = GetFieldVal(field);
        //                List<string> fieldValues = new List<string>() { fieldValue };
        //                if (field.Type == "checkbox")
        //                {
        //                    fieldValues = fieldValue.Split(',').ToList();
        //                }
        //                foreach (string val in fieldValues)
        //                {
        //                    if (!fValues[field.Label].ContainsKey(val))
        //                        fValues[field.Label][val] = 0;
        //                    fValues[field.Label][val] = fValues[field.Label][val] + 1;
        //                }
        //            }
        //        }

        //        foreach (string k in fValues.Keys)
        //        {
        //            List<string> row = new List<string>();
        //            row.Add(k);
        //            for (int i = 1; i < headerLabels.Count; i++)
        //            {
        //                string headerL = headerLabels[i];
        //                if (fValues[k].ContainsKey(headerL))
        //                    row.Add("" + fValues[k][headerL]);
        //                else
        //                    row.Add("0");
        //            }
        //            t.AddRow(row);
        //        }

        //        return GetResult(t.feed);
        //    }


        //    private string GetFieldVal(Field field)
        //    {
        //        if (field.Value == null || string.IsNullOrEmpty(field.Value[0]))
        //            return "";

        //        if (field is FieldRadio)
        //            return (field as FieldRadio).Values.FirstOrDefault(v => v.ThesaurusId == field.Value[0]).Label;
        //        else
        //            return field.Value[0];
        //    }

        //    private void CountYears(List<Domain.Entities.FieldEntity.Field> allFields, GFeedTable t)
        //    {
        //        Dictionary<string, int> yearsDistribution = new Dictionary<string, int>();
        //        yearsDistribution.Add("0-10", 0);
        //        yearsDistribution.Add("10-20", 0);
        //        yearsDistribution.Add("20-30", 0);
        //        yearsDistribution.Add("30-40", 0);
        //        yearsDistribution.Add("40-50", 0);
        //        yearsDistribution.Add("50-60", 0);
        //        yearsDistribution.Add("60-70", 0);
        //        yearsDistribution.Add("70-80", 0);
        //        yearsDistribution.Add("80-90", 0);
        //        yearsDistribution.Add("90-100", 0);
        //        yearsDistribution.Add("100+", 0);

        //        foreach (Domain.Entities.FieldEntity.Field f in allFields)
        //        {
        //            decimal years;
        //            if (decimal.TryParse(f.Value?[0], out years))
        //            {
        //                if (years <= 10) yearsDistribution["0-10"]++;
        //                else if (years > 10 && years <= 20) yearsDistribution["10-20"]++;
        //                else if (years > 20 && years <= 30) yearsDistribution["20-30"]++;
        //                else if (years > 30 && years <= 40) yearsDistribution["30-40"]++;
        //                else if (years > 40 && years <= 50) yearsDistribution["40-50"]++;
        //                else if (years > 50 && years <= 60) yearsDistribution["50-60"]++;
        //                else if (years > 60 && years <= 70) yearsDistribution["60-70"]++;
        //                else if (years > 70 && years <= 80) yearsDistribution["70-80"]++;
        //                else if (years > 80 && years <= 90) yearsDistribution["80-90"]++;
        //                else if (years > 90 && years <= 100) yearsDistribution["90-100"]++;
        //                else yearsDistribution["100+"]++;
        //            }
        //        }
        //        foreach (string k in yearsDistribution.Keys)
        //        {
        //            t.AddRow(new List<string>() { k, "" + yearsDistribution[k] });
        //        }
        //    }
        //}


        //public class GFeedTable
        //{
        //    public GFeed feed { get; set; }
        //    public List<List<string>> tContent = new List<List<string>>();

        //    public List<string> AddRow(List<string> colValues)
        //    {
        //        tContent.Add(colValues);
        //        return colValues;
        //    }

        //    public List<List<string>> GetRows()
        //    {
        //        return tContent;
        //    }
        //}

        //public class GFeed
        //{
        //    public Dictionary<string, GFeedTable> tables = new Dictionary<string, GFeedTable>();

        //    public GFeedTable FirstTable { get; set; }

        //    public GFeed(string firstTableName)
        //    {
        //        FirstTable = AddTable(firstTableName);
        //    }

        //    public GFeedTable GetTable(string tableName)
        //    {
        //        return tables[tableName];
        //    }

        //    public GFeedTable AddTable(string tableName)
        //    {
        //        tables[tableName] = new GFeedTable();
        //        tables[tableName].feed = this;
        //        return tables[tableName];
        //    }

        //    public object GetContent()
        //    {
        //        List<List<List<string>>> retVal = new List<List<List<string>>>();
        //        foreach (string k in tables.Keys)
        //        {
        //            retVal.Add(tables[k].GetRows());
        //        }
        //        return retVal;
        //    }
        //    #endregion
    }

}
