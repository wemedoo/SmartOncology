using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Newtonsoft.Json;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Constants;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Common.Extensions;
using sReportsV2.Common.Helpers;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.DTOs.DTOs.Fhir.DataIn;
using sReportsV2.DTOs.DTOs.FormInstance.DataIn;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Threading.Tasks;
using sReportsV2.Common.JsonModelBinder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace sReportsV2.Controllers
{
    public class FhirController : FormCommonController
    {
        private readonly IFhirBLL fhirBLL;

        public FhirController(
            IUserBLL userBLL,
            IOrganizationBLL organizationBLL, 
            ICodeBLL codeBLL, 
            IFormInstanceBLL formInstanceBLL, 
            IFormBLL formBLL, 
            IFhirBLL fhirBLL, 
            IAsyncRunner asyncRunner, 
            IMapper mapper, 
            IHttpContextAccessor httpContextAccessor,
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            ICacheRefreshService cacheRefreshService) :
            base(userBLL, organizationBLL, codeBLL, formInstanceBLL, formBLL, asyncRunner, mapper, httpContextAccessor, serviceProvider, configuration, cacheRefreshService)
        {
            this.fhirBLL = fhirBLL;
        }

        [SReportsAuthorize(Permission = PermissionNames.ShowJson, Module = ModuleNames.Designer)]
        public ActionResult ExportFormToQuestionnaire(string formId)
        {
            var serializerSettings = new SerializerSettings
            {
                AppendNewLine = true,
                TrimWhiteSpacesInXml = true,
                Pretty = true,
            };
            Questionnaire questionnaire = fhirBLL.ExportFormToQuestionnaire(formId);
            string jsonString = new FhirJsonSerializer(serializerSettings).SerializeToString(questionnaire);
            SetFileNameInResponse(questionnaire.Title, "json");
            return Json(jsonString);
        }

        [SReportsApiAuthenticate]
        [SReportsAuditLog]
        [HttpPost]
        public async Task<ActionResult> SubmitQuestionnaireResponse([ModelBinder(typeof(JsonFhirModelBinder))] QuestionnaireResponse questionnaireResponse)
        {
            LogHelper.Info($"Received Questionnaire Response: {questionnaireResponse.ToJson()}");
            try
            {
                await fhirBLL.CreateOrUpdateFormInstanceFromQuestionnaireResponse(questionnaireResponse, userCookieData).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return LogApiException(ex, "Error while submitting Form instance from QuestionnaireResponse.");
            }
            Response.StatusCode = StatusCodes.Status202Accepted;
            return Json("Form instance creation requested successfully.");
        }

        [SReportsAuthorize(Permission = PermissionNames.ShowJson, Module = ModuleNames.Designer)]
        public ActionResult ExportFormToMimacom(string formId)
        {
            return FormatExportResponse(fhirBLL.ExportFormToMimacom(formId).JsonSerialize(true));
        }

        [SReportsApiAuthenticate]
        [SReportsAuditLog]
        [HttpPost]
        public ActionResult CreateFormInstanceFromJson([ModelBinder(typeof(JsonNetModelBinder))] FormInstanceJsonDTO formInstanceJsonInput)
        {
            try
            {
                LogHelper.Info($"Received Mimacom request: {JsonConvert.SerializeObject(formInstanceJsonInput)}");

                formInstanceJsonInput = Ensure.IsNotNull(formInstanceJsonInput, nameof(formInstanceJsonInput));
                string formId = formInstanceJsonInput.FormId;
                Form form = formBLL.GetFormById(formId);
                FormInstance formInstance = formInstanceBLL.GetFormInstanceSet(form, formInstanceDataIn: null, userCookieData, setFieldsFromRequest: false);
                fhirBLL.InsertFromJson(form, formInstance, formInstanceJsonInput);
            }
            catch (Exception ex)
            {
                return LogApiException(ex, "Error during uploading Form instance from json");
            }

            return Json("Form instance has been created successfully");
        }

        [HttpPost]
        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.Engine)]
        public async Task<ActionResult> GenerateDocumentReferenceForDataExtraction(DataExtractionDataIn dataExtractionDataIn)
        {
            bool submittedForDataExtraction = await fhirBLL.GenerateDocumentReferenceForDataExtraction(dataExtractionDataIn).ConfigureAwait(false);
            
            if(submittedForDataExtraction)
                return Ok();
            else
                return StatusCode(StatusCodes.Status500InternalServerError);
        }

        private ContentResult FormatExportResponse(string jsonData)
        {
            return new ContentResult
            {
                Content = jsonData,
                ContentType = "application/json"
            };
        }

        private JsonResult LogApiException(Exception exception, string errorMessage)
        {
            Log.Error("<--- Exception [{ExceptionType}] is thrown in ({RequestMethod} {RequestPath}) ---> Message: {Message} StackTrace: {StackTrace}",
                exception.GetType(),
                httpContextAccessor.HttpContext.Request.Method,
                httpContextAccessor.HttpContext.Request.Path,
                exception.Message,
                exception.StackTrace);
            Response.StatusCode = StatusCodes.Status500InternalServerError;
            return Json(errorMessage);
        }
    }
}