using sReportsV2.Common.CustomAttributes;
using sReportsV2.DTOs.Encounter;
using System;
using System.Collections.Generic;
using sReportsV2.Cache.Singleton;
using System.Threading.Tasks;
using sReportsV2.Common.Extensions;
using sReportsV2.Common.Enums;
using sReportsV2.BusinessLayer.Interfaces;
using AutoMapper;
using sReportsV2.DTOs.Form.DataOut;
using sReportsV2.Common.Constants;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Pagination;
using sReportsV2.DTOs.DTOs.Encounter.DataOut;
using sReportsV2.DTOs.CodeEntry.DataOut;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.Cache.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using sReportsV2.Common.Exceptions;

namespace sReportsV2.Controllers
{
    public class EncounterController : BaseController
    {
        private readonly IEncounterBLL encounterBLL;
        private readonly IMapper Mapper;

        public EncounterController(IEncounterBLL encounterBLL, 
            IMapper mapper,             
            IHttpContextAccessor httpContextAccessor, 
            IServiceProvider serviceProvider, 
            IConfiguration configuration, 
            IAsyncRunner asyncRunner) : 
            base(httpContextAccessor, serviceProvider, configuration, asyncRunner)
        {
            this.encounterBLL = encounterBLL;
            Mapper = mapper;
        }

        [SReportsAuthorize(Permission = PermissionNames.AddDocument, Module = ModuleNames.Patients)]
        public async Task<ActionResult> ListReferralsAndForms(int encounterId, int? episodeOfCareId)
        {
            if (episodeOfCareId == null)
            {
                throw new UserAdministrationException(StatusCodes.Status400BadRequest, TextLanguage.EocNotFound);
            }

            var result = await encounterBLL.ListReferralsAndForms(encounterId, episodeOfCareId.GetValueOrDefault(), userCookieData).ConfigureAwait(false);
            return PartialView(result);
        }

        [SReportsAuthorize(Permission = PermissionNames.AddDocument, Module = ModuleNames.Patients)]
        public async Task<ActionResult> ListForms(string condition)
        {
            var result = await encounterBLL.ListForms(condition, userCookieData).ConfigureAwait(false);
            return PartialView(result);
        }

        [SReportsAuthorize(Permission = PermissionNames.AddDocument, Module = ModuleNames.Patients)]
        public async Task<ActionResult> GetSuggestedForms()
        {
            var suggestedForms = await encounterBLL.GetSuggestedForms(userCookieData.SuggestedForms).ConfigureAwait(false);
            return PartialView("SuggestedForms", Mapper.Map<List<FormDataOut>>(suggestedForms));
        }

        [SReportsAuthorize(Permission = PermissionNames.RemoveEncounter, Module = ModuleNames.Patients)]
        [HttpDelete]
        public async Task<ActionResult> Delete(int id)
        {
            await encounterBLL.DeleteAsync(id).ConfigureAwait(false);
            return NoContent();
        }

        [SReportsAuthorize(Permission = PermissionNames.ViewEpisodeOfCare, Module = ModuleNames.Patients)]
        public async Task<ActionResult> ShowEncounterDetails(int encounterId, bool isReadOnlyViewMode)
        {
            var organizationId = userCookieData.ActiveOrganization;
            EncounterDataOut encounterDataOut = await encounterBLL.GetByEncounterIdAsync(encounterId, organizationId, false).ConfigureAwait(false);
            SetReadOnlyAndDisabledViewBag(isReadOnlyViewMode);
            ViewBag.TaskTypes = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.TaskType);
            ViewBag.TaskStatuses = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.TaskStatus);
            return PartialView("EncounterDocumentations", encounterDataOut);
        }

        [SReportsAuthorize(Permission = PermissionNames.ViewEpisodeOfCare, Module = ModuleNames.Patients)]
        public async Task<ActionResult> ShowEncounterTypeEncounters(int encounterTypeId, int episodeOfCareId, bool isReadOnlyViewMode) 
        {
            List<EncounterDataOut> encountersDataOut = await encounterBLL.GetEncountersByTypeAndEocIdAsync(encounterTypeId, episodeOfCareId).ConfigureAwait(false);
            SetReadOnlyAndDisabledViewBag(isReadOnlyViewMode);
            return PartialView("EocEncounter", encountersDataOut);
        }

        [SReportsAuthorize(Permission = PermissionNames.AddEncounter, Module = ModuleNames.Patients)]
        [SReportsAuditLog]
        [HttpPost]
        [SReportsModelStateValidate]
        public ActionResult Create(EncounterDataIn encounter)
        {
            return CreateOrEdit(encounter);
        }

        [SReportsAuthorize(Permission = PermissionNames.UpdateEncounter, Module = ModuleNames.Patients)]
        [SReportsAuditLog]
        [HttpPost]
        [SReportsModelStateValidate]
        public ActionResult Edit(EncounterDataIn encounter)
        {
            return CreateOrEdit(encounter);
        }

        [SReportsAuthorize(Permission = PermissionNames.UpdateEncounter, Module = ModuleNames.Patients)]
        public async Task<ActionResult> EditEncounter(int encounterId, bool isReadOnlyViewMode)
        {
            return await GetEditViewEncounter(encounterId, isReadOnlyViewMode).ConfigureAwait(false);
        }

        [SReportsAuthorize(Permission = PermissionNames.AddEncounter, Module = ModuleNames.Patients)]
        public async Task<ActionResult> AddEncounter(int encounterId, bool isReadOnlyViewMode)
        {
            return await GetEditViewEncounter(encounterId, isReadOnlyViewMode).ConfigureAwait(false);
        }

        [SReportsAuthorize(Permission = PermissionNames.ViewEncounter, Module = ModuleNames.Patients)]
        public async Task<ActionResult> ViewEncounter(int encounterId, bool isReadOnlyViewMode)
        {
            return await GetEditViewEncounter(encounterId, true).ConfigureAwait(false);
        }

        [SReportsAuthorize(Permission = PermissionNames.ViewEncounter, Module = ModuleNames.Patients)]
        public ActionResult GetAll(EncounterFilterDataIn dataIn)
        {
            ViewBag.Genders = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.Gender);
            SetEpisodeOfCareAndEncounterViewBags();
            ViewBag.FilterData = dataIn;
            return View();
        }

        [SReportsAuthorize(Permission = PermissionNames.ViewEncounter, Module = ModuleNames.Patients)]
        public async Task<ActionResult> ReloadTable(EncounterFilterDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            PopulateGenders(dataIn);
            SetEpisodeOfCareAndEncounterViewBags();
            PopulateStatuses(dataIn);
            ViewBag.ReloadEncounterFromPatient = dataIn.ReloadEncounterFromPatient;
            SetReadOnlyAndDisabledViewBag(dataIn.IsReadOnlyViewMode);
            PaginationDataOut<EncounterViewDataOut, DataIn> result = await encounterBLL.GetAllFiltered(dataIn).ConfigureAwait(false);

            return PartialView(dataIn.ListEncountersForSelectedPatient && string.IsNullOrEmpty(dataIn.ColumnName) ? "EncountersForSelectedPatient" : "EncounterEntryTable", result);
        }

        private async Task<ActionResult> GetEditViewEncounter(int encounterId, bool isReadOnlyViewMode)
        {
            var data = await encounterBLL.GetByEncounterIdAsync(encounterId, -1, onlyEncounter: true).ConfigureAwait(false) ?? new EncounterDataOut();
            SetEncounterViewBags();
            SetReadOnlyAndDisabledViewBag(isReadOnlyViewMode);
            return PartialView("EncounterModal", data);
        }

        private JsonResult CreateOrEdit(EncounterDataIn encounter)
        {
            int encounterId = encounterBLL.InsertOrUpdate(encounter);

            return Json(new CreateResponseResult { Id = encounterId });
        }

        private void PopulateGenders(EncounterFilterDataIn dataIn)
        {
            ViewBag.Genders = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.Gender);
            string activeLanguage = ViewBag.UserCookieData.ActiveLanguage;
            foreach (CodeDataOut genderCode in ViewBag.Genders)
            {
                string genderName = genderCode.Thesaurus.GetPreferredTermByTranslationOrDefault(LanguageConstants.EN);
                if (!string.IsNullOrEmpty(genderName) && !dataIn.Genders.ContainsKey(genderName))
                {
                    dataIn.Genders.Add(genderName, new Tuple<int, string>(genderCode.Id, genderCode.Thesaurus.GetPreferredTermByTranslationOrDefault(activeLanguage)));
                }
            }
        }

        private void PopulateStatuses(EncounterFilterDataIn dataIn)
        {
            string activeLanguage = ViewBag.UserCookieData.ActiveLanguage;
            foreach (CodeDataOut statusCode in ViewBag.EncounterStatuses)
            {
                string statusName = statusCode.Thesaurus.GetPreferredTermByTranslationOrDefault(LanguageConstants.EN);
                if (!string.IsNullOrEmpty(statusName) && !dataIn.Statuses.ContainsKey(statusName))
                {
                    dataIn.Statuses.Add(statusName, new Tuple<int, string>(statusCode.Id, statusCode.Thesaurus.GetPreferredTermByTranslationOrDefault(activeLanguage)));
                }
            }
        }
    }
}