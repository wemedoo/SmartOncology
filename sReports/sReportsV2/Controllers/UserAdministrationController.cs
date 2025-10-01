using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Constants;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Extensions;
using sReportsV2.Common.Helpers;
using sReportsV2.Cache.Singleton;
using sReportsV2.DTOs.Common.DataOut;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.DTOs.AccessManagment.DataOut;
using sReportsV2.DTOs.DTOs.User.DataIn;
using sReportsV2.DTOs.User.DataIn;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using sReportsV2.Cache.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using Microsoft.Extensions.Configuration;
using sReportsV2.Common.Exceptions;
using System.Threading.Tasks;
using sReportsV2.DTOs.User.DTO;

namespace sReportsV2.Controllers
{
    public class UserAdministrationController : BaseController
    {
        private readonly IUserBLL userBLL;

        public UserAdministrationController(IUserBLL userBLL, 
            IHttpContextAccessor httpContextAccessor, 
            IServiceProvider serviceProvider, 
            IConfiguration configuration, 
            IAsyncRunner asyncRunner,
            ICacheRefreshService cacheRefreshService) :
            base(httpContextAccessor, serviceProvider, configuration, asyncRunner, cacheRefreshService)
        {
            this.userBLL = userBLL;
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Administration)]
        public ActionResult GetAll(PersonnelFilterDataIn dataIn)
        {
            SetCountryNameIfFilterByCountryIsIncluded(dataIn);
            ViewBag.FilterData = dataIn;
            SetPersonnelFilterViewBags();
            return View();
        }

        [SReportsAuthorize]
        public ActionResult ReloadTable(PersonnelFilterDataIn dataIn)
        {
            dataIn.ActiveOrganization = userCookieData.ActiveOrganization;
            dataIn.HasAdminRole = HasAdminRole(userCookieData);

            var result = userBLL.ReloadTable(dataIn);
            SetPersonnelPositionViewBags();
            ViewBag.UserStates = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.UserState);

            return PartialView("UserEntryTable", result);
        }

        #region CRUD

        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.Administration)]
        public ActionResult Edit(int userId)
        {
            return GetUser(isUserAdministration: true, userId: userId);
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Administration)]
        public ActionResult View(int userId)
        {
            return GetUser(isUserAdministration: true, userId: userId);
        }

        [SReportsAuthorize(Permission = PermissionNames.Create, Module = ModuleNames.Administration)]
        public ActionResult Create()
        {
            return GetUser(isUserAdministration: true, shouldRetrieveUser: false);
        }

        [SReportsAuthorize(Permission = PermissionNames.Create, Module = ModuleNames.Administration)]
        [SReportsAuditLog]
        [HttpPost]
        [SReportsModelStateValidate]
        public async Task<ActionResult> Create(UserDataIn user)
        {
            return await CreateOrEdit(user).ConfigureAwait(false);
        }

        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.Administration)]
        [SReportsAuditLog]
        [HttpPost]
        [SReportsModelStateValidate]
        public async Task<ActionResult> Edit(UserDataIn user)
        {
            return await CreateOrEdit(user).ConfigureAwait(false);
        }

        [SReportsAuthorize(Permission = PermissionNames.Create, Module = ModuleNames.Administration)]
        [SReportsAuditLog]
        [HttpPost]
        [SReportsModelStateValidate]
        public async Task<ActionResult> CreateIdentifier(PersonnelIdentifierDataIn identifierDataIn)
        {
            return await CreateOrEdit(identifierDataIn).ConfigureAwait(false);
        }

        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.Administration)]
        [SReportsAuditLog]
        [HttpPost]
        [SReportsModelStateValidate]
        public async Task<ActionResult> EditIdentifier(PersonnelIdentifierDataIn identifierDataIn)
        {
            return await CreateOrEdit(identifierDataIn).ConfigureAwait(false);
        }

        [SReportsAuthorize(Permission = PermissionNames.Create, Module = ModuleNames.Administration)]
        [SReportsAuditLog]
        [HttpPost]
        [SReportsModelStateValidate]
        public async Task<ActionResult> CreateAddress(PersonnelAddressDataIn addressDataIn)
        {
            return await CreateOrEdit(addressDataIn).ConfigureAwait(false);
        }

        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.Administration)]
        [SReportsAuditLog]
        [HttpPost]
        [SReportsModelStateValidate]
        public async Task<ActionResult> EditAddress(PersonnelAddressDataIn addressDataIn)
        {
            return await CreateOrEdit(addressDataIn).ConfigureAwait(false);
        }

        [SReportsAuthorize(Permission = PermissionNames.Delete, Module = ModuleNames.Administration)]
        [HttpDelete]
        [SReportsAuditLog]
        public async Task<ActionResult> DeleteIdentifier(PersonnelIdentifierDataIn personnelIdentifierDataIn)
        {
            await userBLL.Delete(personnelIdentifierDataIn).ConfigureAwait(false);
            return NoContent();
        }

        [SReportsAuthorize(Permission = PermissionNames.Delete, Module = ModuleNames.Administration)]
        [HttpDelete]
        [SReportsAuditLog]
        public async Task<ActionResult> DeleteAddress(PersonnelAddressDataIn personnelAddressDataIn)
        {
            await userBLL.Delete(personnelAddressDataIn).ConfigureAwait(false);
            return NoContent();
        }
        #endregion /CRUD

        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.Administration)]
        [SReportsAuditLog]
        [HttpPost]
        public ActionResult UpdateUserOrganizations(UserDataIn userDataIn)
        {
            CreateResponseResult response = userBLL.UpdateOrganizations(userDataIn);
            return Json(response);
        }

        [SReportsAuthorize]
        [SReportsAuditLog]
        [HttpPost]
        public ActionResult LinkOrganization(LinkOrganizationDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            if (dataIn.OrganizationsIds != null && dataIn.OrganizationsIds.Contains(dataIn.OrganizationId))
            {
                throw new UserAdministrationException(StatusCodes.Status400BadRequest, TextLanguage.OrganizationExist);
            }

            var result = userBLL.LinkOrganization(dataIn);
            ViewBag.UserAdministration = true;
            ViewBag.UserStates = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.UserState);

            return PartialView("OrganizationData", result);
        }

        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.Administration)]
        [HttpPut]
        public ActionResult SetUserState(int userId, int organizationId, int? newState)
        {
            if (userBLL.UserExist(userId))
            {
                userBLL.SetState(userId, newState, organizationId);
            }
            else
            {
                return NotFound();
            }

            return Ok();
        }
        

        [SReportsAuthorize]
        public ActionResult UserProfile(int userId)
        {
            SetReadOnlyAndDisabledViewBag(false);
            return GetUser(isUserAdministration: true, userId: userId);
        }

        [SReportsAuthorize]
        public ActionResult DisplayUser(int userId)
        {
            SetReadOnlyAndDisabledViewBag(true);
            return GetUser(isUserAdministration: false, userId: userId);
        }

        [SReportsAuthorize]
        [SReportsAuditLog]
        [HttpPost]
        [SReportsModelStateValidate]
        public async Task<ActionResult> UpdateUserProfile(UserDataIn user)
        {
            CreateUserResponseResult response = await userBLL.InsertOrUpdate(user, userCookieData.ActiveLanguage);
            bool isEmail = user.Email != null;
            UpdateUserCookieIfNecessary(isEmail, isEmail ? user.Email : user.Username);
            return Json(response);
        }

        [SReportsAuthorize]
        [SReportsAuditLog(new string[] { "oldPassword", "newPassword", "confirmPassword"})]
        [HttpGet]
        public ActionResult ChangePassword(string oldPassword, string newPassword, string confirmPassword, string userId)
        {
            userBLL.ChangePassword(oldPassword, newPassword, confirmPassword, userId);
            return StatusCode(StatusCodes.Status201Created);
        }


        [SReportsAuthorize]
        public ActionResult CheckUsername(string username, int? userId)
        {
            return userBLL.IsUsernameValid(username, userId) ? Ok() : BadRequest();
        }

        [SReportsAuthorize]
        public ActionResult CheckEmail(string email, int? userId)
        {
            return userBLL.IsEmailValid(email, userId) ? Ok() : BadRequest();
        }

        [HttpPut]
        [Authorize]
        [SReportsAuditLog]
        public ActionResult UpdateOrganization(int organizationId)
        {
            userBLL.SetActiveOrganization(userCookieData, organizationId);
            //ResetCookie(userCookieData.Username, data.Value);
            //TO DO IMPORTANT: if we update roles to user organization level, we'll have to reset data in the context to update roles for the active org
            return Ok();
        }

        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.Administration)]
        public ActionResult CreateCustomAccessToken()
        {
            string token = AccessTokenHelper.GetAccessToken();

            return Json(new { token });
        }

        public ActionResult ExistIdentifier(PersonnelIdentifierDataIn dataIn)
        {
            return Json(!userBLL.ExistEntity(dataIn));
        }

        [SReportsAuditLog]
        public async Task<ActionResult> GetAutocompleteData(PersonnelAutocompleteDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            if (dataIn.FilterByDoctors)
            {
                dataIn.OrganizationId = userCookieData.ActiveOrganization;
            }
            return Json(await userBLL.GetAutocompleteData(dataIn).ConfigureAwait(false));
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Administration)]
        [HttpGet]
        public ActionResult ShowUserBasicInfo(int userId)
        {
            UserDataOut userData = userBLL.GetUserForEdit(userId);
            SetUserViewBags(isUserAdministration: true);
            return PartialView("UserBasicInfo", userData);
        }


        private void UpdateUserCookieIfNecessary(bool needsToBeUpdated, string email)
        {
            if(needsToBeUpdated)
            {
                UpdateUserCookie(email);
                UpdateClaims(new Dictionary<string, string>() { {ClaimTypes.Email, email}, { "preferred_username", email} });
            }
        }

        private ActionResult GetUser(bool isUserAdministration, int userId = 0, bool shouldRetrieveUser = true)
        {
            UserDataOut viewModel = null;
            if (shouldRetrieveUser)
            {
                viewModel = userBLL.GetUserForEdit(userId);
            }

            SetUserViewBags(isUserAdministration);
            
            return View("User", viewModel);
        }

        private void SetUserViewBags(bool isUserAdministration)
        {
            ViewBag.UserAdministration = isUserAdministration;
            SetPersonnelPositionViewBags();
            ViewBag.UserPrefixes = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.UserPrefix);
            ViewBag.PersonnelTypes = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.PersonnelType);
            ViewBag.IdentifierTypes = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.PatientIdentifierType);
            ViewBag.IdentifierUseTypes = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.IdentifierUseType);
            ViewBag.AddressTypes = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.AddressType);
            ViewBag.AcademicPositions = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.AcademicPosition).Where(x => x.IsActive()).ToList();
            ViewBag.InactiveAcademicPositions = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.AcademicPosition).Where(x => x.IsInactive());
            ViewBag.OccupationCategories = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.OccupationCategory);
            ViewBag.Occupations = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.Occupation);
            ViewBag.PersonnelSeniorities = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.PersonnelSeniority);
            ViewBag.OccupationSubCategories = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.OccupationSubCategory);
            ViewBag.MedicalDoctorCodeId = userBLL.GetMedicalDoctorsCodeId();
            ViewBag.Roles = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.Role).Where(x => x.IsActive());
            ViewBag.InactiveRoles = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.Role).Where(x => x.IsInactive());
            ViewBag.UserStates = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.UserState);
        }

        private void SetPersonnelFilterViewBags()
        {
            SetSuperAdministratorViewBags();
            SetPersonnelPositionViewBags();
            ViewBag.PersonnelTypes = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.PersonnelType);
            ViewBag.IdentifierTypes = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.PatientIdentifierType);
        }

        private void SetSuperAdministratorViewBags()
        {
            int? superAdministratorRoleCodeId = SingletonDataContainer.Instance.GetCodeId((int)CodeSetList.Role, SmartOncologyRoleNames.SuperAdministrator);
            ViewBag.ShowUnassignedUsers = (ViewBag.UserCookieData.PositionPermissions as List<PositionPermissionDataOut>).Exists(p => p.PositionId == superAdministratorRoleCodeId);
        }

        private void SetPersonnelPositionViewBags()
        {
            ViewBag.PersonnelPositions = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.Role);
        }

        private bool HasAdminRole(UserCookieData userCookieData)
        {
            var requiredRoleIds = new[]
            {
                RoleNames.SuperAdministrator,
                RoleNames.Administrator
            };

            var userRoleIds = userCookieData.Roles.Select(r => r.Name);

            return RoleHelper.UserHasAnyRole(userRoleIds, requiredRoleIds);
        }

        #region CRUD
        private async Task<JsonResult> CreateOrEdit(UserDataIn user)
        {
            CreateUserResponseResult response = await userBLL.InsertOrUpdate(user, userCookieData.ActiveLanguage).ConfigureAwait(false);
            UpdateUserCookieIfNecessary(response.Id == userCookieData.Id, user.Email);
            return Json(response);
        }

        private async Task<JsonResult> CreateOrEdit(PersonnelIdentifierDataIn identifierDataIn)
        {
            ResourceCreatedDTO resourceCreatedDTO = await userBLL.InsertOrUpdate(identifierDataIn).ConfigureAwait(false);

            if (identifierDataIn.Id == 0)
            {
                Response.StatusCode = 201;
            }
            return Json(resourceCreatedDTO);
        }

        private async Task<JsonResult> CreateOrEdit(PersonnelAddressDataIn addressDataIn)
        {
            ResourceCreatedDTO resourceCreatedDTO = await userBLL.InsertOrUpdate(addressDataIn).ConfigureAwait(false);

            if (addressDataIn.Id == 0)
            {
                Response.StatusCode = 201;
            }
            return Json(resourceCreatedDTO);
        }
        #endregion /CRUD
    }
}