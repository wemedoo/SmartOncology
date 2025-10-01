using AutoMapper;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Constants;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Extensions;
using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.Domain.Entities.Consensus;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.DTOs.Common.DataOut;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.Consensus.DataIn;
using sReportsV2.DTOs.DTOs.Consensus.DataOut;
using sReportsV2.DTOs.DTOs.FormConsensus.DataIn;
using sReportsV2.DTOs.Form.DataIn;
using sReportsV2.DTOs.Form.DataOut;
using sReportsV2.DTOs.Organization.DataOut;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using sReportsV2.Cache.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using sReportsV2.Common.Helpers;
using sReportsV2.Common.Exceptions;
using System.IO;

namespace sReportsV2.Controllers
{
    public class FormConsensusController : FormCommonController
    {
        protected readonly IConsensusBLL consensusBLL;

        public FormConsensusController(
            IUserBLL userBLL, 
            IOrganizationBLL organizationBLL, 
            ICodeBLL codeBLL, 
            IFormInstanceBLL formInstanceBLL, 
            IFormBLL formBLL, 
            IConsensusBLL consensusBLL,
            IAsyncRunner asyncRunner, 
            IMapper mapper,            
            IHttpContextAccessor httpContextAccessor,
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            ICacheRefreshService cacheRefreshService) : 
            base(userBLL, organizationBLL, codeBLL, formInstanceBLL, formBLL, asyncRunner, mapper, httpContextAccessor, serviceProvider, configuration, cacheRefreshService)
        {
            this.consensusBLL = consensusBLL;
        }

        [SReportsAuditLog]
        [SReportsAuthorize]
        [HttpGet]
        public ActionResult GetMapObject()
        {
            var obj = System.IO.File.ReadAllText(Path.Combine(DirectoryHelper.AppDataFolder, ResourceTypes.CountriesFolder, "countries-50m.json"));
            return Content(obj);
        }

        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.FindConsensus, Module = ModuleNames.Designer)]
        public ActionResult StartNewIteration(string consensusId, string formId)
        {
            ResourceCreatedDTO resourceCreatedDTO = consensusBLL.StartNewIteration(consensusId, formId, userCookieData.Id);

            return Json(resourceCreatedDTO);
        }

        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.FindConsensus, Module = ModuleNames.Designer)]
        public ActionResult TerminateCurrentIteration(string consensusId)
        {
            ResourceCreatedDTO resourceCreatedDTO = consensusBLL.TerminateCurrentIteration(consensusId);

            return Json(resourceCreatedDTO);
        }

        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.FindConsensus, Module = ModuleNames.Designer)]
        public ActionResult LoadConsensusPartial(string formId)
        {
            ViewBag.ConsensusQuestionnaire = new ConsensusQuestionnaireDataOut(consensusBLL.GetByFormId(formId));
            return PartialView("~/Views/Form/Consensus/ConsensusPartial.cshtml", GetFormDataOut(formBLL.GetFormById(formId)));
        }

        [SReportsAuditLog]
        public ActionResult GetQuestionnairePartial(ConsensusInstanceUserDataIn consensusInstanceUserData)
        {
            return GetQuestionnairePartialCommon(consensusInstanceUserData, "~/Views/Form/Consensus/Questionnaire/ConsensusQuestionnairePartial.cshtml");
        }

        [SReportsAuditLog]
        [SReportsAuthorize]
        public ActionResult GetConsensusUsersPartial(string consensusId, bool readOnlyMode)
        {
            ConsensusUsersDataOut data = consensusBLL.GetConsensusUsers(consensusId, userCookieData.ActiveOrganization);
            SetLastIterationStateViewBag(consensusId);
            SetReadOnlyAndDisabledViewBag(readOnlyMode);
            return PartialView("~/Views/Form/Consensus/Users/ConsensusUsersPartial.cshtml", data);
        }

        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.FindConsensus, Module = ModuleNames.Designer)]
        [HttpPost]
        public ActionResult ProceedConsensus(ProceedConsensusDataIn proceedConsensusDataIn)
        {
            ViewBag.ConsensusQuestionnaire = new ConsensusQuestionnaireDataOut(consensusBLL.ProceedIteration(proceedConsensusDataIn));
            return PartialView("~/Views/Form/Consensus/Questionnaire/ConsensusFormTree.cshtml", formBLL.GetFormDataOutById(proceedConsensusDataIn.FormId, userCookieData));
        }

        public ActionResult ReloadConsensusTree(string consensusId, string formId)
        {
            ViewBag.ConsensusQuestionnaire = new ConsensusQuestionnaireDataOut(consensusBLL.GetById(consensusId));
            return PartialView("~/Views/Form/Consensus/Questionnaire/ConsensusFormTree.cshtml", formBLL.GetFormDataOutById(formId, userCookieData));
        }

        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.FindConsensus, Module = ModuleNames.Designer)]
        public ActionResult AddQuestion(ConsensusQuestionDataIn questionDataIn)
        {
            Ensure.IsNotNull(questionDataIn, nameof(questionDataIn));
            consensusBLL.AddQuestion(questionDataIn);
            return GetConsensusTreePartial(questionDataIn.FormId);
        }

        [SReportsAuditLog]
        public ActionResult GetConsensusFormPreview(string formId)
        {
            Form form = formBLL.GetFormById(formId);
            FormDataOut formDataOut = mapper.Map<FormDataOut>(form);
            formDataOut.SetActiveChapterAndPageId(null);
            SetReadOnlyAndDisabledViewBag(true);
            SetViewBagAndMakeResetAndNeSectionHidden();
            ViewBag.CollapseChapters = true;
            return PartialView("~/Views/FormInstance/FormInstanceContent.cshtml", formDataOut);
        }

        [SReportsAuditLog]
        [HttpGet]
        public ActionResult ReloadConsensusTree(string formId)
        {
            return GetConsensusTreePartial(formId);
        }

        [SReportsAuditLog]
        [HttpGet]
        public ActionResult ReloadConsensusInstanceTree(ConsensusInstanceUserDataIn consensusInstanceUserData)
        {
            consensusInstanceUserData.ShowQuestionnaireType = ResourceTypes.ConsensusInstance;
            return GetQuestionnairePartialCommon(consensusInstanceUserData, "~/Views/Form/Consensus/Questionnaire/ConsensusFormTree.cshtml");
        }

        [SReportsAuditLog]
        [SReportsAuthorize]
        [HttpGet]
        public ActionResult GetTrackerData(string consensusId)
        {
            return PartialView("~/Views/Form/Consensus/Tracker/ConsensusTrackerPartial.cshtml", consensusBLL.GetTrackerData(consensusId));
        }

        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.FindConsensus, Module = ModuleNames.Designer)]
        [HttpPost]
        public ActionResult GetUserHierarchy(string name, List<string> countries)
        {
            List<OrganizationUsersCountDataOut> data = mapper.Map<List<OrganizationUsersCountDataOut>>(organizationBLL.GetOrganizationUsersCount(name, countries));
            return PartialView("~/Views/Form/Consensus/Users/OrganizationHierarchy.cshtml", data);
        }
        
        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.FindConsensus, Module = ModuleNames.Designer)]
        [HttpPost]
        public ActionResult CreateOutsideUser(OutsideUserDataIn userDataIn)
        {
            List<ConsensusUserDataOut> users = consensusBLL.CreateOutsideUser(userDataIn);
            SetLastIterationStateViewBag(userDataIn.ConsensusRef);
            ViewBag.IsOutsideUserList = true;
            return PartialView("~/Views/Form/Consensus/Users/ConsensusUsersList.cshtml", users);
        }

        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.FindConsensus, Module = ModuleNames.Designer)]
        [HttpPost]
        public ActionResult ReloadUsers(List<int> organizationIds, string consensusId)
        {
            List<OrganizationUsersDataOut> result = new List<OrganizationUsersDataOut>();

            if (organizationIds != null) 
            {
                result = organizationBLL.GetUsersByOrganizationsIds(organizationIds);
            }
            SetLastIterationStateViewBag(consensusId);
            return PartialView("~/Views/Form/Consensus/Users/ConsensusUsers.cshtml", result);
        }


        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.FindConsensus, Module = ModuleNames.Designer)]
        [HttpPost]
        public ActionResult DeleteOutsideUser(int userId, string consensusId)
        {
            consensusBLL.DeleteConsensusUser(userId, consensusId, true);
            return Ok();
        }


        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.FindConsensus, Module = ModuleNames.Designer)]
        [HttpPost]
        public ActionResult DeleteInsideUser(int userId, string consensusId)
        {
            consensusBLL.DeleteConsensusUser(userId, consensusId, false);
            return Ok();
        }

        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.FindConsensus, Module = ModuleNames.Designer)]
        [HttpPost]
        public ActionResult SaveUsers(List<int> usersIds, string consensusId)
        {
            List<UserDataOut> users = consensusBLL.SaveUsers(usersIds, consensusId);
            SetLastIterationStateViewBag(consensusId);
            ViewBag.IsOutsideUserList = false;
            return PartialView("~/Views/Form/Consensus/Users/ConsensusUsersList.cshtml", users);
        }

        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.FindConsensus, Module = ModuleNames.Designer)]
        [HttpPost]
        public ActionResult StartConsensusFindingProcess(ConsensusFindingProcessDataIn dataIn)
        {
            this.consensusBLL.StartConsensusFindingProcess(dataIn);
            return Json(new
            {
                message = TextLanguage.CF_Process_Start
            });
        }

        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.FindConsensus, Module = ModuleNames.Designer)]
        public ActionResult CreateConsensusInstance(ConsensusInstanceUserDataIn consensusInstanceUser)
        {
            consensusInstanceUser.ViewType = EndpointConstants.Create;
            consensusInstanceUser.IsOutsideUser = false;
            return ShowQuestionnaire(consensusInstanceUser, "~/Views/Form/Consensus/Instance/CreateConsensusInstance.cshtml");
        }

        [SReportsAuditLog]
        public ActionResult CreateConsensusInstanceExternal(ConsensusInstanceUserDataIn consensusInstanceUser)
        {
            consensusInstanceUser.ViewType = EndpointConstants.Create;
            consensusInstanceUser.IsOutsideUser = true;
            return ShowQuestionnaire(consensusInstanceUser, "~/Views/Form/Consensus/Instance/CreateConsensusInstance.cshtml");
        }

        [SReportsAuditLog]
        public ActionResult ShowUserQuestionnaire(ConsensusInstanceUserDataIn consensusInstanceUser)
        {
            consensusInstanceUser.ViewType = EndpointConstants.View;
            return ShowQuestionnaire(consensusInstanceUser, "~/Views/Form/Consensus/Instance/ShowUserQuestionnaire.cshtml");
        }

        [HttpPost]
        [SReportsAuditLog]
        public ActionResult CreateConsensusInstance(ConsensusInstanceDataIn consensusInstance)
        {
            consensusBLL.CanSubmitConsensusInstance(consensusInstance);
            ResourceCreatedDTO resourceCreatedDTO = consensusBLL.SubmitConsensusInstance(consensusInstance);
            return Json(resourceCreatedDTO);
        }

        [SReportsAuditLog]
        public ActionResult RemindUser(RemindUserDataIn remindUserDataIn)
        {
            consensusBLL.RemindUser(remindUserDataIn);
            return Ok();
        }
        
        private ActionResult ShowQuestionnaire(ConsensusInstanceUserDataIn consensusInstanceUser, string viewName)
        {
            ConsensusInstance instance = consensusBLL.GetByConsensusAndUserAndIteration(consensusInstanceUser);
            ConsensusDataOut consensus = consensusBLL.GetById(consensusInstanceUser.ConsensusId);

            ViewBag.ConsensusQuestionnaire = new ConsensusQuestionnaireDataOut(
                consensus,
                instance, 
                consensusInstanceUser,
                ResourceTypes.ConsensusInstance,
                userCookieData?.Id
            );

            return View(viewName, GetFormDataOut(formBLL.GetFormById(consensus.FormRef)));
        }

        public ActionResult GetQuestionnairePartialCommon(ConsensusInstanceUserDataIn consensusInstanceUserData, string partialViewName)
        {
            ViewBag.ConsensusQuestionnaire = consensusBLL.GetQuestionnairePartialCommon(consensusInstanceUserData, userCookieData?.Id);
            return PartialView(partialViewName, GetFormDataOut(formBLL.GetFormById(consensusInstanceUserData.FormId)));
        }

        private void SetLastIterationStateViewBag(string consensusId)
        {
            ViewBag.CanEditConsensusUsers = consensusBLL.GetLastIterationState(consensusId) == IterationState.Design;
        }

        private ActionResult GetConsensusTreePartial(string formId)
        {
            ViewBag.ConsensusQuestionnaire = new ConsensusQuestionnaireDataOut(consensusBLL.GetByFormId(formId));
            return PartialView("~/Views/Form/Consensus/Questionnaire/ConsensusFormTree.cshtml", GetFormDataOut(formBLL.GetFormById(formId)));
        }
    }
}