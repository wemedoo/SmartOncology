using sReportsV2.Common.Constants;
using System.Net;
using sReportsV2.Common.CustomAttributes;
using System.Threading.Tasks;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Extensions;
using Microsoft.AspNetCore.Mvc;
using sReports.PathoLink.Entities;
using Microsoft.AspNetCore.Http;
using System;
using Microsoft.Extensions.Configuration;
using sReportsV2.Common.Exceptions;

namespace sReportsV2.Controllers
{
    public class PatholinkController : BaseController
    {
        private readonly IPatholinkBLL patholinkBLL;

        public PatholinkController(IPatholinkBLL patholinkBLL, IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider, IConfiguration configuration, IAsyncRunner asyncRunner) : base(httpContextAccessor, serviceProvider, configuration, asyncRunner)
        {
            this.patholinkBLL = patholinkBLL;
        }

        [HttpPost]
        [SReportsAuditLog]
        public ActionResult Import(PathoLink pathoLink)
        {
            pathoLink = Ensure.IsNotNull(pathoLink, nameof(pathoLink));

            if (string.IsNullOrEmpty(pathoLink.CaseDetails.submissionID))
            {
                throw new UserAdministrationException(StatusCodes.Status400BadRequest, "Submission id must be set");
            }

            if (patholinkBLL.Import(pathoLink, userCookieData))
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        [SReportsAuthorize(Permission = PermissionNames.Download, Module = ModuleNames.Engine)]
        public async Task<ActionResult> Export(string formInstanceId)
        {
            formInstanceId = Ensure.IsNotNull(formInstanceId, nameof(formInstanceId));
            PathoLink pathoLink = await patholinkBLL.Export(formInstanceId, userCookieData).ConfigureAwait(false);
            SetCustomResponseHeaderForMultiFileDownload();
            return Json(pathoLink);
        }
    }
}