using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Cache.Singleton;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Entities.User;
using sReportsV2.Common.Enums;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.DTOs.GlobalThesaurus.DataIn;
using sReportsV2.DTOs.DTOs.GlobalThesaurus.DataOut;
using sReportsV2.DTOs.DTOs.GlobalThesaurusUser.DataIn;
using sReportsV2.DTOs.DTOs.GlobalThesaurusUser.DataOut;
using sReportsV2.DTOs.O4CodeableConcept.DataIn;
using sReportsV2.DTOs.O4CodeableConcept.DataOut;
using sReportsV2.DTOs.Pagination;
using sReportsV2.DTOs.ThesaurusEntry;
using sReportsV2.DTOs.ThesaurusEntry.DataOut;
using sReportsV2.DTOs.User.DataIn;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using sReportsV2.Common.Extensions;
using Microsoft.AspNetCore.Http;
using System;
using sReportsV2.Common.Exceptions;

namespace sReportsV2.Controllers
{
    public class ThesaurusGlobalController : BaseController
    {
        private readonly IGlobalUserBLL globalUserBLL;
        private readonly IThesaurusEntryBLL thesaurusBLL;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;

        public ThesaurusGlobalController(IGlobalUserBLL globalUserBLL, 
            IThesaurusEntryBLL thesaurusBLL,
            IMapper mapper, 
            IConfiguration configuration, 
            IHttpContextAccessor httpContextAccessor, 
            IServiceProvider serviceProvider, 
            IAsyncRunner asyncRunner,
            ICacheRefreshService cacheRefreshService) : 
            base(httpContextAccessor, serviceProvider, configuration, asyncRunner, cacheRefreshService)
        {
            this.globalUserBLL = globalUserBLL;
            this.thesaurusBLL = thesaurusBLL;
            this.mapper = mapper;
            this.configuration = configuration;
        }

        public IActionResult Index()
        {
            return View("~/Views/ThesaurusGlobal/Home/Home.cshtml");
        }

        [HttpPost]
        public IActionResult RegisterUser(GlobalThesaurusUserDataIn userDataIn)
        {
            var globalUserSources = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.GlobalUserSource);
            int? internalSourceCD = globalUserSources?.Find(x => x.Thesaurus.Translations.Exists(m => m.PreferredTerm == CodeAttributeNames.Internal))?.Id;
            userDataIn.SourceCD = internalSourceCD;
            if (globalUserBLL.ExistByEmailAndSource(userDataIn.Email, userDataIn.SourceCD))
            {
                throw new UserAdministrationException(StatusCodes.Status400BadRequest, "User with given email is already registered!");
            }
            globalUserBLL.InsertOrUpdate(userDataIn);
            return Ok();
        }

        [HttpPost]
        public IActionResult Login(UserLoginDataIn userDataIn)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "ThesaurusGlobal");
            }
            userDataIn = Ensure.IsNotNull(userDataIn, nameof(userDataIn));

            if (ModelState.IsValid)
            {
                GlobalThesaurusUserDataOut user = globalUserBLL.TryLoginUser(userDataIn.Username, userDataIn.Password);
                if (user != null)
                {
                    SignInUser(user);
                    if (!string.IsNullOrEmpty(userDataIn.ReturnUrl))
                    {
                        return Redirect(userDataIn.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "ThesaurusGlobal");
                    }
                }
            }

            ViewBag.IsLogin = true;
            ModelState.AddModelError("General", "Invalid Username or Password");

            return View("~/Views/User/LoginGlobal.cshtml");
        }

        public IActionResult Browse(GlobalThesaurusFilterDataIn filter)
        {
            ViewBag.FilterData = filter;
            return View("~/Views/ThesaurusGlobal/Search/Browse.cshtml");
        }

        [Authorize(Roles = SmartOncologyRoleNames.EditorOrCurator)]
        public IActionResult Create(CodesFilterDataIn filterDataIn)
        {
            ThesaurusEntryDataOut dataOut = thesaurusBLL.GetThesaurusByFilter(filterDataIn);
            ViewBag.FilterData = filterDataIn;
            ViewBag.CodeSystems = SingletonDataContainer.Instance.GetCodeSystems();
            ViewBag.ThesaurusStates = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.ThesaurusState);

            return View("~/Views/ThesaurusGlobal/Create/Create.cshtml", dataOut);
        }

        [Authorize(Roles = SmartOncologyRoleNames.EditorOrCurator)]
        [HttpPost]
        public IActionResult Create(ThesaurusEntryDataIn thesaurusDataIn)
        {
            int result = thesaurusBLL.TryInsertOrUpdate(thesaurusDataIn, mapper.Map<UserData>(userCookieData));

            return Content(result.ToString());
        }

        [Authorize(Roles = SmartOncologyRoleNames.EditorOrCurator)]
        [HttpPost]
        public IActionResult SubmitConnectionWithOntology(ThesaurusEntryDataIn thesaurusDataIn)
        {
            int result = thesaurusBLL.UpdateConnectionWithOntology(thesaurusDataIn, mapper.Map<UserData>(userCookieData));

            return Content(result.ToString());
        }

        [Authorize(Roles = SmartOncologyRoleNames.EditorOrCurator)]
        [HttpPost]
        public IActionResult CreateCode(O4CodeableConceptDataIn codeDataIn, int? thesaurusEntryId)
        {
            ResourceCreatedDTO result = thesaurusBLL.TryInsertOrUpdateCode(codeDataIn, thesaurusEntryId);

            return Json(result);
        }

        [Authorize(Roles = SmartOncologyRoleNames.EditorOrCurator)]
        [HttpDelete]
        public IActionResult DeleteCode(int thesaurusId, int codeId)
        {
            thesaurusBLL.DeleteCode(codeId);

            return Content(thesaurusId.ToString());
        }

        public IActionResult PreviewThesaurus(CodesFilterDataIn filterDataIn)
        {
            ThesaurusEntryDataOut dataOut = thesaurusBLL.GetThesaurusByFilter(filterDataIn);
            ViewBag.FilterData = filterDataIn;
            ViewBag.CodeSystems = SingletonDataContainer.Instance.GetCodeSystems();

            return View("~/Views/ThesaurusGlobal/Preview/ThesaurusPreview.cshtml", dataOut);
        }

        public IActionResult ReloadThesaurus(GlobalThesaurusFilterDataIn filterDataIn)
        {
            var result = thesaurusBLL.ReloadThesaurus(filterDataIn);
            ViewBag.FilterData = filterDataIn;

            return PartialView("~/Views/ThesaurusGlobal/Search/GlobalThesaurusTable.cshtml", result);
        }

        [HttpPost]
        public IActionResult ReloadCodes(CodesFilterDataIn filterDataIn)
        {
            PaginationDataOut<O4CodeableConceptDataOut, CodesFilterDataIn> result = null;

            if (filterDataIn.Id != null)
            {
                result = thesaurusBLL.ReloadCodes(filterDataIn);
            }

            return PartialView("~/Views/ThesaurusGlobal/Preview/CodesTable.cshtml", result);
        }

        [Authorize(Roles = SmartOncologyRoleNames.EditorOrCurator)]
        public IActionResult ContributeToTranslation(CodesFilterDataIn filterDataIn)
        {
            ThesaurusEntryDataOut thesaurusDataOut = thesaurusBLL.GetThesaurusByFilter(filterDataIn);
            ViewBag.FilterData = filterDataIn;
            ViewBag.CodeSystems = SingletonDataContainer.Instance.GetCodeSystems();
            ViewBag.ReturnUrl = filterDataIn.ReturnUrl;
            ViewBag.ThesaurusStates = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.ThesaurusState);

            return View("~/Views/ThesaurusGlobal/Create/ContributeToTranslation.cshtml", thesaurusDataOut);
        }

        [Authorize(Roles = SmartOncologyRoleNames.EditorOrCurator)]
        [HttpPost]
        public IActionResult ContributeToTranslation(ThesaurusEntryTranslationDataIn thesaurusEntryTranslationDataIn)
        {
            Ensure.IsNotNull(thesaurusEntryTranslationDataIn, nameof(thesaurusEntryTranslationDataIn));
            ThesaurusEntryDataOut thesaurusDataOut = thesaurusBLL.UpdateTranslation(thesaurusEntryTranslationDataIn, mapper.Map<UserData>(userCookieData));

            ViewBag.CurrentTargetLanguage = thesaurusEntryTranslationDataIn.Language;
            ViewBag.ThesaurusStates = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.ThesaurusState);

            return View("~/Views/ThesaurusGlobal/Create/ContributeToTranslation.cshtml", thesaurusDataOut);
        }

        public IActionResult Technology()
        {
            return View("~/Views/ThesaurusGlobal/GeneralInfo/Technology.cshtml");
        }

        public IActionResult Contact()
        {
            ViewBag.ReCaptchaSiteKey = configuration["ReCaptchaSiteKey"];
            return View("~/Views/ThesaurusGlobal/GeneralInfo/Contact.cshtml");
        }

        [HttpPost]
        public IActionResult Contact(ContactFormDataIn contactFormData)
        {
            string secretKey = configuration["ReCaptchaSecretKey"];
            bool reCaptchaInputValid = globalUserBLL.IsReCaptchaInputValid(Request.Form["g-recaptcha-response"], secretKey);

            if (reCaptchaInputValid)
            {
                globalUserBLL.SubmitContactForm(contactFormData);
            }
            return Json(new { reCaptchaInputValid });
        }

        public IActionResult TermsAndConditions()
        {
            return View("~/Views/ThesaurusGlobal/GeneralInfo/TermsAndConditions.cshtml");
        }

        public IActionResult GetTotalChartData()
        {
            ThesaurusGlobalCountDataOut result = thesaurusBLL.GetThesaurusGlobalChartData();
            return Json(result);
        }

        [Authorize(Roles = SmartOncologyRoleNames.SuperAdministrator)]
        public IActionResult Users()
        {
            var data = globalUserBLL.GetUsers();
            ViewBag.Roles = globalUserBLL.GetRoles().Skip(1);
            ViewBag.GlobalUserStatuses = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.GlobalUserStatus);

            return View("~/Views/ThesaurusGlobal/Administration/UserList.cshtml", data);
        }

        public IActionResult ActivateUser(string email)
        {
            globalUserBLL.ActivateUser(email);

            return RedirectToAction("Login", "User");
        }

        [HttpPut]
        public IActionResult SetUserStatus(int userId, int? newStatus)
        {
            globalUserBLL.SetUserStatus(userId, newStatus);
            return Ok();
        }

        [HttpPut]
        public IActionResult UpdateUserRoles(GlobalThesaurusUserDataIn userDataIn)
        {
            globalUserBLL.UpdateRoles(userDataIn);
            return Ok();
        }

        private void SignInUser(GlobalThesaurusUserDataOut user)
        {
            globalUserBLL.ActivateUser(user.Email);
            var claimsIdentity = new ClaimsIdentity(user.GetClaims(), "Cookies");

            // This uses ASP.NET Core authentication
            HttpContext.SignInAsync("Cookies", new ClaimsPrincipal(claimsIdentity)).Wait();
        }
    }
}
