using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Constants;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Extensions;
using sReportsV2.Cache.Singleton;
using sReportsV2.DTOs.Autocomplete;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.DTOs.PersonnelTeam.DataIn;
using sReportsV2.DTOs.DTOs.PersonnelTeam.DataOut;
using sReportsV2.DTOs.Pagination;
using System.Collections.Generic;
using System.Net;
using sReportsV2.Cache.Resources;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace sReportsV2.Controllers
{
    public class PersonnelTeamController : BaseController
    {
        private readonly IPersonnelTeamBLL personnelTeamBLL;

        public PersonnelTeamController(IPersonnelTeamBLL personnelTeamBLL, 
            IHttpContextAccessor httpContextAccessor,
            IServiceProvider serviceProvider, 
            IConfiguration configuration, 
            IAsyncRunner asyncRunner) : 
            base(httpContextAccessor, serviceProvider, configuration, asyncRunner)
        {
            this.personnelTeamBLL = personnelTeamBLL;
        }

        [SReportsAuthorize(Permission = PermissionNames.Update)]
        [SReportsAuditLog]
        [HttpPost]
        public ActionResult CreateOrUpdate(PersonnelTeamDataIn personnelTeamDataIn)
        {
            personnelTeamDataIn = Ensure.IsNotNull(personnelTeamDataIn, nameof(personnelTeamDataIn));
            personnelTeamBLL.InsertOrUpdate(personnelTeamDataIn);
            return StatusCode(StatusCodes.Status201Created);
        }

        [SReportsAuthorize(Permission = PermissionNames.View)]
        [SReportsAuditLog]
        public ActionResult ReloadTable(PersonnelTeamFilterDataIn dataIn)
        {
            PaginationDataOut<PersonnelTeamDataOut, DataIn> result = personnelTeamBLL.GetAllFiltered(dataIn);
            ViewBag.TeamTypes = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.TeamType);
            return PartialView("PersonnelTeamTable", result);
        }

        [SReportsAuthorize(Permission = PermissionNames.View)]
        [SReportsAuditLog]
        public ActionResult GetNameAutocompleteData(AutocompleteDataIn dataIn, int organizationId)
        {
            var result = personnelTeamBLL.GetNameForAutocomplete(dataIn, organizationId);
            return Json(result);
        }

        [SReportsAuthorize(Permission = PermissionNames.View)]
        [SReportsAuditLog]
        public ActionResult GetTeamTypeCodes()
        {
            var result = personnelTeamBLL.GetTeamTypeCodes(userCookieData.ActiveLanguage, SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.TeamType));
            return Json(result);
        }

        [SReportsAuthorize(Permission = PermissionNames.Update)]
        [SReportsAuditLog]
        public ActionResult GetNewPersonnelTeamView()
        {
            ViewBag.TeamTypes = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.TeamType);
            ViewBag.PersonnelTeamRelationshipType = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.PersonnelTeamRelationshipType);
            return PartialView("~/Views/PersonnelTeam/AddPersonnelTeam.cshtml");
        }

        [SReportsAuthorize(Permission = PermissionNames.Update)]
        [SReportsAuditLog]
        public ActionResult GetEditView(int personnelTeamId)
        {
            PersonnelTeamDataOut result = personnelTeamBLL.GetById(personnelTeamId);
            ViewBag.TeamTypes = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.TeamType);
            ViewBag.PersonnelTeamRelationshipType = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.PersonnelTeamRelationshipType);
            return PartialView("~/Views/PersonnelTeam/EditPersonnelTeam.cshtml", result);
        }

        [SReportsAuthorize(Permission = PermissionNames.Delete)]
        [SReportsAuditLog]
        [HttpDelete]
        public ActionResult GetDeleteView(int personnelTeamId)
        {
            PersonnelTeamDataOut result = personnelTeamBLL.GetById(personnelTeamId);
            return PartialView("~/Views/PersonnelTeam/DeletePersonnelTeam.cshtml", result);
        }

        [SReportsAuthorize(Permission = PermissionNames.Delete)]
        [SReportsAuditLog]
        [HttpDelete]
        public ActionResult Delete(int personnelTeamId)
        {
            personnelTeamBLL.Delete(personnelTeamId);
            return NoContent();
        }

        [SReportsAuthorize(Permission = PermissionNames.View)]
        [SReportsAuditLog]
        public ActionResult CountTeamsPerOrganization(int organizationId)
        {
            int count = personnelTeamBLL.CountTeamsPerOrganization(organizationId);
            return Json(new Dictionary<string, int>{ { "count", count } });
        }

        [SReportsAuthorize(Permission = PermissionNames.View)]
        [SReportsAuditLog]
        public ActionResult IsNameNotUsedCheck(string personnelTeamNameInput, int organizationId, int personnelTeamId)
        {
            bool isNameUsed = personnelTeamBLL.IsNameUsedCheck(personnelTeamNameInput, organizationId, personnelTeamId);
            string returnString = isNameUsed ? 
                personnelTeamNameInput + " " + TextLanguage.Is_Already_Used + ",\n" + TextLanguage.Please_Choose_Another + " "  + TextLanguage.Name 
                : "true";
            return Json(returnString);
        }

        [SReportsAuthorize(Permission = PermissionNames.View)]
        [SReportsAuditLog]
        public ActionResult IsPersonnelUnique(int newTeamMemberNameSelect2, int personnelTeamId)
        {
            bool isPersonnelUnique = personnelTeamBLL.IsPersonnelUnique(userId: newTeamMemberNameSelect2, personnelTeamId);

            return Json(isPersonnelUnique.ToJsonString());
        }

        [SReportsAuthorize(Permission = PermissionNames.View)]
        [SReportsAuditLog]
        public ActionResult IsLeaderUnique(int newTeamMemberRoleSelect2, int personnelTeamId)
        {
            int? leaderRoleCD = SingletonDataContainer.Instance.GetCodeId((int)CodeSetList.PersonnelTeamRelationshipType, ResourceTypes.TeamLeader);

            bool isLeaderUnique = personnelTeamBLL.IsLeaderUnique(roleCD: newTeamMemberRoleSelect2, leaderRoleCD, personnelTeamId);

            return Json(isLeaderUnique.ToJsonString());
        }
    }
}