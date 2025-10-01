using AutoMapper;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Constants;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Common.Entities.User;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Extensions;
using sReportsV2.Cache.Singleton;
using sReportsV2.DTOs.Autocomplete;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.EpisodeOfCare;
using sReportsV2.DTOs.Pagination;
using sReportsV2.DTOs.Patient;
using sReportsV2.DTOs.Patient.DataIn;
using System;
using System.Collections.Generic;
using System.Linq;
using sReportsV2.DTOs.CodeEntry.DataOut;
using System.Threading.Tasks;
using sReportsV2.DTOs.DTOs.Patient.DataIn;
using sReportsV2.DTOs.Patient.DataOut;
using sReportsV2.DTOs.DTOs.PatientList;
using sReportsV2.DTOs.DTOs.PatientList.DataIn;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace sReportsV2.Controllers
{
    public class PatientController : BaseController
    {
        private readonly IPatientBLL patientBLL;
        private readonly IEncounterBLL encounterBLL;
        private readonly ICodeSetBLL codeSetBLL;
        private readonly ICodeBLL codeBLL;
        private readonly IPatientListBLL patientListBLL;
        private readonly IMapper mapper;

        public PatientController(IPatientBLL patientBLL, 
            IEncounterBLL encounterBLL, 
            ICodeSetBLL codeSetBLL, 
            ICodeBLL codeBLL, 
            IPatientListBLL patientListBLL, 
            IMapper mapper,            
            IHttpContextAccessor httpContextAccessor, 
            IServiceProvider serviceProvider, 
            IConfiguration configuration, 
            IAsyncRunner asyncRunner,
            ICacheRefreshService cacheRefreshService) : 
            base(httpContextAccessor, serviceProvider, configuration, asyncRunner, cacheRefreshService)
        {
            this.patientBLL = patientBLL;
            this.encounterBLL = encounterBLL;
            this.codeSetBLL = codeSetBLL;
            this.codeBLL = codeBLL;
            this.patientListBLL = patientListBLL;
            this.mapper = mapper;
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Patients)]
        public ActionResult GetAll(PatientFilterDataIn dataIn)
        {
            SetCountryNameIfFilterByCountryIsIncluded(dataIn);
            ViewBag.FilterData = dataIn;
            SetIdentifierTypesToViewBag();
            return View();
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Patients)]
        public async Task<ActionResult> ReloadTable(PatientFilterDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            dataIn.OrganizationId = userCookieData.ActiveOrganization;
            dataIn.ActiveLanguage = userCookieData.ActiveLanguage;
            PaginationDataOut<PatientDataOut, PatientFilterDataIn> result = await patientBLL.GetAllFilteredAsync<PatientDataOut>(dataIn).ConfigureAwait(false);
            await SetPatientListsViewBag(dataIn, result.Data).ConfigureAwait(false);
            SetIdentifierTypesToViewBag();
            SetGenderTypesToViewBag();
            SetEpisodeOfCareAndEncounterViewBags();

            return PartialView("PatientEntryTable", result);
        }

        #region CRUD
        [SReportsAuthorize(Permission = PermissionNames.Create, Module = ModuleNames.Patients)]
        public ActionResult Create()
        {
            SetPatientInfoPropertiesToViewBag();
            SetReadOnlyAndDisabledViewBag(false);
            return View("PatientEdit");
        }

        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.Patients)]
        public ActionResult Edit(PatientEditDataIn patientEditDataIn)
        {
            return GetEditViewResponse(patientEditDataIn);
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Patients)]
        public ActionResult View(PatientEditDataIn patientEditDataIn)
        {
            return GetEditViewResponse(patientEditDataIn);
        }

        [SReportsAuthorize(Permission = PermissionNames.Create, Module = ModuleNames.Patients)]
        [SReportsAuditLog]
        [HttpPost]
        [SReportsModelStateValidate]
        public ActionResult Create(PatientDataIn patient)
        {
            return CreateOrEdit(patient);
        }

        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.Patients)]
        [SReportsAuditLog]
        [HttpPost]
        [SReportsModelStateValidate]
        public ActionResult Edit(PatientDataIn patient)
        {
            return CreateOrEdit(patient);
        }

        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.Patients)]
        [SReportsAuditLog]
        [HttpPost]
        [SReportsModelStateValidate]
        public async Task<ActionResult> CreateContactInfo(PatientContactDataIn patientContact)
        {
            ResourceCreatedDTO resourceCreatedDTO = await patientBLL.InsertOrUpdate(patientContact).ConfigureAwait(false);

            if (patientContact.Id != 0)
            {
                Response.StatusCode = 201;
            }
            return Json(resourceCreatedDTO);
        }

        [SReportsAuthorize(Permission = PermissionNames.Create, Module = ModuleNames.Patients)]
        [SReportsAuditLog]
        [HttpPost]
        [SReportsModelStateValidate]
        public async Task<ActionResult> CreateIdentifier(PatientIdentifierDataIn identifierDataIn)
        {
            return await CreateOrEdit(identifierDataIn).ConfigureAwait(false);
        }

        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.Patients)]
        [SReportsAuditLog]
        [HttpPost]
        [SReportsModelStateValidate]
        public async Task<ActionResult> EditIdentifier(PatientIdentifierDataIn identifierDataIn)
        {
            return await CreateOrEdit(identifierDataIn).ConfigureAwait(false);
        }

        [SReportsAuthorize(Permission = PermissionNames.Create, Module = ModuleNames.Patients)]
        [SReportsAuditLog]
        [HttpPost]
        [SReportsModelStateValidate]
        public async Task<ActionResult> CreateAddress(PatientAddressDataIn addressDataIn)
        {
            return await CreateOrEdit(addressDataIn).ConfigureAwait(false);
        }

        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.Patients)]
        [SReportsAuditLog]
        [HttpPost]
        [SReportsModelStateValidate]
        public async Task<ActionResult> EditAddress(PatientAddressDataIn addressDataIn)
        {
            return await CreateOrEdit(addressDataIn).ConfigureAwait(false);
        }

        [SReportsAuthorize(Permission = PermissionNames.Create, Module = ModuleNames.Patients)]
        [SReportsAuditLog]
        [HttpPost]
        [SReportsModelStateValidate]
        public async Task<ActionResult> CreateContactAddress(PatientContactAddressDataIn addressContactDataIn)
        {
            return await CreateOrEdit(addressContactDataIn).ConfigureAwait(false);
        }

        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.Patients)]
        [SReportsAuditLog]
        [HttpPost]
        [SReportsModelStateValidate]
        public async Task<ActionResult> EditContactAddress(PatientContactAddressDataIn addressContactDataIn)
        {
            return await CreateOrEdit(addressContactDataIn).ConfigureAwait(false);
        }

        [SReportsAuthorize(Permission = PermissionNames.Create, Module = ModuleNames.Patients)]
        [SReportsAuditLog]
        [HttpPost]
        [SReportsModelStateValidate]
        public async Task<ActionResult> CreateTelecom(PatientTelecomDataIn telecomDataIn)
        {
            return await CreateOrEdit(telecomDataIn).ConfigureAwait(false);
        }

        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.Patients)]
        [SReportsAuditLog]
        [HttpPost]
        [SReportsModelStateValidate]
        public async Task<ActionResult> EditTelecom(PatientTelecomDataIn telecomDataIn)
        {
            return await CreateOrEdit(telecomDataIn).ConfigureAwait(false);
        }

        [SReportsAuthorize(Permission = PermissionNames.Create, Module = ModuleNames.Patients)]
        [SReportsAuditLog]
        [HttpPost]
        [SReportsModelStateValidate]
        public async Task<ActionResult> CreateContactTelecom(PatientContactTelecomDataIn telecomContactDataIn)
        {
            return await CreateOrEdit(telecomContactDataIn).ConfigureAwait(false);
        }

        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.Patients)]
        [SReportsAuditLog]
        [HttpPost]
        [SReportsModelStateValidate]
        public async Task<ActionResult> EditContactTelecom(PatientContactTelecomDataIn telecomContactDataIn)
        {
            return await CreateOrEdit(telecomContactDataIn).ConfigureAwait(false);
        }

        [SReportsAuthorize(Permission = PermissionNames.Delete, Module = ModuleNames.Patients)]
        [SReportsAuditLog]
        [HttpDelete]
        public async Task<ActionResult> Delete(PatientDataIn patientDataIn)
        {
            await patientBLL.Delete(patientDataIn).ConfigureAwait(false);
            return NoContent();
        }

        [SReportsAuthorize(Permission = PermissionNames.Delete, Module = ModuleNames.Patients)]
        [HttpDelete]
        [SReportsAuditLog]
        public async Task<ActionResult> DeleteContact(PatientContactDataIn patientContactDataIn)
        {
            await patientBLL.Delete(patientContactDataIn).ConfigureAwait(false);
            return NoContent();
        }

        [SReportsAuthorize(Permission = PermissionNames.Delete, Module = ModuleNames.Patients)]
        [HttpDelete]
        [SReportsAuditLog]
        public async Task<ActionResult> DeleteIdentifier(PatientIdentifierDataIn patientIdentifierDataIn)
        {
            await patientBLL.Delete(patientIdentifierDataIn).ConfigureAwait(false);
            return NoContent();
        }

        [SReportsAuthorize(Permission = PermissionNames.Delete, Module = ModuleNames.Patients)]
        [HttpDelete]
        [SReportsAuditLog]
        public async Task<ActionResult> DeleteAddress(PatientAddressDataIn patientAddressDataIn)
        {
            await patientBLL.Delete(patientAddressDataIn).ConfigureAwait(false);
            return NoContent();
        }

        [SReportsAuthorize(Permission = PermissionNames.Delete, Module = ModuleNames.Patients)]
        [HttpDelete]
        [SReportsAuditLog]
        public async Task<ActionResult> DeleteContactAddress(PatientContactAddressDataIn patientContactAddressDataIn)
        {
            await patientBLL.Delete(patientContactAddressDataIn).ConfigureAwait(false);
            return NoContent();
        }

        [SReportsAuthorize(Permission = PermissionNames.Delete, Module = ModuleNames.Patients)]
        [HttpDelete]
        [SReportsAuditLog]
        public async Task<ActionResult> DeleteTelecom(PatientTelecomDataIn patientTelecomDataIn)
        {
            await patientBLL.Delete(patientTelecomDataIn).ConfigureAwait(false);
            return NoContent();
        }

        [SReportsAuthorize(Permission = PermissionNames.Delete, Module = ModuleNames.Patients)]
        [HttpDelete]
        [SReportsAuditLog]
        public async Task<ActionResult> DeleteContactTelecom(PatientContactTelecomDataIn patientContactTelecomDataIn)
        {
            await patientBLL.Delete(patientContactTelecomDataIn).ConfigureAwait(false);
            return NoContent();
        }

        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.Patients)]
        public ActionResult GetPatientLanguages(int patientId)
        {
            ViewBag.PatientLanguages = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.Language);
            return PartialView("CommunicationsTable", patientBLL.GetById(patientId, false).Communications);
        }
        #endregion /CRUD

        [SReportsAuthorize(Permission = PermissionNames.ViewEpisodeOfCare, Module = ModuleNames.Patients)]
        public ActionResult ViewPatientEncounters(PatientEditDataIn patientEditDataIn)
        {
            ViewBag.ReadOnly = true;
            return GetEditViewResponse(patientEditDataIn);
        }

        [SReportsAuthorize(Permission = PermissionNames.UpdateEpisodeOfCare, Module = ModuleNames.Patients)]
        public ActionResult EditPatientEncounters(PatientEditDataIn patientEditDataIn)
        {
            ViewBag.ReadOnly = false;
            return GetEditViewResponse(patientEditDataIn);
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Patients)]
        public async Task<ActionResult> ViewPatientInfo(int patientId, bool isReadOnlyViewMode, int? activeEOC = null)
        {
            return await GetEditPatientInfo(patientId, isReadOnlyViewMode, activeEOC).ConfigureAwait(false);
        }

        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.Patients)]
        public async Task<ActionResult> EditPatientInfo(int patientId, bool isReadOnlyViewMode, int? activeEOC = null)
        {
            return await GetEditPatientInfo(patientId, isReadOnlyViewMode, activeEOC).ConfigureAwait(false);
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Patients)]
        public async Task<ActionResult> GetPatientInfo(int patientId, bool isReadOnlyViewMode)
        {
            var patient = patientBLL.GetByIdAsync(patientId);
            SetPatientInfoPropertiesToViewBag();
            SetReadOnlyAndDisabledViewBag(isReadOnlyViewMode);

            return PartialView("PatientClinicalTrials", await patient.ConfigureAwait(false));
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Patients)]
        public async Task<ActionResult> PatientInfo(int patientId)
        {
            var patient = patientBLL.GetByIdAsync(patientId);

            return PartialView("PatientInfo", await patient.ConfigureAwait(false));
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Patients)]
        public async Task<ActionResult> PatientBasicInfo(int patientId, int episodeOfCareId, bool isReadOnlyViewMode)
        {
            SetGenderTypesToViewBag();
            ViewBag.ReadOnly = isReadOnlyViewMode;
            ViewBag.ActiveEpisodeOfCare = episodeOfCareId;
            SetEpisodeOfCareViewBags();
            var patient = patientBLL.GetByIdAsync(patientId);

            return PartialView("PatientBasicInfo", await patient.ConfigureAwait(false));
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Patients)]
        public async Task<ActionResult> EditPatientContactInfo(int contactId, bool isReadOnlyViewMode)
        {
            var patientContact = await patientBLL.GetContactById(contactId).ConfigureAwait(false);
            SetPatientInfoPropertiesToViewBag();
            SetReadOnlyAndDisabledViewBag(isReadOnlyViewMode);
            return PartialView("PatientContactForm", patientContact);
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Patients)]
        public async Task<ActionResult> GetPatientContactRow(int contactId, bool isReadOnlyViewMode)
        {
            var patientContact = await patientBLL.GetContactById(contactId).ConfigureAwait(false);
            SetGenderTypesToViewBag();
            ViewBag.ContactRelationships = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.ContactRelationship);
            SetReadOnlyAndDisabledViewBag(isReadOnlyViewMode);
            return PartialView("PatientContactRow", patientContact);
        }

        public ActionResult ExistIdentifier(PatientIdentifierDataIn dataIn)
        {
            return Json(!patientBLL.ExistEntity(dataIn));
        }

        [HttpGet]
        public ActionResult ReloadPatients(PatientByNameFilterDataIn patientSearchFilter)
        {
            patientSearchFilter = Ensure.IsNotNull(patientSearchFilter, nameof(patientSearchFilter));

            patientSearchFilter.OrganizationId = userCookieData.ActiveOrganization;
            List<PatientTableDataOut> result = patientBLL.GetPatientsByFirstAndLastName(patientSearchFilter);
            return PartialView("~/Views/Patient/PatientAutocomplete.cshtml", result);
        }

        public ActionResult GetAutoCompleteCodeData(AutocompleteDataIn dataIn, CodeSetList codeSetId)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));

            var filtered = FilterCodeByName(dataIn.Term, (int)codeSetId);
            var enumDataOuts = filtered
                .OrderBy(x => x.text).Skip(dataIn.GetHowManyElementsToSkip()).Take(FilterConstants.DefaultPageSize)
                .Where(x => string.IsNullOrEmpty(dataIn.ExcludeId) || !x.id.Equals(dataIn.ExcludeId))
                .ToList();

            AutocompleteResultDataOut result = new AutocompleteResultDataOut()
            {
                pagination = new AutocompletePaginatioDataOut()
                {
                    more = dataIn.ShouldLoadMore(filtered.Count())
                },
                results = enumDataOuts
            };

            return Json(result);
        }

        public ActionResult GetCode(int codeId, bool readOnlyMode)
        {
            CodeDataOut code = SingletonDataContainer.Instance.GetCode(codeId, !readOnlyMode);

            string activeLanguage = ViewBag.UserCookieData.ActiveLanguage as string;
            AutocompleteDataOut result = new AutocompleteDataOut();
            if (code != null)
            {
                result = new AutocompleteDataOut()
                {
                    id = code.Id.ToString(),
                    text = code.Thesaurus.GetPreferredTermByTranslationOrDefault(activeLanguage)
                };
            }

            return Json(result);
        }

        private IEnumerable<AutocompleteDataOut> FilterCodeByName(string term, int codeSetId)
        {
            string activeLanguage = ViewBag.UserCookieData.ActiveLanguage as string;
            return SingletonDataContainer.Instance.GetCodesByCodeSetId(codeSetId).Where(code => code.IsActive() && FilterCodeByName(code, activeLanguage, term)).Select(e => new AutocompleteDataOut()
            {
                id = e.Id.ToString(),
                text = e.Thesaurus.GetPreferredTermByTranslationOrDefault(activeLanguage)
            });
        }

        private bool FilterCodeByName(CodeDataOut customEnum, string activeLanguage, string term)
        {
            string preferredTerm = customEnum.Thesaurus.GetPreferredTermByTranslationOrDefault(activeLanguage);
            return !string.IsNullOrEmpty(term) && !string.IsNullOrEmpty(preferredTerm) && preferredTerm.ToLower().Contains(term.ToLower());
        }

        #region CRUD

        private ActionResult GetEditViewResponse(PatientEditDataIn patientEditDataIn)
        {
            patientEditDataIn = Ensure.IsNotNull(patientEditDataIn, nameof(patientEditDataIn));

            var patient = patientBLL.GetById(patientEditDataIn.PatientId, loadClinicalTrials: false);
            SetLoadEncountersForActiveEOCToViewBag(patient, patientEditDataIn.EpisodeOfCareId);
            SetIdentifierTypesToViewBag();
            SetEpisodeOfCareAndEncounterViewBags();
            SetActiveElementDataToViewBag(patientEditDataIn);
            SetGenderTypesToViewBag();

            ViewBag.FilterData = patientEditDataIn;
            ViewBag.ActiveEOC = patientEditDataIn.EpisodeOfCareId;
            ViewBag.ActiveEncounter = patientEditDataIn.EncounterId;
            ViewBag.ActiveForm = patientEditDataIn.FormInstanceId;
            ViewBag.ActiveEncounterType = 0;

            if (patientEditDataIn.EncounterId != 0)
                ViewBag.ActiveEncounterType = encounterBLL.GetEncounterTypeByEncounterId(patientEditDataIn.EncounterId);

            if (patientEditDataIn.EpisodeOfCareId != 0)
            {
                ViewBag.ShowEOCDropdown = true;
                return View("EditPatientEncounters", patient);
            }

            return View(EndpointConstants.Edit, patient);
        }

        private async Task<ActionResult> GetEditPatientInfo(int patientId, bool isReadOnlyViewMode, int? activeEOC = null)
        {
            var patient = patientBLL.GetByIdAsync(patientId);
            ViewBag.ActiveEOC = activeEOC;
            SetEpisodeOfCareAndEncounterViewBags();
            SetPatientInfoPropertiesToViewBag();
            SetReadOnlyAndDisabledViewBag(isReadOnlyViewMode);

            return View("PatientEdit", await patient.ConfigureAwait(false));
        }

        private JsonResult CreateOrEdit(PatientDataIn patient)
        {
            ResourceCreatedDTO resourceCreatedDTO = patientBLL.InsertOrUpdate(patient, mapper.Map<UserData>(userCookieData));

            Response.StatusCode = 201;
            return Json(resourceCreatedDTO);
        }

        private async Task<JsonResult> CreateOrEdit(PatientIdentifierDataIn identifierDataIn)
        {
            ResourceCreatedDTO resourceCreatedDTO = await patientBLL.InsertOrUpdate(identifierDataIn).ConfigureAwait(false);

            if (identifierDataIn.Id == 0)
            {
                Response.StatusCode = 201;
            }
            return Json(resourceCreatedDTO);
        }

        private async Task<JsonResult> CreateOrEdit(PatientAddressDataIn addressDataIn)
        {
            ResourceCreatedDTO resourceCreatedDTO = await patientBLL.InsertOrUpdate(addressDataIn).ConfigureAwait(false);

            if (addressDataIn.Id == 0)
            {
                Response.StatusCode = 201;
            }
            return Json(resourceCreatedDTO);
        }

        private async Task<JsonResult> CreateOrEdit(PatientContactAddressDataIn addressContactDataIn)
        {
            ResourceCreatedDTO resourceCreatedDTO = await patientBLL.InsertOrUpdate(addressContactDataIn).ConfigureAwait(false);

            if (addressContactDataIn.Id == 0)
            {
                Response.StatusCode = 201;
            }
            return Json(resourceCreatedDTO);
        }

        private async Task<JsonResult> CreateOrEdit(PatientTelecomDataIn telecomDataIn)
        {
            ResourceCreatedDTO resourceCreatedDTO = await patientBLL.InsertOrUpdate(telecomDataIn).ConfigureAwait(false);

            if (telecomDataIn.Id == 0)
            {
                Response.StatusCode = 201;
            }
            return Json(resourceCreatedDTO);
        }

        private async Task<JsonResult> CreateOrEdit(PatientContactTelecomDataIn telecomContactDataIn)
        {
            ResourceCreatedDTO resourceCreatedDTO = await patientBLL.InsertOrUpdate(telecomContactDataIn).ConfigureAwait(false);

            if (telecomContactDataIn.Id == 0)
            {
                Response.StatusCode = 201;
            }
            return Json(resourceCreatedDTO);
        }
        #endregion /CRUD

        #region ViewBags

        private void SetActiveElementDataToViewBag(PatientEditDataIn patientEditDataIn)
        {
            if (!string.IsNullOrWhiteSpace(patientEditDataIn.FormInstanceId) && patientEditDataIn.EncounterId != 0)
            {
                ViewBag.ActiveElementParent = patientEditDataIn.EncounterId;
                ViewBag.ActiveElement = patientEditDataIn.FormInstanceId;
                ViewBag.ActiveElementType = "forminstance";
            }
            else if (patientEditDataIn.EncounterId != 0 && patientEditDataIn.EpisodeOfCareId != 0)
            {
                ViewBag.ActiveElementParent = patientEditDataIn.EpisodeOfCareId;
                ViewBag.ActiveElement = patientEditDataIn.EncounterId;
                ViewBag.ActiveElementType = "encounter";
            }
            else if (patientEditDataIn.EpisodeOfCareId != 0)
            {
                ViewBag.ActiveElement = patientEditDataIn.EpisodeOfCareId;
                ViewBag.ActiveElementType = "episodeofcare";
            }

            ViewBag.IsPageReload = true;
        }

        private void SetLoadEncountersForActiveEOCToViewBag(PatientDataOut patient, int episodeOfCareId)
        {
            if (episodeOfCareId != 0)
            {
                EpisodeOfCareDataOut episodeOfCareDataOut = patient.EpisodeOfCares.Find(x => x.Id.Equals(episodeOfCareId));
                if (episodeOfCareDataOut != null)
                {
                    episodeOfCareDataOut.Encounters = this.encounterBLL.GetAllByEocId(episodeOfCareId);
                }
                ViewBag.EpisodeOfCareId = episodeOfCareId;
            }
        }

        private void SetPatientInfoPropertiesToViewBag()
        {
            SetIdentifierTypesToViewBag();
            SetGenderTypesToViewBag();
            SetTelecomViewBags(false);
            ViewBag.PatientLanguages = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.Language);
            ViewBag.MaritalStatuses = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.MaritalStatus);
            ViewBag.ContactRelationships = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.ContactRelationship);
            ViewBag.ContactRoles = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.ContactRole);
            ViewBag.Citizenships = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.Citizenship);
            ViewBag.AddressTypes = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.AddressType);
        }

        private void SetIdentifierTypesToViewBag()
        {
            ViewBag.IdentifierTypes = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.PatientIdentifierType);
            ViewBag.IdentifierUseTypes = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.IdentifierUseType);
        }

        private async Task SetPatientListsViewBag(PatientFilterDataIn dataIn, List<PatientDataOut> patients)
        {
            if(dataIn.PatientListId.HasValue && dataIn.PatientListId.Value > 0)
            {
                ViewBag.CurrentPatientList = new PatientListFilterDataIn() { 
                    PatientListId = dataIn.PatientListId.Value, 
                    PatientListName = dataIn.PatientListName,
                    ListWithSelectedPatients = dataIn.ListWithSelectedPatients
                };
            }
            else
            {
                Dictionary<int, IEnumerable<PatientListDTO>> listsAvailableForPatients = await patientListBLL.GetListsAvailableForPatients(patients, userCookieData.Id).ConfigureAwait(false);
                ViewBag.ListsAvailableForPatients = listsAvailableForPatients;
            }
        }

        #endregion /ViewBags
    }
}