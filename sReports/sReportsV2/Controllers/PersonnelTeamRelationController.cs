using Hl7.FhirPath.Sprache;
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
using sReportsV2.SqlDomain.Interfaces;
using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.Extensions.Configuration;

namespace sReportsV2.Controllers
{
    public class PersonnelTeamRelationController : BaseController
    {
        private readonly IPersonnelTeamRelationDAL personnelTeamRelationDAL;
        private readonly IPersonnelTeamRelationBLL personnelTeamRelationBLL;
        private readonly IPersonnelTeamBLL personnelTeamBLL;

        public PersonnelTeamRelationController(IPersonnelTeamRelationDAL personnelTeamRelationDAL, 
            IPersonnelTeamRelationBLL personnelTeamRelationBLL, 
            IPersonnelTeamBLL personnelTeamBLL,             
            IHttpContextAccessor httpContextAccessor, 
            IServiceProvider serviceProvider, 
            IConfiguration configuration,
            IAsyncRunner asyncRunner) : 
            base(httpContextAccessor, serviceProvider, configuration, asyncRunner)
        {
            this.personnelTeamRelationDAL = personnelTeamRelationDAL;
            this.personnelTeamRelationBLL = personnelTeamRelationBLL;
            this.personnelTeamBLL = personnelTeamBLL;
        }

        [SReportsAuthorize(Permission = PermissionNames.Update)]
        [SReportsAuditLog]
        public ActionResult GetAddTeamMemberView(int personnelTeamId)
        {
            ViewBag.PersonnelTeamId = personnelTeamId;
            ViewBag.PersonnelTeamRelationshipType = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.PersonnelTeamRelationshipType);
            return PartialView("~/Views/PersonnelTeam/PersonnelTeamRelation/AddMembers.cshtml");
        }

        [SReportsAuthorize(Permission = PermissionNames.Update)]
        [SReportsAuditLog]
        public ActionResult GetEditTeamMemberView(int personnelTeamRelationId)
        {
            PersonnelTeamRelationDataOut personnelTeamRelationDataOut = personnelTeamRelationBLL.GetById(personnelTeamRelationId);
            ViewBag.PersonnelTeamRelationshipType = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.PersonnelTeamRelationshipType);
            return PartialView("~/Views/PersonnelTeam/PersonnelTeamRelation/EditMember.cshtml", personnelTeamRelationDataOut);
        }

        [SReportsAuthorize(Permission = PermissionNames.Delete)]
        [SReportsAuditLog]
        public ActionResult GetDeleteTeamMemberView(int personnelTeamRelationId)
        {
            PersonnelTeamRelationDataOut personnelTeamRelationDataOut = personnelTeamRelationBLL.GetById(personnelTeamRelationId);
            return PartialView("~/Views/PersonnelTeam/PersonnelTeamRelation/DeleteMember.cshtml", personnelTeamRelationDataOut);
        }

        [SReportsAuthorize(Permission = PermissionNames.Update)]
        [SReportsAuditLog]
        public ActionResult GetNewMemberForm(int personnelTeamId)
        {
            ViewBag.PersonnelTeamId = personnelTeamId;
            ViewBag.PersonnelTeamRelationshipType = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.PersonnelTeamRelationshipType);
            return PartialView("~/Views/PersonnelTeam/PersonnelTeamRelation/NewMemberForm.cshtml");
        }

        [SReportsAuthorize(Permission = PermissionNames.Update)]
        [SReportsAuditLog]
        [HttpPost]
        public ActionResult AddMembers(List<PersonnelTeamRelationDataIn> personnelTeamRelationDataIns)
        {
            personnelTeamRelationBLL.InsertMany(personnelTeamRelationDataIns);
            return NoContent();
        }

        [SReportsAuthorize(Permission = PermissionNames.Update)]
        [SReportsAuditLog]
        [HttpPost]
        public ActionResult Edit(PersonnelTeamRelationDataIn personnelTeamRelationDataIn)
        {
            personnelTeamRelationBLL.InsertOrUpdate(personnelTeamRelationDataIn);
            return NoContent();
        }

        [SReportsAuthorize(Permission = PermissionNames.Delete)]
        [SReportsAuditLog]
        [HttpDelete]
        public ActionResult Delete(int personnelTeamRelationId)
        {
            personnelTeamRelationBLL.Delete(personnelTeamRelationId);
            return NoContent();
        }

        [SReportsAuthorize(Permission = PermissionNames.View)]
        [SReportsAuditLog]
        public ActionResult GetNameAutocompleteData(AutocompleteDataIn dataIn, int personnelTeamId)
        {
            var result = personnelTeamRelationBLL.GetNameForAutocomplete(dataIn, personnelTeamId);
            return Json(result);
        }

        [SReportsAuthorize(Permission = PermissionNames.View)]
        [SReportsAuditLog]
        public ActionResult GetPersonnelTeamRelationshipTypeCodes()
        {
            var result = personnelTeamRelationBLL.GetPersonnelTeamRelationshipTypeCodes(userCookieData.ActiveLanguage, SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.PersonnelTeamRelationshipType));
            return Json(result);
        }

        [SReportsAuthorize(Permission = PermissionNames.View)]
        [SReportsAuditLog]
        public ActionResult GetSinglePersonnelTeam(PersonnelTeamRelationFilterDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            PaginationDataOut<PersonnelTeamRelationDataOut, DataIn> paginationDataOut = personnelTeamRelationBLL.GetAllFiltered(dataIn);
            
            SetViewBag(dataIn.PersonnelTeamId);

            return PartialView("~/Views/PersonnelTeam/PersonnelTeamRelation/SinglePersonnelTeam.cshtml", paginationDataOut);
        }

        [SReportsAuthorize(Permission = PermissionNames.View)]
        [SReportsAuditLog]
        public ActionResult ReloadSinglePersonnelTeamTable(PersonnelTeamRelationFilterDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            PaginationDataOut<PersonnelTeamRelationDataOut, DataIn> paginationDataOut = personnelTeamRelationBLL.GetAllFiltered(dataIn);

            SetViewBag(dataIn.PersonnelTeamId);

            return PartialView("~/Views/PersonnelTeam/PersonnelTeamRelation/SinglePersonnelTeamTable.cshtml", paginationDataOut);
        }

        [SReportsAuthorize(Permission = PermissionNames.Update)]
        [SReportsAuditLog]
        public ActionResult GetLeaderCodeId()
        {
            return Json(
                SingletonDataContainer.Instance.GetCodeId((int)CodeSetList.PersonnelTeamRelationshipType, ResourceTypes.TeamLeader));
        }

        private void SetViewBag(int personnelTeamId)
        {
            PersonnelTeamDataOut personnelTeamDataOut = personnelTeamBLL.GetById(personnelTeamId);
            ViewBag.PersonnelTeamName = personnelTeamDataOut.Name;
            ViewBag.PersonnelTeamId = personnelTeamDataOut.PersonnelTeamId;
            ViewBag.PersonnelTeamRelationshipType = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.PersonnelTeamRelationshipType);
            ViewBag.LeaderCodeId = SingletonDataContainer.Instance.GetCodeId((int)CodeSetList.PersonnelTeamRelationshipType, ResourceTypes.TeamLeader);
        }
    }
}