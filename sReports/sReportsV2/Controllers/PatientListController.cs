using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Cache.Singleton;
using sReportsV2.Common.Constants;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Extensions;
using sReportsV2.DTOs.Autocomplete;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Common.DataOut;
using sReportsV2.DTOs.DTOs.PatientList;
using sReportsV2.DTOs.DTOs.PatientList.DataIn;
using sReportsV2.DTOs.Pagination;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace sReportsV2.Controllers
{
    public class PatientListController : BaseController
    {
        private readonly IPatientListBLL patientListBLL;

        public PatientListController(IPatientListBLL patientListBLL,             
            IHttpContextAccessor httpContextAccessor, 
            IServiceProvider serviceProvider, 
            IConfiguration configuration, 
            IAsyncRunner asyncRunner) : 
            base(httpContextAccessor, serviceProvider, configuration, asyncRunner)   
        {
            this.patientListBLL = patientListBLL;
        }

        [SReportsAuthorize(Permission = PermissionNames.ViewPatientList, Module = ModuleNames.Patients)]
        [SReportsAuditLog]
        public async Task<ActionResult> GetAll(PatientListFilterDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            dataIn.PersonnelId = userCookieData.Id;
            return PartialView(await patientListBLL.GetAll(dataIn).ConfigureAwait(false));
        }

        [SReportsAuthorize(Permission = PermissionNames.ViewPatientList, Module = ModuleNames.Patients)]
        [SReportsAuditLog]
        public async Task<ActionResult> GetPatientList(int id)
        {
            return Json(await patientListBLL.GetById(id).ConfigureAwait(false));
        }

        [SReportsAuthorize(Permission = PermissionNames.CreatePatientList, Module = ModuleNames.Patients)]
        [SReportsAuditLog]
        public ActionResult Create()
        {
            SetEpisodeOfCareAndEncounterViewBags();
            SetPersonnelViewBags();
            return PartialView("PatientList", new PatientListDTO());
        }

        [HttpPost]
        [SReportsAuthorize(Permission = PermissionNames.CreatePatientList, Module = ModuleNames.Patients)]
        [SReportsAuditLog]
        public async Task<ActionResult> Create(PatientListDTO patientListDTO)
        {
            await patientListBLL.Create(patientListDTO).ConfigureAwait(false);
            return StatusCode(StatusCodes.Status201Created);
        }
        
        [SReportsAuthorize(Permission = PermissionNames.UpdatePatientList, Module = ModuleNames.Patients)]
        [SReportsAuditLog]
        public async Task<ActionResult> Edit(int? id)
        {
            var patientList = await patientListBLL.GetById(id).ConfigureAwait(false);
            if (patientList == null)
            {
                return NotFound();
            }
            SetEpisodeOfCareAndEncounterViewBags();
            SetPersonnelViewBags();
            return PartialView("PatientList", patientList);
        }
        
        [HttpPost]
        [SReportsAuthorize(Permission = PermissionNames.UpdatePatientList, Module = ModuleNames.Patients)]
        [SReportsAuditLog]
        public async Task<ActionResult> Edit(PatientListDTO patientListDTO)
        {
            if (patientListDTO!= null && patientListDTO.PatientListId > 0)
            {
                await patientListBLL.Edit(patientListDTO).ConfigureAwait(false);
                return Ok();
            }
            return NotFound();
        }

        [HttpDelete]
        [SReportsAuthorize(Permission = PermissionNames.DeletePatientList, Module = ModuleNames.Patients)]
        [SReportsAuditLog]
        public async Task<ActionResult> Delete(int? id)
        {
            await patientListBLL.Delete(id).ConfigureAwait(false);
            return NoContent();
        }

        [SReportsAuthorize(Permission = PermissionNames.ViewPatientList, Module = ModuleNames.Patients)]
        [SReportsAuditLog]
        public async Task<ActionResult> GetAutoCompleteName(AutocompleteDataIn dataIn)
        {
            return Json(await patientListBLL.GetAutoCompleteName(dataIn).ConfigureAwait(false));
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Administration)]
        [SReportsAuditLog]
        public async Task<ActionResult> ReloadPersonnelTable(PatientListFilterDataIn dataIn)
        {
            PaginationDataOut<UserDataOut, DataIn> result = await patientListBLL.ReloadPersonnelTable(dataIn).ConfigureAwait(false);
            ViewBag.Container = (dataIn != null && dataIn.LoadSelectedPersonnel.HasValue && dataIn.LoadSelectedPersonnel.Value) ? "SelectedPersonnelTable" : "PersonnelToSelectTable";
            SetPersonnelViewBags();
            return PartialView("~/Views/PatientList/Personnel/PatientListPersonnelTable.cshtml", result);
        }

        [SReportsAuthorize(Permission = PermissionNames.AddPatientListUsers, Module = ModuleNames.Patients)]
        [SReportsAuditLog]
        [HttpPost]
        public async Task<ActionResult> AddPersonnels(List<PatientListPersonnelRelationDTO> patientListPersonnelRelationDTOs)
        {
            await patientListBLL.AddPersonnelRelations(patientListPersonnelRelationDTOs).ConfigureAwait(false);
            return Ok();
        }

        [SReportsAuthorize(Permission = PermissionNames.RemovePatientListUsers, Module = ModuleNames.Patients)]
        [SReportsAuditLog]
        [HttpDelete]
        public async Task<ActionResult> RemovePersonnel(int patientListId, int personnelId)
        {
            await patientListBLL.RemovePersonnelRelation(patientListId, personnelId).ConfigureAwait(false);
            return NoContent();
        }

        [SReportsAuthorize(Permission = PermissionNames.AddPatientToPatientList, Module = ModuleNames.Patients)]
        [SReportsAuditLog]
        [HttpPost]
        public async Task<ActionResult> AddPatient(PatientListPatientRelationDTO patientListPatientRelationDTO) 
        {
            await patientListBLL.AddPatientRelations(patientListPatientRelationDTO).ConfigureAwait(false);
            return Ok();
        }

        [SReportsAuthorize(Permission = PermissionNames.RemovePatientFromPatientList, Module = ModuleNames.Patients)]
        [SReportsAuditLog]
        [HttpDelete]
        public async Task<ActionResult> RemovePatient(PatientListPatientRelationDTO patientListPatientRelationDTO)
        {
            await patientListBLL.RemovePatientRelation(patientListPatientRelationDTO).ConfigureAwait(false);
            return NoContent();
        }

        private void SetPersonnelViewBags()
        {
            ViewBag.Occupations = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.Occupation);
        }
    }
} 