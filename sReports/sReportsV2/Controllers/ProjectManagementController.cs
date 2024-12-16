using AutoMapper;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Constants;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Extensions;
using sReportsV2.Cache.Singleton;
using sReportsV2.DTOs;
using sReportsV2.DTOs.Autocomplete;
using sReportsV2.DTOs.CodeEntry.DataOut;
using sReportsV2.DTOs.DTOs.Patient.DataIn;
using sReportsV2.DTOs.DTOs.ProjectManagement.DataIn;
using sReportsV2.DTOs.DTOs.TrialManagement;
using sReportsV2.DTOs.Form;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.Extensions.Configuration;

namespace sReportsV2.Controllers
{
    public class ProjectManagementController : BaseController
    {
        private readonly ITrialManagementBLL trialManagementBLL;
        private readonly IProjectManagementBLL projectManagementBLL;
        private readonly ICodeBLL codeBLL;
        private readonly IMapper Mapper;

        public ProjectManagementController(IProjectManagementBLL projectManagementBLL, 
            ITrialManagementBLL trialManagementBLL, 
            ICodeBLL codeBLL, 
            IMapper mapper,             
            IHttpContextAccessor httpContextAccessor, 
            IServiceProvider serviceProvider, 
            IConfiguration configuration,
            IAsyncRunner asyncRunner) : 
            base(httpContextAccessor, serviceProvider, configuration, asyncRunner)
        {
            this.projectManagementBLL = projectManagementBLL;
            this.trialManagementBLL = trialManagementBLL;
            this.codeBLL = codeBLL; 
            Mapper = mapper;
        } 

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.ProjectManagement)]
        [SReportsAuditLog]
        public ActionResult GetAll(TrialManagementFilterDataIn dataIn)
        {
            SetProjectManagementViewBags();
            ViewBag.FilterData = dataIn;
            return View();
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.ProjectManagement)]
        [SReportsAuditLog]
        public ActionResult GetAllUserProjects(TrialManagementFilterDataIn dataIn)
        {
            SetProjectManagementViewBags();
            ViewBag.FilterData = dataIn;
            return View();
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.ProjectManagement)]
        [SReportsAuditLog]
        public async Task<ActionResult> ReloadTable(ProjectFilterDataIn dataIn)
        {
            var result = await projectManagementBLL.ReloadTable(dataIn).ConfigureAwait(false);
            SetProjectManagementViewBags();
            return PartialView("ProjectTable", result);
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.ProjectManagement)]
        [SReportsAuditLog]
        public async Task<ActionResult> ReloadUserProjectTable(ProjectFilterDataIn dataIn)
        {
            var result = await projectManagementBLL.ReloadUserProjectTable(dataIn, userCookieData.Id).ConfigureAwait(false);
            SetUserProjectViewBags();
            return PartialView("ProjectTable", result);
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.ProjectManagement)]
        [SReportsAuditLog]
        public async Task<ActionResult> ReloadPersonnelTable(ProjectFilterDataIn dataIn, string tableContainer)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            ViewBag.ReadOnly = dataIn.IsReadOnly;
            ViewBag.Container = tableContainer;
            SetProjectManagementViewBags();
            var result = await projectManagementBLL.ReloadPersonnelTable(dataIn).ConfigureAwait(false);
            return PartialView("~/Views/ProjectManagement/ProjectPersonnel/ProjectPersonnelTable.cshtml", result);
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.ProjectManagement)]
        [SReportsAuditLog]
        public async Task<ActionResult> ReloadDocumentsTable(FormFilterDataIn dataIn, int projectId)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            ViewBag.ShowUserProjects = dataIn.ShowUserProjects;
            ViewBag.ReadOnly = dataIn.IsReadOnly;
            return PartialView("~/Views/ProjectManagement/ProjectDocuments/ProjectDocumentsTable.cshtml", 
                await projectManagementBLL.ReloadDocumentsTable(dataIn, projectId, userCookieData).ConfigureAwait(false));
        }

        [SReportsAuthorize(Permission = PermissionNames.Create, Module = ModuleNames.ProjectManagement)]
        [SReportsAuditLog]
        public async Task<ActionResult> Create()
        {
            SetProjectManagementViewBags();
            return await CreateOrEdit().ConfigureAwait(false);
        }

        [SReportsAuthorize(Permission = PermissionNames.Create, Module = ModuleNames.ProjectManagement)]
        [SReportsAuditLog]
        [HttpPost]
        public async Task<ActionResult> Create(ProjectDataIn project)
        {
            SetProjectManagementViewBags();
            return Json((await projectManagementBLL.InsertOrUpdate(project).ConfigureAwait(false)).ProjectId);
        }

        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.ProjectManagement)]
        [SReportsAuditLog]
        public async Task<ActionResult> Edit(int projectId)
        {
            SetProjectManagementViewBags();
            return await CreateOrEdit(projectId).ConfigureAwait(false);
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.ProjectManagement)]
        [SReportsAuditLog]
        public async Task<ActionResult> View(int projectId)
        {
            SetProjectManagementViewBags();
            return await CreateOrEdit(projectId).ConfigureAwait(false);
        }

        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.ProjectManagement)]
        [SReportsAuditLog]
        [HttpPost]
        public async Task<ActionResult> Edit(ProjectDataIn project)
        {
            SetProjectManagementViewBags();
            ViewBag.ReadOnly = false;
            return View("Project", await projectManagementBLL.InsertOrUpdate(project).ConfigureAwait(false));
        }

        [SReportsAuthorize(Permission = PermissionNames.Delete, Module = ModuleNames.ProjectManagement)]
        [SReportsAuditLog]
        [HttpDelete]
        public async Task<ActionResult> Delete(int projectId)
        {
            await projectManagementBLL.Delete(projectId).ConfigureAwait(false);
            return NoContent();
        }

        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.ProjectManagement)]
        [SReportsAuditLog]
        [HttpPost]
        public async Task<ActionResult> Archive(int clinicalTrialId)
        {
            await trialManagementBLL.Archive(clinicalTrialId).ConfigureAwait(false);
            return NoContent();
        }

        [SReportsAuthorize(Permission = PermissionNames.AddPersonnel, Module = ModuleNames.ProjectManagement)]
        [SReportsAuditLog]
        [HttpPost]
        public async Task<ActionResult> AddPersonnels(AddPersonnelToProjectDataIn dataIn)
        {
            await projectManagementBLL.AddPersonnelsToProject(dataIn.PersonnelProjects).ConfigureAwait(false);
            return Ok();
        }

        [SReportsAuthorize(Permission = PermissionNames.DeletePersonnel, Module = ModuleNames.ProjectManagement)]
        [SReportsAuditLog]
        [HttpDelete]
        public async Task<ActionResult> RemovePersonnelFromTrial(int personnelId, int projectId)
        {
            await projectManagementBLL.RemovePersonnelsFromProject(personnelId, projectId).ConfigureAwait(false);
            return NoContent();
        }

        [SReportsAuthorize(Permission = PermissionNames.AddDocument, Module = ModuleNames.ProjectManagement)]
        [SReportsAuditLog]
        [HttpPost]
        public async Task<ActionResult> AddDocumentToProject(ProjectDocumentDataIn dataIn)
        {
            await projectManagementBLL.AddDocumentToProject(dataIn).ConfigureAwait(false);
            return Ok();
        }

        [SReportsAuthorize(Permission = PermissionNames.DeleteDocument, Module = ModuleNames.ProjectManagement)]
        [SReportsAuditLog]
        [HttpDelete]
        public async Task<ActionResult> RemoveDocumentFromProject(string formId, int projectId)
        {
            await projectManagementBLL.RemoveDocumentFromProject(formId, projectId).ConfigureAwait(false);
            return NoContent();
        }

        [SReportsAuthorize(Permission = PermissionNames.AddPatient, Module = ModuleNames.ProjectManagement)] 
        [SReportsAuditLog]
        [HttpPost]
        public async Task<ActionResult> AddPatientToTrials(AddProjectsToPatientDataIn dataIn)
        {
            await projectManagementBLL.AddPatientToProjects(dataIn.PatientProjects).ConfigureAwait(false);
            return Ok();
        }

        [SReportsAuthorize(Permission = PermissionNames.DeletePatient, Module = ModuleNames.ProjectManagement)]
        [SReportsAuditLog]
        [HttpDelete]
        public async Task<ActionResult> RemovePatientFromTrial(int patientId, int projectId)
        {
            await projectManagementBLL.RemovePatientFromProject(patientId, projectId).ConfigureAwait(false);
            
            return NoContent();
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.ProjectManagement)]
        [SReportsAuditLog]
        public async Task<ActionResult> GetTrialAutoCompleteName(AutocompleteDataIn dataIn)
        {
            return Json(await trialManagementBLL.GetTrialAutoCompleteTitle(dataIn).ConfigureAwait(false));
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.ProjectManagement)]
        [SReportsAuditLog]
        public async Task<ActionResult> GetTrialAutoCompleteTitleForPatient(AutocompleteDataIn dataIn, int patientId)
        {
            var clinicalTrialTypeId = GetClinicalTrialTypeId();
            return Json(await projectManagementBLL.GetProjectAutoCompleteTitleForPatient(dataIn, patientId, clinicalTrialTypeId)
                .ConfigureAwait(false));
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Designer)]
        [SReportsAuditLog]
        public async Task<ActionResult> GetDocumentsAutoCompleteName(AutocompleteDataIn dataIn, int projectId)
        {
            return Json(await projectManagementBLL.GetDocumentTitleForAutocomplete(dataIn, userCookieData, projectId)
                .ConfigureAwait(false));
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.ProjectManagement)]
        [SReportsAuditLog]
        public async Task<ActionResult> ProjectForms(int projectId)
        {
            SetUserProjectViewBags();
            return View("ProjectForms", await projectManagementBLL.GetById(projectId).ConfigureAwait(false));
        }

        private async Task<ActionResult> CreateOrEdit(int projectId = 0)
        {
            SetProjectManagementViewBags();
            return View("Project", await projectManagementBLL.GetById(projectId).ConfigureAwait(false));
        }

        private void SetProjectManagementViewBags()
        {
            ViewBag.ClinicalTrialIdentifiers = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.ClinicalTrialIdentifiers);
            ViewBag.ClinicalTrialSponsorIdentifierTypes = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.ClinicalTrialSponsorIdentifierType);
            ViewBag.ClinicalTrialRecruitmentsStatuses = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.ClinicalTrialRecruitmentsStatus);
            ViewBag.Occupations = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.Occupation);
            ViewBag.DocumentPropertiesEnums = Mapper.Map<Dictionary<string, List<EnumDTO>>>(this.codeBLL.GetDocumentPropertiesEnums());
            ViewBag.ClinicalTrialTypeId = GetClinicalTrialTypeId();
            ViewBag.ShowUserProjects = false;
        }

        private void SetUserProjectViewBags()
        {
            ViewBag.ShowUserProjects = true;
            ViewBag.ClinicalTrialTypeId = GetClinicalTrialTypeId();
            ViewBag.DocumentPropertiesEnums = Mapper.Map<Dictionary<string, List<EnumDTO>>>(this.codeBLL.GetDocumentPropertiesEnums());
        }

        private int GetClinicalTrialTypeId() 
        {
            ViewBag.ProjectTypes = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.ProjectType);
            var projectTypes = ViewBag.ProjectTypes as List<CodeDataOut>;
            return projectTypes.Where(e => e.Thesaurus.Translations
                .Exists(t => t.PreferredTerm == "Clinical Trial")).Select(x => x.Id).FirstOrDefault();
        }
    }
}