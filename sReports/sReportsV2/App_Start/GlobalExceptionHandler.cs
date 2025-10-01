using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Http;
using Serilog;
using sReportsV2.BusinessLayer.Components.Implementations;
using sReportsV2.Cache.Resources;
using sReportsV2.Common.Exceptions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using sReportsV2.DTOs.Common;
using Newtonsoft.Json;
using sReportsV2.Common.Extensions;
using Microsoft.EntityFrameworkCore;
using sReportsV2.Common.Helpers;

namespace sReportsV2.App_Start
{
    public class GlobalExceptionHandler
    {
        public async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
        {
            string exTypeName = exception.GetType()?.Name;
            switch (exception)
            {
                case ConcurrencyException _:
                case DbUpdateConcurrencyException _:
                    await HandleException(httpContext, exception, StatusCodes.Status409Conflict, TextLanguage.ConcurrencyExEdit, exTypeName);
                    break;
                case ConcurrencyDeleteException _:
                    await HandleException(httpContext, exception, StatusCodes.Status409Conflict, TextLanguage.ConcurrencyExDelete, exTypeName);
                    break;
                case ConcurrencyDeleteEditException _:
                    await HandleException(httpContext, exception, StatusCodes.Status409Conflict, TextLanguage.ConcurrencyExDeleteEdit, exTypeName);
                    break;
                case NullReferenceException _:
                    var responseMessage = exception.Message.Equals("Object reference not set to an instance of an object.'") ? TextLanguage.NotFound : exception.Message;
                    await HandleException(httpContext, exception, StatusCodes.Status404NotFound, responseMessage, exTypeName);
                    break;
                case IterationNotFinishedException _:
                    await HandleException(httpContext, exception, StatusCodes.Status400BadRequest, TextLanguage.Current_Iteration_is_not_finished, exTypeName);
                    break;
                case ConsensusCannotStartException consensusCannotStartEx:
                    string errorMessage = TextLanguage.Cannot_start_CF + consensusCannotStartEx.FormItemLevel;
                    await HandleException(httpContext, exception, StatusCodes.Status400BadRequest, errorMessage, exTypeName);
                    break;
                case UserAdministrationException userAdministrationEx:
                    await HandleException(httpContext, exception, userAdministrationEx.HttpStatusCode, exception.Message, exTypeName);
                    break;
                case DuplicateException _:
                    await HandleException(httpContext, exception, StatusCodes.Status409Conflict, exception.Message, exTypeName);
                    break;
                case ApiCallException _:
                    await HandleException(httpContext, exception, StatusCodes.Status500InternalServerError, "Error calling external api service.", exTypeName);
                    break;
                case ThesaurusCannotDeleteException _:
                    await HandleException(httpContext, exception, StatusCodes.Status409Conflict, exception.Message, exTypeName);
                    break;
                case DuplicateAliasException _:
                    await HandleException(httpContext, exception, StatusCodes.Status409Conflict, exception.Message, exTypeName);
                    break;
                case TabularExportException _:
                    await HandleException(httpContext, exception, StatusCodes.Status500InternalServerError, TextLanguage.TabularExportException, exTypeName);
                    break;
                case InvalidFormulaException _:
                    await HandleException(httpContext, exception, StatusCodes.Status409Conflict, exception.Message, exTypeName);
                    break;
                case InvalidEvaluationException invalidEvaluationException:
                    HandleException(httpContext, invalidEvaluationException, StatusCodes.Status400BadRequest, invalidEvaluationException.Message, exTypeName);
                    break;
                default:
                    string unknownExceptionMsg = TextLanguage.UnknownExceptionMsg;
                    await HandleException(httpContext, exception, StatusCodes.Status500InternalServerError, unknownExceptionMsg, $"Unkown -> {exTypeName}");
                    break;
            }
        }

        private async Task HandleException(HttpContext httpContext, Exception exception, int errorStatusCode, string responseErrorMessage, string exType)
        {
            LogException(httpContext, exception, exType);

            var response = httpContext.Response;
            response.StatusCode = errorStatusCode;
            if (httpContext.Request.IsAjaxRequest())
            {
                response.ContentType = "application/json";
                var result = JsonConvert.SerializeObject(new ErrorDTO(responseErrorMessage));
                await response.WriteAsync(result);
            }
            else
            {
                var routeData = httpContext.GetRouteData() ?? new RouteData();
                var actionContext = new ActionContext(httpContext, routeData, new ActionDescriptor());

                var viewResult = new ViewResult
                {
                    ViewName = "Error",
                    ViewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                    {
                        {"ErrorMessage", responseErrorMessage}
                    }
                };
                var executor = httpContext.RequestServices.GetRequiredService<IActionResultExecutor<ViewResult>>();
                await executor.ExecuteAsync(actionContext, viewResult);
            }
        }

        private void LogException(HttpContext httpContext, Exception exception, string exType)
        {
            Log.Error("<--- Exception [{ExType}] is thrown in ({RequestMethod} {RequestPath}) ---> Message: {Message} StackTrace: {StackTrace}", exType, httpContext.Request.Method, httpContext.Request.Path, exception.GetExceptionStackMessages(), exception.StackTrace);
        }
    }
}
