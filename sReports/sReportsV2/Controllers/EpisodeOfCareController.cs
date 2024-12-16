using sReportsV2.Common.CustomAttributes;
using sReportsV2.Cache.Singleton;
using sReportsV2.DTOs.EpisodeOfCare;
using System.Linq;
using System.Net;
using sReportsV2.Common.Enums;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Constants;
using System.Collections.Generic;
using sReportsV2.Common.Extensions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.Extensions.Configuration;

namespace sReportsV2.Controllers
{
    public class EpisodeOfCareController : BaseController
    {
        private readonly IEpisodeOfCareBLL episodeOfCareBLL;

        public EpisodeOfCareController(IEpisodeOfCareBLL episodeOfCareBLL,             
            IHttpContextAccessor httpContextAccessor, 
            IServiceProvider serviceProvider,
            IConfiguration configuration, 
            IAsyncRunner asyncRunner) : 
            base(httpContextAccessor, serviceProvider, configuration, asyncRunner)
        {
            this.episodeOfCareBLL = episodeOfCareBLL;
        }

        [SReportsAuthorize(Permission = PermissionNames.AddEpisodeOfCare, Module = ModuleNames.Patients)]
        public async Task<ActionResult> AddEpisodeOfCare(int episodeOfCareId, bool isReadOnlyViewMode)
        {
            return await GetEditViewEpisodeOfCare(episodeOfCareId, isReadOnlyViewMode).ConfigureAwait(false);
        }

        [SReportsAuthorize(Permission = PermissionNames.UpdateEpisodeOfCare, Module = ModuleNames.Patients)]
        public async Task<ActionResult> UpdateEpisodeOfCare(int episodeOfCareId, bool isReadOnlyViewMode)
        {
            return await GetEditViewEpisodeOfCare(episodeOfCareId, isReadOnlyViewMode).ConfigureAwait(false);
        }

        [SReportsAuthorize(Permission = PermissionNames.ViewEpisodeOfCare, Module = ModuleNames.Patients)]
        public async Task<ActionResult> ViewEpisodeOfCare(int episodeOfCareId, bool isReadOnlyViewMode)
        {
            return await GetEditViewEpisodeOfCare(episodeOfCareId, true).ConfigureAwait(false);
        }


        [SReportsAuthorize(Permission = PermissionNames.AddEpisodeOfCare, Module = ModuleNames.Patients)]
        [SReportsAuditLog]
        [HttpPost]
        public async Task<ActionResult> Create(EpisodeOfCareDataIn episodeOfCare)
        {
            return await CreateOrEdit(episodeOfCare).ConfigureAwait(false);
        }

        [SReportsAuthorize(Permission = PermissionNames.UpdateEpisodeOfCare, Module = ModuleNames.Patients)]
        [SReportsAuditLog]
        [HttpPost]
        public async Task<ActionResult> Edit(EpisodeOfCareDataIn episodeOfCare)
        {
            return await CreateOrEdit(episodeOfCare).ConfigureAwait(false);
        }

        [SReportsAuthorize(Permission = PermissionNames.RemoveEpisodeOfCare, Module = ModuleNames.Patients)]
        [HttpDelete]
        [SReportsAuditLog]
        public async Task<ActionResult> DeleteEOC(int eocId)
        {
            var task = episodeOfCareBLL.DeleteAsync(eocId);
            await task.ConfigureAwait(false);

            return NoContent();
        }

        [SReportsAuthorize(Permission = PermissionNames.ViewEpisodeOfCare, Module = ModuleNames.Patients)]
        public async Task<ActionResult> EditFromEOC(int episodeOfCareId)
        {
            EpisodeOfCareDataOut episodeOfCareDataOut = await episodeOfCareBLL.GetByIdAsync(episodeOfCareId, userCookieData.ActiveLanguage)
                .ConfigureAwait(false);
            SetEpisodeOfCareAndEncounterViewBags();

            ViewBag.PatientId = episodeOfCareDataOut.PatientId;
            ViewBag.EocId = episodeOfCareDataOut.Id;
            ViewBag.EncounterId = episodeOfCareDataOut.Encounters.LastOrDefault()?.Id;
            ViewBag.Type = "EOC";

            return PartialView("EditFromEOC", episodeOfCareDataOut);
        }

        public ActionResult ShowEpisodeOfCareContent(bool isReadOnlyViewMode)
        {
            ViewBag.ActiveEOC = 0;
            SetEpisodeOfCareViewBags();
            SetReadOnlyAndDisabledViewBag(isReadOnlyViewMode);
            return PartialView("ShowEOCContent");
        }

        public ActionResult ShowEpisodeOfCareFilterGroup(bool isReadOnlyViewMode)
        {
            ViewBag.ActiveEOC = 0;
            SetEpisodeOfCareViewBags();
            SetReadOnlyAndDisabledViewBag(isReadOnlyViewMode);
            return PartialView("EpisodeOfCareFilterGroup");
        }

        [SReportsAuthorize(Permission = PermissionNames.ViewEpisodeOfCare, Module = ModuleNames.Patients)]
        public async Task<ActionResult> EncounterData(int episodeOfCareId, bool isReadOnlyViewMode)
        {
            EpisodeOfCareDataOut episodeOfCareDataOut = await episodeOfCareBLL.GetByIdAsync(episodeOfCareId, userCookieData.ActiveLanguage)
                .ConfigureAwait(false);
            SetEpisodeOfCareAndEncounterViewBags();
            SetReadOnlyAndDisabledViewBag(isReadOnlyViewMode);

            if (episodeOfCareDataOut.Encounters.Count > 0)
                return PartialView("EncounterData", episodeOfCareDataOut);
            else
                return PartialView("NoEncounters");
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Patients)]
        public async Task<ActionResult> ReloadEOCFromPatient(EpisodeOfCareDataIn episodeOfCare, bool isReadOnlyViewMode)
        {
            episodeOfCare = Ensure.IsNotNull(episodeOfCare, nameof(episodeOfCare));
            List<EpisodeOfCareDataOut> episodesOfCareDataOut = await episodeOfCareBLL.GetByPatientIdAsync(episodeOfCare, userCookieData.ActiveLanguage)
                .ConfigureAwait(false);
            SetEpisodeOfCareViewBags();
            SetReadOnlyAndDisabledViewBag(isReadOnlyViewMode);
            return PartialView("PatientEOC", episodesOfCareDataOut);
        }

        [SReportsAuthorize(Permission = PermissionNames.ViewEpisodeOfCare, Module = ModuleNames.Patients)]
        public async Task<ActionResult> NoDocumentIsSelected()
        {
            return PartialView("NoDocumentIsSelected");
        }

        [SReportsAuthorize(Permission = PermissionNames.ViewEpisodeOfCare, Module = ModuleNames.Patients)]
        public async Task<ActionResult> GetActiveBreadcrumbValue(int episodeOfCareId, int? encounterId = null)
        {
            var episodeOfCareDataOut = await episodeOfCareBLL.GetByIdAsync(episodeOfCareId, userCookieData.ActiveLanguage).ConfigureAwait(false);
            SetEpisodeOfCareAndEncounterViewBags();

            return Json(episodeOfCareDataOut.ConvertEOCAndEncounterTypeCDToDisplayName(ViewBag.EpisodeOfCareTypes, ViewBag.EncounterTypes, userCookieData.ActiveLanguage, encounterId));
        }

        private async Task<ActionResult> CreateOrEdit(EpisodeOfCareDataIn episodeOfCare)
        {
            episodeOfCare = Ensure.IsNotNull(episodeOfCare, nameof(episodeOfCare));
            await episodeOfCareBLL.InsertOrUpdateAsync(episodeOfCare, userCookieData)
                .ConfigureAwait(false);

            return StatusCode(StatusCodes.Status201Created);
        }

        private async Task<ActionResult> GetEditViewEpisodeOfCare(int episodeOfCareId, bool isReadOnlyViewMode)
        {
            var data = await episodeOfCareBLL.GetByIdAsync(episodeOfCareId, userCookieData.ActiveLanguage);
            SetEpisodeOfCareViewBags();
            SetReadOnlyAndDisabledViewBag(isReadOnlyViewMode);
            return PartialView("AddEocModal", data);
        }
    }
}