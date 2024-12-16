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

namespace sReportsV2.Controllers
{
    public class FormConsensusController : FormCommonController
    {
        protected readonly IConsensusInstanceDAL consensusInstanceDAL;
        protected readonly IConsensusBLL consensusBLL;
        protected readonly IConsensusDAL consensusDAL;

        private readonly IOutsideUserDAL outsideUserDAL;
        private readonly IPersonnelDAL userDAL;
        private readonly IOrganizationDAL organizationDAL;
        private readonly IMapper Mapper;

        public FormConsensusController(IPatientDAL patientDAL, 
            IEpisodeOfCareDAL episodeOfCareDAL,
            IEncounterDAL encounterDAL, 
            IUserBLL userBLL, 
            IOrganizationBLL organizationBLL, 
            ICodeBLL codeBLL, 
            IFormInstanceBLL formInstanceBLL, 
            IFormBLL formBLL, 
            IOutsideUserDAL outsideUserDAL, 
            IPersonnelDAL userDAL, 
            IOrganizationDAL organizationDAL,
            IConsensusDAL consensusDAL,
            IConsensusInstanceDAL consensusInstanceDAL,
            IConsensusBLL consensusBLL,
            IThesaurusDAL thesaurusDAL, 
            IAsyncRunner asyncRunner, 
            IPdfBLL pdfBLL,
            IMapper mapper,            
            IHttpContextAccessor httpContextAccessor,
            IServiceProvider serviceProvider,
            IConfiguration configuration) : 
            base(patientDAL, episodeOfCareDAL, encounterDAL, userBLL, organizationBLL, codeBLL, formInstanceBLL, formBLL, thesaurusDAL, asyncRunner, pdfBLL, mapper, httpContextAccessor, serviceProvider, configuration)
        {
            this.outsideUserDAL = outsideUserDAL;
            this.consensusDAL = consensusDAL;
            this.consensusInstanceDAL = consensusInstanceDAL;
            this.userDAL = userDAL;
            this.organizationDAL = organizationDAL;
            this.consensusBLL = consensusBLL;
            Mapper = mapper;
        }

        [SReportsAuditLog]
        [SReportsAuthorize]
        [HttpGet]
        public ActionResult GetMapObject()
        {
            var obj = System.IO.File.ReadAllText($@"{DirectoryHelper.AppDataFolder}\countries-50m.json");
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
            ViewBag.ConsensusQuestionnaire = new ConsensusQuestionnaireDataOut(Mapper.Map<ConsensusDataOut>(consensusDAL.GetByFormId(formId)));
            return PartialView("~/Views/Form/Consensus/ConsensusPartial.cshtml", GetFormDataOut(formDAL.GetForm(formId)));
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
            Form form = formDAL.GetForm(formId);
            FormDataOut formDataOut = Mapper.Map<FormDataOut>(form);
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
            List<OrganizationUsersCountDataOut> data = Mapper.Map<List<OrganizationUsersCountDataOut>>(organizationBLL.GetOrganizationUsersCount(name, countries));
            return PartialView("~/Views/Form/Consensus/Users/OrganizationHierarchy.cshtml", data);
        }
        
        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.FindConsensus, Module = ModuleNames.Designer)]
        [HttpPost]
        public ActionResult CreateOutsideUser(OutsideUserDataIn userDataIn)
        {
            int userId = outsideUserDAL.InsertOrUpdate(Mapper.Map<Domain.Sql.Entities.OutsideUser.OutsideUser>(userDataIn));
            Consensus consensus = consensusDAL.GetById(userDataIn.ConsensusRef);
            if (!consensus.Iterations.Last().OutsideUserIds.Contains(userId)) 
            {
                consensus.Iterations.Last().OutsideUserIds.Add(userId);
            }

            consensusDAL.Insert(consensus);

            List<ConsensusUserDataOut> users = Mapper.Map<List<ConsensusUserDataOut>>(outsideUserDAL.GetAllByIds(consensus.Iterations.Last().OutsideUserIds));
            SetLastIterationStateViewBag(consensus.Id);
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
            Consensus consensus = consensusDAL.GetById(consensusId);
            consensus.Iterations.Last().OutsideUserIds = consensus.Iterations.Last().OutsideUserIds.Where(x => x != userId).ToList();
            consensusDAL.Insert(consensus);
            return Ok();
        }


        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.FindConsensus, Module = ModuleNames.Designer)]
        [HttpPost]
        public ActionResult DeleteInsideUser(int userId, string consensusId)
        {
            Consensus consensus = consensusDAL.GetById(consensusId);
            consensus.Iterations.Last().UserIds = consensus.Iterations.Last().UserIds.Where(x => x != userId).ToList();
            consensusDAL.Insert(consensus);
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
            if (consensusDAL.CanStartConsensusFindingProcess(dataIn.ConsensusId))
            {
                this.consensusBLL.StartConsensusFindingProcess(dataIn);
            }
            else
            {
                throw new UserAdministrationException(StatusCodes.Status400BadRequest, TextLanguage.CF_iteration_stop);
            }
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
            ConsensusInstance instance = consensusInstanceDAL.GetByConsensusAndUserAndIteration(consensusInstanceUser.ConsensusId, consensusInstanceUser.UserId, consensusInstanceUser.IterationId);
            Consensus consensus = consensusDAL.GetById(consensusInstanceUser.ConsensusId);

            ViewBag.ConsensusQuestionnaire = new ConsensusQuestionnaireDataOut(
                Mapper.Map<ConsensusDataOut>(consensus),
                instance, 
                consensusInstanceUser,
                ResourceTypes.ConsensusInstance,
                userCookieData?.Id
            );

            return View(viewName, GetFormDataOut(formDAL.GetForm(consensus.FormRef)));
        }

        public ActionResult GetQuestionnairePartialCommon(ConsensusInstanceUserDataIn consensusInstanceUserData, string partialViewName)
        {
            ConsensusDataOut consensus = Mapper.Map<ConsensusDataOut>(consensusDAL.GetByFormId(consensusInstanceUserData.FormId));
            if (consensus != null && consensus.Iterations != null)
            {
                consensus.Iterations = consensus.Iterations.Where(x => x.Id == consensusInstanceUserData.IterationId).ToList();
            }
            ConsensusInstance consensusInstance = null;
            if (!string.IsNullOrWhiteSpace(consensusInstanceUserData.ConsensusInstanceId))
            {
                consensusInstance = consensusInstanceDAL.GetById(consensusInstanceUserData.ConsensusInstanceId);
                consensus.GetIterationById(consensusInstanceUserData.IterationId).SetQuestionsValue(Mapper.Map<List<ConsensusQuestionDataOut>>(consensusInstance.Questions));
            }

            consensusInstanceUserData.ViewType = consensusInstanceUserData.ViewType ?? EndpointConstants.View;
            ViewBag.ConsensusQuestionnaire = new ConsensusQuestionnaireDataOut(
                consensus,
                consensusInstance,
                consensusInstanceUserData,
                consensusInstanceUserData.ShowQuestionnaireType,
                userCookieData?.Id
                );
            return PartialView(partialViewName, GetFormDataOut(formDAL.GetForm(consensusInstanceUserData.FormId)));
        }

        private void SetLastIterationStateViewBag(string consensusId)
        {
            ViewBag.CanEditConsensusUsers = consensusDAL.GetLastIterationState(consensusId) == IterationState.Design;
        }

        private ActionResult GetConsensusTreePartial(string formId)
        {
            ViewBag.ConsensusQuestionnaire = new ConsensusQuestionnaireDataOut(Mapper.Map<ConsensusDataOut>(consensusDAL.GetByFormId(formId)));
            return PartialView("~/Views/Form/Consensus/Questionnaire/ConsensusFormTree.cshtml", GetFormDataOut(formDAL.GetForm(formId)));
        }
    }
}