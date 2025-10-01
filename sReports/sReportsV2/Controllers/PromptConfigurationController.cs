using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Constants;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.DTOs.DTOs.Prompt.DataIn;
using System;
using System.Threading.Tasks;

namespace sReportsV2.Controllers
{
    public class PromptConfigurationController : BaseController
    {
        private readonly IPromptConfigurationBLL promptConfigurationBLL;

        public PromptConfigurationController(IHttpContextAccessor httpContextAccessor, 
            IServiceProvider serviceProvider, 
            IConfiguration configuration, 
            IAsyncRunner asyncRunner,
            ICacheRefreshService cacheRefreshService,
            IPromptConfigurationBLL promptConfigurationBLL) : 
            base(httpContextAccessor, serviceProvider, configuration, asyncRunner, cacheRefreshService)
        {
            this.promptConfigurationBLL = promptConfigurationBLL;
        }

        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.ProjectManagement)]
        public async Task<ActionResult> ConfigurePrompts(PromptDataIn dataIn)
        {
            return View("PromptConfiguration", await promptConfigurationBLL.GetFormPrompt(dataIn).ConfigureAwait(false));
        }

        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.ProjectManagement)]
        public async Task<ActionResult> PreviewPrompts(PromptDataIn dataIn)
        {
            return PartialView("PromptPreview", await promptConfigurationBLL.PreviewPrompts(dataIn).ConfigureAwait(false));
        }

        public async Task<ActionResult> GetPrompt(PromptDataIn dataIn)
        {
            return PartialView("PromptDetail", await promptConfigurationBLL.GetPrompt(dataIn).ConfigureAwait(false));
        }


        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.ProjectManagement)]
        [HttpPost]
        [SReportsAuditLog]
        public async Task<ActionResult> UpdatePrompt(PromptInputDataIn dataIn)
        {
            return Json(await promptConfigurationBLL.UpdatePrompt(dataIn, userCookieData.Id).ConfigureAwait(false));
        }

        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.ProjectManagement)]
        [HttpPost]
        [SReportsAuditLog]
        public async Task<ActionResult> AddNewPromptVersion(PromptDataIn dataIn)
        {
            return Json(await promptConfigurationBLL.AddNewPromptVersion(dataIn, userCookieData.Id).ConfigureAwait(false));
        }

        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.ProjectManagement)]
        [HttpPost]
        [SReportsAuditLog]
        public async Task<ActionResult> SwitchPromptVersion(PromptDataIn dataIn)
        {
            return Json(await promptConfigurationBLL.SwitchPromptVersion(dataIn).ConfigureAwait(false));
        }
    }
}
