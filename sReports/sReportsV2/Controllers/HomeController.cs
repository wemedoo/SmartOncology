using sReportsV2.BusinessLayer.Components.Interfaces;
using sReportsV2.DTOs.Common.DTO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.Extensions.Configuration;
using sReportsV2.BusinessLayer.Interfaces;

namespace sReportsV2.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly IEmailSender emailSender;
        public HomeController(IEmailSender emailSender,             
            IHttpContextAccessor httpContextAccessor, 
            IServiceProvider serviceProvider, 
            IConfiguration configuration,
            IAsyncRunner asyncRunner) : 
            base(httpContextAccessor, serviceProvider, configuration, asyncRunner)
        {
            this.emailSender = emailSender;
        }

        public ActionResult Index()
        {
            if (ViewBag.IsDateCaptureMode)
            {
                return RedirectToAction("DataCaptureViewMode", "FormInstance");
            }
            else
            {
                return View();
            }
        }

        public ActionResult TestEMail()
        {
            Task.Run(() => emailSender.SendAsync(new EmailDTO("danilo.acimovic@wemedoo.com", "<div>Testing email</div>", "Testing email")));

            return Json("Test email endpoint is finished");
        }

        public ActionResult TestAudioBasic()
        {
            return View();
        }

    }
}