using AutoMapper;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.DTOs.Organization;
using System.Collections.Generic;
using sReportsV2.DTOs.Autocomplete;
using sReportsV2.Cache.Singleton;
using sReportsV2.DTOs.Organization.DataIn;
using sReportsV2.DTOs.Organization.DataOut;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Constants;
using System.Threading.Tasks;
using sReportsV2.DTOs.DTOs.Organization.DataOut;
using sReportsV2.DTOs.DTOs.Organization.DataIn;
using sReportsV2.DTOs.Common.DTO;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using sReportsV2.BusinessLayer.Implementations;
using sReportsV2.DTOs.DTOs.Patient.DataIn;
using sReportsV2.SqlDomain.Implementations;

namespace sReportsV2.Controllers
{
    public class OrganizationController : BaseController
    {
        private readonly IOrganizationBLL organizationBLL;
        private readonly IMapper Mapper;

        public OrganizationController(IOrganizationBLL organizationBLL, 
            IHttpContextAccessor httpContextAccessor, 
            IServiceProvider serviceProvider, 
            IConfiguration configuration, 
            IMapper mapper, 
            IAsyncRunner asyncRunner) : 
            base(httpContextAccessor, serviceProvider, configuration, asyncRunner)
        {
            this.organizationBLL = organizationBLL;
            this.Mapper = mapper;
        }

        #region CRUD

        [SReportsAuthorize(Permission = PermissionNames.Create, Module = ModuleNames.Administration)]
        public ActionResult Create()
        {
            SetOrganizationViewBags();
            return View("Organization");
        }

        public ActionResult GetById(int? organizationId)
        {
            if (organizationId.HasValue)
            {
                OrganizationDataOut organization = organizationBLL.GetOrganizationById(organizationId.Value);
                return Json(new { id = organization.Id, text = organization.Name });
            }
            else
            {
                return Json(new { });
            }

        }

        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.Administration)]
        public ActionResult Edit(int organizationId)
        {
            return GetViewEditResponse(organizationId);
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Administration)]
        public ActionResult View(int organizationId)
        {
            return GetViewEditResponse(organizationId);
        }

        [SReportsAuthorize(Permission = PermissionNames.Create, Module = ModuleNames.Administration)]
        [SReportsAuditLog]
        [HttpPost]
        [SReportsModelStateValidate]
        public ActionResult Create(OrganizationDataIn organization)
        {
            return CreateOrEdit(organization);
        }

        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.Administration)]
        [SReportsAuditLog]
        [HttpPost]
        [SReportsModelStateValidate]
        public ActionResult Edit(OrganizationDataIn organization)
        {
            return CreateOrEdit(organization);
        }

        [SReportsAuthorize(Permission = PermissionNames.Create, Module = ModuleNames.Administration)]
        [SReportsAuditLog]
        [HttpPost]
        [SReportsModelStateValidate]
        public async Task<ActionResult> CreateIdentifier(OrganizationIdentifierDataIn identifierDataIn)
        {
            return await CreateOrEdit(identifierDataIn).ConfigureAwait(false);
        }

        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.Administration)]
        [SReportsAuditLog]
        [HttpPost]
        [SReportsModelStateValidate]
        public async Task<ActionResult> EditIdentifier(OrganizationIdentifierDataIn identifierDataIn)
        {
            return await CreateOrEdit(identifierDataIn).ConfigureAwait(false);
        }

        [SReportsAuthorize(Permission = PermissionNames.Create, Module = ModuleNames.Administration)]
        [SReportsAuditLog]
        [HttpPost]
        [SReportsModelStateValidate]
        public async Task<ActionResult> CreateTelecom(OrganizationTelecomDataIn telecomDataIn)
        {
            return await CreateOrEdit(telecomDataIn).ConfigureAwait(false);
        }

        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.Administration)]
        [SReportsAuditLog]
        [HttpPost]
        [SReportsModelStateValidate]
        public async Task<ActionResult> EditTelecom(OrganizationTelecomDataIn telecomDataIn)
        {
            return await CreateOrEdit(telecomDataIn).ConfigureAwait(false);
        }

        [SReportsAuthorize(Permission = PermissionNames.Delete, Module = ModuleNames.Administration)]
        [HttpDelete]
        [SReportsAuditLog]
        public ActionResult Delete(OrganizationDataIn organizationDataIn)
        {
            organizationBLL.Delete(organizationDataIn);
            return NoContent();
        }

        [SReportsAuthorize(Permission = PermissionNames.Delete, Module = ModuleNames.Administration)]
        [HttpDelete]
        [SReportsAuditLog]
        public async Task<ActionResult> DeleteIdentifier(OrganizationIdentifierDataIn organizationIdentifierDataIn)
        {
            await organizationBLL.Delete(organizationIdentifierDataIn).ConfigureAwait(false);
            return NoContent();
        }

        [SReportsAuthorize(Permission = PermissionNames.Delete, Module = ModuleNames.Administration)]
        [HttpDelete]
        [SReportsAuditLog]
        public async Task<ActionResult> DeleteTelecom(OrganizationTelecomDataIn patientTelecomDataIn)
        {
            await organizationBLL.Delete(patientTelecomDataIn).ConfigureAwait(false);
            return NoContent();
        }
        #endregion /CRUD


        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Administration)]
        public ActionResult GetAll(OrganizationFilterDataIn dataIn)
        {
            SetCountryNameIfFilterByCountryIsIncluded(dataIn);
            ViewBag.FilterData = dataIn;
            SetOrganizationViewBags();
            return View();
        }

        [SReportsAuthorize]
        public ActionResult ReloadTable(OrganizationFilterDataIn dataIn)
        {
            var result = organizationBLL.ReloadTable(dataIn);
            SetOrganizationViewBags();
            return PartialView("OrganizationEntryTable", result);
        }

        public ActionResult GetOrganizationTelecoms(int organizationId)
        {
            var result = organizationBLL.GetOrganizationForEdit(organizationId);
            SetOrganizationViewBags();
            ViewBag.ReadOnly = false;
            return PartialView("OrganizationTelecoms", result);
        }

        public ActionResult GetOrganizationIdentifiers(int organizationId)
        {
            var result = organizationBLL.GetOrganizationForEdit(organizationId);
            SetOrganizationViewBags();
            ViewBag.ReadOnly = false;
            return PartialView("OrganizationIdentifierTable", result.Identifiers);
        }

        [SReportsAuthorize]
        public ActionResult ReloadOrgCommunicationTable(OrganizationFilterDataIn dataIn)
        {
            var result = organizationBLL.GetOrgCommunicationByOrgId(dataIn);
            SetOrganizationViewBags();
            return PartialView("OrganizationCommunicationEntityTable", result);
        }

        public ActionResult ExistIdentifier(OrganizationIdentifierDataIn dataIn)
        {
            return Json(!organizationBLL.ExistEntity(dataIn));
        }

        public ActionResult ReloadHierarchy(int? parentId)
        {
            if (parentId == null)
            {
                return PartialView("OrganizationHierarchy");
            }
            
            return PartialView("OrganizationHierarchy", organizationBLL.ReloadHierarchy(parentId));
        }

        public ActionResult GetAutocompleteData(AutocompleteDataIn dataIn)
        {
            var result = organizationBLL.GetDataForAutocomplete(dataIn);
            return Json(result);
        }

        public ActionResult GetOrganizationValues(OrganizationFilterDataIn dataIn)
        {
            var result = organizationBLL.ReloadTable(dataIn);
            return PartialView("OrganizationValues", result.Data);
        }

        public ActionResult GetUsersByOrganizationCount()
        {
            return PartialView(Mapper.Map<List<OrganizationUsersCountDataOut>>(organizationBLL.GetOrganizationUsersCount(null, null)));
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Administration)]
        public async Task<ActionResult> ShowOrganizationCommunicationModal(int orgCommunicationEntityId, bool readOnly)
        {
            OrganizationCommunicationEntityDataOut result = await organizationBLL.GetOrgCommunicationEntityIdByIdAsync(orgCommunicationEntityId).ConfigureAwait(false) ?? new OrganizationCommunicationEntityDataOut();
            SetOrganizationViewBags();
            ViewBag.IsReadOnly = readOnly;
            return PartialView("OrganizationCommunicationModal", result);
        }

        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.Administration)]
        [SReportsAuditLog]
        [HttpPost]
        public async Task<ActionResult> CreateOrganizationCommunication(OrganizationCommunicationEntityDataIn dataIn)
        {
            int orgCommunicationEntityId = await organizationBLL.InsertOrganizationCommunication(dataIn).ConfigureAwait(false);

            return Json(new CreateResponseResult { Id = orgCommunicationEntityId });
        }


        #region CRUD
        private ActionResult GetViewEditResponse(int organizationId)
        {
            var result = organizationBLL.GetOrganizationForEdit(organizationId);
            SetOrganizationViewBags();
            return View("Organization", result);
        }

        private JsonResult CreateOrEdit(OrganizationDataIn organization)
        {
            return Json(organizationBLL.Insert(organization));
        }

        private async Task<JsonResult> CreateOrEdit(OrganizationIdentifierDataIn identifierDataIn)
        {
            ResourceCreatedDTO resourceCreatedDTO = await organizationBLL.InsertOrUpdate(identifierDataIn).ConfigureAwait(false);

            if (identifierDataIn.Id == 0)
            {
                Response.StatusCode = 201;
            }
            return Json(resourceCreatedDTO);
        }

        private async Task<JsonResult> CreateOrEdit(OrganizationTelecomDataIn organizationTelecomDataIn)
        {
            ResourceCreatedDTO resourceCreatedDTO = await organizationBLL.InsertOrUpdate(organizationTelecomDataIn).ConfigureAwait(false);

            if (organizationTelecomDataIn.Id == 0)
            {
                Response.StatusCode = 201;
            }
            return Json(resourceCreatedDTO);
        }
        #endregion /CRUD

        private void SetOrganizationViewBags()
        {
            ViewBag.OrganizationTypes = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.OrganizationType);
            ViewBag.IdentifierTypes = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.OrganizationIdentifierType);
            ViewBag.IdentifierUseTypes = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.IdentifierUseType);
            ViewBag.TeamTypes = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.TeamType);
            ViewBag.PrimaryCommunicationSystems = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.CommunicationSystem);
            ViewBag.OrgCommunicationEntities = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.OrganizationCommunicationEntity);
            ViewBag.TimeZoneList = TimeZoneInfo.GetSystemTimeZones();
            ViewBag.ClinicalDomains = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.ClinicalDomain);
            SetTelecomViewBags(true);
        }
    }
}