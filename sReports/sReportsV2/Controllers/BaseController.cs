using sReportsV2.Common.Constants;
using sReportsV2.Cache.Singleton;
using System.Collections.Generic;
using sReportsV2.DTOs.User.DTO;
using System.Security.Claims;
using System;
using sReportsV2.Common.Extensions;
using sReportsV2.Common.Helpers;
using sReportsV2.Common.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.SqlDomain.Interfaces;
using sReportsV2.BusinessLayer.Interfaces;

namespace sReportsV2.Controllers
{
    public class BaseController : Controller
    {
        protected UserCookieData userCookieData;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IServiceProvider _serviceProvider;
        protected readonly IAsyncRunner _asyncRunner;
        public IConfiguration Configuration { get; }


        public BaseController(IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider, IConfiguration configuration, IAsyncRunner asyncRunner)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _asyncRunner = asyncRunner ?? throw new ArgumentNullException(nameof(asyncRunner));
            Configuration = configuration;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            var email = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue("preferred_username");
            bool isEmail = true;

            if (User.FindFirstValue("not_email") != null)
                isEmail = false;

            if (!string.IsNullOrEmpty(email))
            {
                userCookieData = HttpContext.Session.GetUserFromSession();
                if (userCookieData == null)
                {
                    if (Configuration.IsSReportsRunning())
                    {
                        SetUserCookieDataForSReports(email, isEmail);
                    }
                    else if (Configuration.IsGlobalThesaurusRunning())
                    {
                        SetUserCookieDataForThesaurusGlobal(email);
                    }
                }
                ViewBag.UserCookieData = userCookieData;
            }
            ViewBag.Languages = SingletonDataContainer.Instance.GetLanguages();
            ViewBag.Env = Configuration["Environment"];
            SetLocalDateFormat();
            SetCustomViewBags();
        }

        protected void UpdateUserCookie(string email)
        {
            SetUserCookieDataForSReports(email);
            ViewBag.UserCookieData = userCookieData;
        }

        protected void SetCustomResponseHeaderForMultiFileDownload()
        {
            HttpContext.Response.Headers.Append("MultiFile", "true");

            if (HttpContext.Request.Headers.TryGetValue("LastFile", out var lastFile))
            {
                HttpContext.Response.Headers.Append("LastFile", string.IsNullOrWhiteSpace(lastFile) ? "true" : lastFile.ToString());
            }
            else
            {
                HttpContext.Response.Headers.Append("LastFile", "true");
            }
        }

        protected void UpdateClaims(Dictionary<string, string> claims)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var claimsIdentity = (ClaimsIdentity)httpContext.User.Identity;
            if (claimsIdentity == null)
            {
                return;
            }

            foreach (KeyValuePair<string, string> keyValue in claims)
            {
                var existingClaim = claimsIdentity.FindFirst(keyValue.Key);
                if (existingClaim != null)
                {
                    claimsIdentity.RemoveClaim(existingClaim);
                }

                claimsIdentity.AddClaim(new Claim(keyValue.Key, keyValue.Value));
            }

            httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Sign in the user with the updated claims
             httpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties { IsPersistent = false }
            );
        }

        protected void SetReadOnlyAndDisabledViewBag(bool readOnly)
        {
            ViewBag.ReadOnly = readOnly;
            ViewBag.Disabled = readOnly ? "disabled" : "";
        }


        protected void SetEpisodeOfCareAndEncounterViewBags()
        {
            SetEpisodeOfCareViewBags();
            SetEncounterViewBags();
        }

        protected void SetEncounterViewBags()
        {
            ViewBag.ServiceTypes = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.ServiceType);
            ViewBag.EncounterTypes = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.EncounterType);
            ViewBag.EncounterStatuses = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.EncounterStatus);
            ViewBag.EncounterClassifications = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.EncounterClassification);
            ViewBag.RelationType = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.RelationType);
        }

        protected void SetEpisodeOfCareViewBags()
        {
            ViewBag.EpisodeOfCareStatuses = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.EOCStatus);
            ViewBag.EpisodeOfCareTypes = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.EpisodeOfCareType);
        }

        protected void SetTelecomViewBags(bool isTelecomUseType)
        {
            ViewBag.TelecomSystem = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.TelecomSystemType);
            ViewBag.TelecomUse = isTelecomUseType ? SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.TelecomUseType)
                : SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.TelecommunicationUseType);
        }

        protected void SetCountryNameIfFilterByCountryIsIncluded(dynamic dataIn)
        {
            if (dataIn != null)
            {
                string countryName = string.Empty;
                if (dataIn.CountryCD != null)
                {
                    countryName = SingletonDataContainer.Instance.GetCodePreferredTerm(dataIn.CountryCD);
                }
                dataIn.CountryName = countryName;
            }
        }

        public void RefreshCache(int? resourceId = null, ModifiedResourceType? modifiedResourceType = null, bool callAsyncRunner = true)
        {
            if (callAsyncRunner)
            {
                _asyncRunner.Run<BaseController>((controller) => controller.RefreshCache(resourceId, modifiedResourceType, callAsyncRunner: false));
            }
            else
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
                    var codeAliasViewDAL = scope.ServiceProvider.GetRequiredService<ICodeAliasViewDAL>();
                    var codeDAL = scope.ServiceProvider.GetRequiredService<ICodeDAL>();
                    SingletonDataContainer.Instance.RefreshSingleton(mapper, codeAliasViewDAL, codeDAL, resourceId, modifiedResourceType);
                }
            }
        }

        private void SetUserCookieDataForSReports(string email, bool isEmail = true)
        {
            userCookieData = UserCookieDataHelper.PrepareUserCookie(_serviceProvider, isEmail, identifier: email, shouldResetOrganizations: false);
            HttpContext.Session.SetObjectAsJson("userData", userCookieData);
        }

        private void SetUserCookieDataForThesaurusGlobal(string email)
        {
            userCookieData = UserCookieDataHelper.PrepareUserCookieForThGlobal(_serviceProvider, email);
            HttpContext.Session.SetObjectAsJson("userData", userCookieData);
        }

        private void SetLocalDateFormat()
        {
            if (userCookieData != null)
            {
                ViewBag.DateFormatClient = DateConstants.DateFormatClient;
                ViewBag.DateFormatDisplay = DateConstants.DateFormatDisplay;
                ViewBag.TimeFormatDisplay = DateConstants.TimeFormatDisplay;
                ViewBag.DateFormat = DateConstants.DateFormat;
            }
        }

        private void SetCustomViewBags()
        {
            if (userCookieData != null)
            {
                ViewBag.IsDateCaptureMode = userCookieData.ActiveOrganization == ResourceTypes.OrganizationIdForDataCaptureMode && !userCookieData.UserHasAnyOfRole(PredifinedRole.SuperAdministrator.ToString());
                ViewBag.ReadOnly = false;
            }
        }
    }
}
