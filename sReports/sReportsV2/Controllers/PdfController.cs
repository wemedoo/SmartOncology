using sReportsV2.BusinessLayer.Interfaces;
using System.Net;
using sReportsV2.Common.CustomAttributes;
using System.Collections.Generic;
using sReportsV2.Common.Constants;
using sReportsV2.DTOs.DTOs.PDF.DataOut;
using sReportsV2.DTOs.DTOs.PDF.DataIn;
using sReportsV2.Cache.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.Extensions.Configuration;

namespace sReportsV2.Controllers
{
    public class PdfController : BaseController
    {
        private readonly IPdfBLL pdfBLL;

        public PdfController(IPdfBLL pdfBLL,            
            IHttpContextAccessor httpContextAccessor, 
            IServiceProvider serviceProvider, 
            IConfiguration configuration,
            IAsyncRunner asyncRunner) : 
            base(httpContextAccessor, serviceProvider, configuration, asyncRunner)
        {
            this.pdfBLL = pdfBLL;
        }

        [SReportsAuthorize(Permission = PermissionNames.Download, Module = ModuleNames.Engine)]
        public ActionResult GetPdfForFormId(string formId)
        {
            SetCustomResponseHeaderForMultiFileDownload();
            PdfDocumentDataOut result = pdfBLL.Generate(new PdfDocumentDataIn
            {
                ResourceId = formId,
                UserCookieData = userCookieData
            });
            return File(result.Content, result.ContentType, result.DocumentName);          
        }
        
        [HttpPost]
        [SReportsAuditLog]
        [SReportsAuthorize]
        public ActionResult Upload(IFormFile file)
        {
            pdfBLL.UploadFile(file, userCookieData);

            return StatusCode(StatusCodes.Status201Created);
        }

        [SReportsAuthorize(Permission = PermissionNames.Download, Module = ModuleNames.Engine)]
        public ActionResult GetSynopticPdf(string formInstanceId)
        {
            PdfDocumentDataOut result = pdfBLL.GenerateSynoptic(new PdfDocumentDataIn
            {
                ResourceId = formInstanceId,
                UserCookieData = userCookieData
            });
            return File(result.Content, result.ContentType, result.DocumentName);
        }
    }
}