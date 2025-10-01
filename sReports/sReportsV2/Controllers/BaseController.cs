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
using sReportsV2.BusinessLayer.Interfaces;

namespace sReportsV2.Controllers
{
    public class BaseController : Controller
    {
        protected UserCookieData userCookieData;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IServiceProvider _serviceProvider;
        protected readonly IAsyncRunner _asyncRunner;
        protected readonly ICacheRefreshService _cacheRefreshService;
        public IConfiguration Configuration { get; }

        public BaseController(IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider, IConfiguration configuration, IAsyncRunner asyncRunner, ICacheRefreshService cacheRefreshService)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _asyncRunner = asyncRunner ?? throw new ArgumentNullException(nameof(asyncRunner));
            _cacheRefreshService = cacheRefreshService;
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

        protected void SetGenderTypesToViewBag()
        {
            ViewBag.Genders = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.Gender);
            ViewBag.MaleCodeId = SingletonDataContainer.Instance.GetCodeIdForPreferredTerm(Gender.Male.ToString(), (int)CodeSetList.Gender);
            ViewBag.FemaleCodeId = SingletonDataContainer.Instance.GetCodeIdForPreferredTerm(Gender.Female.ToString(), (int)CodeSetList.Gender);
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

        protected void RefreshCache(int? resourceId = null, ModifiedResourceType? modifiedResourceType = null, bool callAsyncRunner = true)
        {
            _asyncRunner.Run<ICacheRefreshService>((service) =>
            {
                service.RefreshCache(resourceId, modifiedResourceType, callAsyncRunner: false);
            });
        }

        protected void SetFileNameInResponse(string fileName, string fileType = "")
        {
            string fullName = string.IsNullOrEmpty(fileType) ? fileName : $"{fileName}.{fileType}";
            HttpContext.Response.Headers.Append("Original-File-Name", fullName);
        }

        private void SetUserCookieDataForSReports(string email, bool isEmail = true)
        {
            userCookieData = UserCookieDataHelper.PrepareUserCookie(_serviceProvider, isEmail, identifier: email);
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
                ViewBag.DateFormatClient = DateTimeConstants.DateFormatClient;
                ViewBag.DateFormatDisplay = DateTimeConstants.DateFormatDisplay;
                ViewBag.TimeFormatDisplay = DateTimeConstants.TimeFormatDisplay;
                ViewBag.DateFormat = DateTimeConstants.DateFormat;
            }
        }

        private void SetCustomViewBags()
        {
            if (userCookieData != null)
            {
                ViewBag.IsDateCaptureMode = userCookieData.ActiveOrganization == ResourceTypes.OrganizationIdForDataCaptureMode && !userCookieData.UserHasAnyOfRole(PredifinedRole.SuperAdministrator.ToString());
                ViewBag.ReadOnly = false;
                ViewBag.ArchivedUserStateCD = SingletonDataContainer.Instance.GetCodeIdForPreferredTerm(CodeAttributeNames.Archived, (int)CodeSetList.UserState);
            }
        }
    }
}
