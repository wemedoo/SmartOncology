using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Constants;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Common.Exceptions;
using sReportsV2.DTOs.Common;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace sReportsV2.Controllers
{
    public class BlobController : BaseController
    {
        private readonly IBlobStorageBLL blobStorageBLL;
        private readonly IWebHostEnvironment hostingEnvironment;

        public BlobController(IWebHostEnvironment hostingEnvironment, IBlobStorageBLL blobStorageBLL, IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider, IConfiguration configuration, IAsyncRunner asyncRunner) : base(httpContextAccessor, serviceProvider, configuration, asyncRunner)
        {
            this.hostingEnvironment = hostingEnvironment;
            this.blobStorageBLL = blobStorageBLL;
        }

        public ActionResult Create()
        {
            return View();
        }

        [SReportsAuditLog]
        [HttpPost]
        public async Task<ActionResult> Create(IFormFile file, string domain)
        {
            if (file != null && file.Length > 0)
            {
                var storageUri = await blobStorageBLL.CreateAsync(file, domain).ConfigureAwait(false);
                return Content(storageUri);
            }
            else
            {
                return BadRequest(new ErrorDTO("No file uploaded"));
            }
        }

        [SReportsAuditLog(new string[] { "file" })]
        [HttpPost]
        public async Task<ActionResult> CreateAudio(string file, bool isLastChunk)
        {
            string url = "";
            string audioBase64 = file;
            byte[] audioBytes = Convert.FromBase64String(audioBase64);

            if (audioBytes != null && audioBytes.Length > 0)
            {
                string tempDirPath = System.IO.Path.Combine(hostingEnvironment.ContentRootPath, "App_Data", "TempChunks");
                blobStorageBLL.SaveChunk(audioBytes, tempDirPath);

                if (isLastChunk)
                {
                    return Content(await blobStorageBLL.MergeAudioFiles(tempDirPath).ConfigureAwait(false));
                }

                return Content(url);
            }
            else
            {
                return BadRequest();
            }
        }

        [SReportsAuditLog]
        [SReportsAuthorize]
        [HttpDelete]
        public ActionResult Delete(BinaryMetadataDataIn data)
        {
            blobStorageBLL.Delete(data);
            return Ok();
        }

        [SReportsAuthorize]
        public async Task<IActionResult> Download(BinaryMetadataDataIn data)
        {
            try
            {
                var fileContent = await blobStorageBLL.DownloadAsync(data).ConfigureAwait(false);
                if (fileContent == null)
                {
                    return NotFound(new ErrorDTO("Object does not exit"));
                }

                var provider = new FileExtensionContentTypeProvider();
                if (!provider.TryGetContentType(data.ResourceId, out var contentType))
                {
                    contentType = "application/octet-stream";
                }

                return File(fileContent, contentType);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while processing the resource. Details: resourceId: {ResourceId}, domain: {Domain}", data.ResourceId, data.Domain);
                throw new UserAdministrationException(StatusCodes.Status404NotFound, "Error while downloading resource!");
            }
        }

        [SReportsAuditLog]
        [HttpPost]
        public async Task<IActionResult> UploadLogo(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new ErrorDTO("File cannot be null or empty."));
            }
            CheckLogoSize((int)file.Length);
            string resourceUrl = await blobStorageBLL.CreateAsync(file, StorageDirectoryNames.OrganizationLogo).ConfigureAwait(false);

            return Content(resourceUrl);
        }

        private void CheckLogoSize(int fileLength)
        {
            if (!IsLessThanLimit(fileLength))
            {
                throw new UserAdministrationException(StatusCodes.Status409Conflict, $"Your logo size is limited to {GetLogoLimitFromConfiguration()}MB. Please upload the appropriate logo size.");
            }
        }

        private bool IsLessThanLimit(int fileLength)
        {
            return fileLength < GetLogoLimit();
        }

        private double GetLogoLimit()
        {
            return GetLogoLimitFromConfiguration() * Math.Pow(10, 6);
        }

        private double GetLogoLimitFromConfiguration()
        {
            double logoLimitInMB = double.TryParse(Configuration["LogoSizeLimit"], out double logoSizeLimit) ? logoSizeLimit : 3;
            return logoLimitInMB;
        }
    }
}