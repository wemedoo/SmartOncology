using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using sReportsV2.BusinessLayer.Interfaces;
using System;

namespace sReportsV2.Controllers
{
    public class ErrorController : BaseController
    {
        public ErrorController(
            IHttpContextAccessor httpContextAccessor,
            IServiceProvider serviceProvider, 
            IConfiguration configuration, 
            IAsyncRunner asyncRunner
        ) : base(httpContextAccessor, serviceProvider, configuration, asyncRunner)
        {
            // Constructor logic here if needed
        }

        // GET: Error
        public ActionResult AccessDenied()
        {
            return View();
        }

        public new ActionResult NotFound()
        {
            return View();
        }

        public ActionResult Error()
        {
            return View();
        }
    }
}