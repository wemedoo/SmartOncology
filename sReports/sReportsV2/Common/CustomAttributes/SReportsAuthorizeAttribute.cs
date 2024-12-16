using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using sReportsV2.DTOs.User.DTO;
using sReportsV2.DAL.Sql.Interfaces;
using Microsoft.AspNetCore.Authentication;
using System.IO;
using sReportsV2.Common.Constants;
using sReportsV2.Cache.Resources;
using sReportsV2.Common.Extensions;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using sReportsV2.DTOs.Common;
using Newtonsoft.Json;
using sReportsV2.Common.Exceptions;
using Serilog;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Helpers;

namespace sReportsV2.Common.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class SReportsAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter, IResultFilter
    {
        public string Module { get; set; }
        public string Permission { get; set; }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var session = context?.HttpContext?.Session;
            if (session == null)
            {
                Log.Error("No session");
                HandleSReportsUnauthorizedRequest(context);
                return;
            }

            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                Log.Error("User is not authenticated");
                HandleSReportsUnauthorizedRequest(context);
                return;
            }

            var userCookieData = session.GetUserFromSession();
            var userDAL = context.HttpContext.RequestServices.GetService<IPersonnelDAL>();
            var codeDAL = context.HttpContext.RequestServices.GetService<ICodeDAL>();
            int? activeUserStateCD = codeDAL.GetByCodeSetIdAndPreferredTerm((int)CodeSetList.UserState, CodeAttributeNames.Active);

            if (userCookieData == null || !userDAL.IsUserStillValid(userCookieData.Id, activeUserStateCD))
            {
                SignOutUser(context).Wait();
            }
            else if (!DoesUserHaveAccessRight(session))
            {
                HandleSReportsUnauthorizedRequest(context);
            }
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            string actionName = (string)context.RouteData.Values["action"];
            string requestType = context.HttpContext.Request.Method;

            if(requestType.Equals(EndpointConstants.GET, StringComparison.OrdinalIgnoreCase))
            {
                var viewData = ((Controller)context.Controller).ViewData;

                if (actionName.Equals(EndpointConstants.Create, StringComparison.OrdinalIgnoreCase))
                {
                    viewData["ReadOnly"] = false;
                    viewData["Disabled"] = "";
                }
                else if (actionName.Equals(EndpointConstants.Edit, StringComparison.OrdinalIgnoreCase))
                {
                    viewData["ReadOnly"] = !DoesUserHaveAccessRight(Module, PermissionNames.Update, context.HttpContext.Session);
                    viewData["Disabled"] = (bool)viewData["ReadOnly"] ? "disabled" : "";
                }
                else if (actionName.Equals(EndpointConstants.View, StringComparison.OrdinalIgnoreCase))
                {
                    viewData["ReadOnly"] = true;
                    viewData["Disabled"] = "disabled";
                }
            }
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
        }

        private async Task SignOutUser(AuthorizationFilterContext context)
        {
            AccountService accountService = context.HttpContext.RequestServices.GetService<AccountService>();
            await accountService.SignOutAsync();
        }

        private void HandleSReportsUnauthorizedRequest(AuthorizationFilterContext context)
        {
            HttpRequest request = context.HttpContext.Request;

            if (request.IsAjaxRequest())
            {
                string forbiddenMessage = FormatForbiddenMessage(context.ActionDescriptor.DisplayName);
                throw new UserAdministrationException(StatusCodes.Status403Forbidden, forbiddenMessage);
            }
            else
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Error", new { id = $"{Permission}_{Module}" });
            }

            context.Result = new RedirectToActionResult("AccessDenied", "Error", new { id = $"{Permission}_{Module}" });
        }

        private string FormatForbiddenMessage(string actionName)
        {
            return string.Format("{0} ({1} : {2}, {3} : {4}, {5} : {6})", TextLanguage.Access_Denied_Message, TextLanguage.Permission, Permission, TextLanguage.Module, Module, TextLanguage.Action, actionName);
        }

        private bool DoesUserHaveAccessRight(ISession session)
        {
            return DoesUserHaveAccessRight(Module, Permission, session);
        }

        private bool DoesUserHaveAccessRight(string module, string permission, ISession session)
        {
            if (!string.IsNullOrWhiteSpace(module) && !string.IsNullOrWhiteSpace(permission))
            {
                UserCookieData userCookieData = session.GetUserFromSession();
                if (userCookieData != null)
                {
                    return userCookieData.UserHasPermission(permission, module);
                }
            }

            return true;
        }
    }
}
