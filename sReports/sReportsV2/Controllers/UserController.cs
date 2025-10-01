using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Extensions;
using sReportsV2.DTOs.Common.DataOut;
using sReportsV2.DTOs.User.DataIn;
using sReportsV2.DTOs.Autocomplete;
using System.Linq;
using Serilog;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authentication.Google;
using sReportsV2.Common.Enums;
using Microsoft.AspNetCore.Authentication.Cookies;
using sReportsV2.Common.Constants;
using sReportsV2.Cache.Singleton;
using sReportsV2.Common.Helpers;

namespace sReportsV2.Controllers
{
    public partial class UserController : Controller
    {
        protected IUserBLL userBLL;
        protected IOrganizationBLL organizationBLL;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AccountService _accountService;
        private readonly IConfiguration configuration;

        public UserController(IUserBLL userBLL, IOrganizationBLL organizationBLL, IHttpContextAccessor httpContextAccessor, IConfiguration configuration, AccountService accountService)
        {
            this.userBLL = userBLL;
            this.organizationBLL = organizationBLL;
            this._httpContextAccessor = httpContextAccessor;
            this._accountService = accountService;
            this.configuration = configuration;
        }
        [HttpGet]
        [ResponseCache(NoStore = true, Duration = 0)]
        public ActionResult Login(string returnUrl, bool isLogin = true)
        {
            if (User.Identity.IsAuthenticated)
            {
                Log.Information("Already signed in");
                if (string.IsNullOrEmpty(returnUrl))
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return Redirect(returnUrl);
                }
            }

            ViewBag.IsLogin = isLogin;
            ViewBag.ReturnUrl = returnUrl != "/User/Logout" ? returnUrl : ResourceTypes.DefaultEndpoint;
            string loginView = configuration["LoginViewName"];

            return View(loginView);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login(UserLoginDataIn userDataIn)
        {
            if (User.Identity.IsAuthenticated)
            {
                Log.Information("Already signed in");
                if (string.IsNullOrEmpty(userDataIn?.ReturnUrl))
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return Redirect(userDataIn.ReturnUrl);
                }
            }
            userDataIn = Ensure.IsNotNull(userDataIn, nameof(userDataIn));

            if (ModelState.IsValid)
            {
                UserDataOut userDataOut = userBLL.TryLoginUser(userDataIn);
                var userStates = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.UserState);
                int? blockedUserStateCD = userStates?.Find(x => x.Thesaurus.Translations.Exists(m => m.PreferredTerm == CodeAttributeNames.Blocked))?.Id;
                int? activeUserStateCD = userStates?.Find(x => x.Thesaurus.Translations.Exists(m => m.PreferredTerm == CodeAttributeNames.Active))?.Id;

                if (userDataOut == null || userDataOut.IsBlockedInEveryOrganization(blockedUserStateCD))
                {
                    AddError();
                    return View(userDataIn);
                }

                if (userDataOut.HasNoAnyActiveOrganization(activeUserStateCD))
                {
                    return View("~/Views/User/ChooseActiveOrganization.cshtml");
                }

                await _accountService.SignInUserAsync(userDataOut.GetClaims(), userDataOut.Email);

                if (userDataOut.HasToChooseActiveOrganization)
                {
                    return RedirectToAction("ChooseActiveOrganization", new { userId = userDataOut.Id });
                }

                if (string.IsNullOrEmpty(userDataIn.ReturnUrl))
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return Redirect(userDataIn.ReturnUrl);
                }
            }
            else
            {
                return View(userDataIn);
            }
        }

        [Authorize]
        public ActionResult ChooseActiveOrganization(int userId)
        {
            var userStates = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.UserState);
            int? activeUserStateCD = userStates?.Find(x => x.Thesaurus.Translations.Exists(m => m.PreferredTerm == CodeAttributeNames.Active))?.Id;

            return View("~/Views/User/ChooseActiveOrganization.cshtml", userBLL.GetById(userId).GetActiveOrganizations(activeUserStateCD).ToList());
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> Logout(string returnUrl = ResourceTypes.DefaultEndpoint)
        {
            if (User.Identity.IsAuthenticated)
            {
                await SignOutAsync();    
            }

            if (configuration.IsSReportsRunning())
            {
                return RedirectToAction("Login", new { returnUrl });
            }
            else
            {
                return RedirectToAction("Index", configuration["DefaultController"]);
            }
        }

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GeneratePassword(string email)
        {
            userBLL.GeneratePassword(email);
            return StatusCode(StatusCodes.Status201Created);
        }

        [AllowAnonymous]
        public IActionResult SignInExternal(string providerName, string returnUrl = ResourceTypes.DefaultEndpoint)
        {
            if (!User.Identity.IsAuthenticated)
            {
                var redirectUrl = Url.Action(nameof(SignInExternalCallback), "User", new { providerName, returnUrl });
                var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
                return Challenge(properties, GetAuthenticationScheme(providerName));
            }
            else
            {
                return RedirectToAction("Index", configuration["DefaultController"]);
            }
            
        }
        
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> SignInExternalCallback(string providerName, string returnUrl = ResourceTypes.DefaultEndpoint)
        {
            var authenticateResult = await HttpContext.AuthenticateAsync();
            if (!authenticateResult.Succeeded)
            {
                return RedirectToAction(nameof(Login));
            }

            var globalUserStatuses = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.GlobalUserStatus);
            int? sourceCD = GetSourceCD(providerName);
            int? activeStatusCD = globalUserStatuses?.Find(x => x.Thesaurus.Translations.Exists(m => m.PreferredTerm == CodeAttributeNames.Active))?.Id;

            await _accountService.SignExternalUser(User, sourceCD, activeStatusCD);

            return LocalRedirect(returnUrl);
        }

        private void AddError()
        {
            ModelState.AddModelError("General", "Invalid Username or Password");
        }

        private async Task<IActionResult> SignOutAsync()
        {
            await _accountService.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        private string GetAuthenticationScheme(string providerName)
        {
            switch (providerName)
            {
                case CodeAttributeNames.Microsoft:
                    return MicrosoftAccountDefaults.AuthenticationScheme;
                case CodeAttributeNames.Google:
                    return GoogleDefaults.AuthenticationScheme;
                default:
                    return CookieAuthenticationDefaults.AuthenticationScheme;
            }
        }

        private int? GetSourceCD(string providerName)
        {
            var globalUserSources = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.GlobalUserSource);
            int? internalSourceCD = globalUserSources?.Find(x => x.Thesaurus.Translations.Exists(m => m.PreferredTerm == CodeAttributeNames.Internal))?.Id;
            int? microsoftSourceCD = globalUserSources?.Find(x => x.Thesaurus.Translations.Exists(m => m.PreferredTerm == CodeAttributeNames.Microsoft))?.Id;
            int? googleSourceCD = globalUserSources?.Find(x => x.Thesaurus.Translations.Exists(m => m.PreferredTerm == CodeAttributeNames.Google))?.Id;

            switch (providerName)
            {
                case CodeAttributeNames.Microsoft:
                    return microsoftSourceCD;
                case CodeAttributeNames.Google:
                    return googleSourceCD;
                default:
                    return internalSourceCD;
            }
        }
    }
}