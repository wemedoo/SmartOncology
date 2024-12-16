using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using sReportsV2.Common.Enums;
using sReportsV2.Domain.Sql.Entities.GlobalThesaurusUser;
using sReportsV2.DTOs.DTOs.GlobalThesaurusUser.DataOut;
using sReportsV2.SqlDomain.Interfaces;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace sReportsV2.Common.Helpers
{
    public class AccountService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGlobalThesaurusUserDAL _globalThesaurusUserDAL;
        private readonly IGlobalThesaurusRoleDAL _globalThesaurusRoleDAL;
        private readonly IMapper _mapper;

        public AccountService(IHttpContextAccessor httpContextAccessor, IGlobalThesaurusUserDAL globalThesaurusUserDAL, IGlobalThesaurusRoleDAL globalThesaurusRoleDAL, IMapper mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _globalThesaurusRoleDAL = globalThesaurusRoleDAL;
            _globalThesaurusUserDAL = globalThesaurusUserDAL;
            _mapper = mapper;
        }

        public async Task SignOutAsync()
        {
            var context = _httpContextAccessor.HttpContext;

            // Clear session data
            context.Session.Clear();

            // Clear any custom user data
            context.Session.Remove("userData");

            // Sign out
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public async Task SignInUserAsync(List<Claim> claims)
        {
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = false
            };

            await _httpContextAccessor.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties).ConfigureAwait(false);
        }

        public async Task SignExternalUser(ClaimsPrincipal user, int? sourceCD, int? activeStatusCD)
        {
            await AddUserIfNotExist(user, sourceCD, activeStatusCD);

            var globalThesaurusUser = _globalThesaurusUserDAL.GetByEmail(user.FindFirstValue(ClaimTypes.Email));
            if (user != null)
            {
                var userDataOut = _mapper.Map<GlobalThesaurusUserDataOut>(globalThesaurusUser);
                await SignInUserAsync(userDataOut.GetClaims());
            }
        }

        private async Task AddUserIfNotExist(ClaimsPrincipal user, int? sourceCD, int? activeStatusCD)
        {
            string email = user.FindFirstValue(ClaimTypes.Email);

            if (!_globalThesaurusUserDAL.ExistByEmailAndSource(email, sourceCD))
            {
                GlobalThesaurusUser userDb = new GlobalThesaurusUser()
                {
                    Email = email,
                    FirstName = user.FindFirstValue(ClaimTypes.GivenName),
                    LastName = user.FindFirstValue(ClaimTypes.Surname),
                    SourceCD = sourceCD,
                    StatusCD = activeStatusCD
                };
                SetPredifinedRole(userDb);
                _globalThesaurusUserDAL.InsertOrUpdate(userDb);
            }
        }

        private void SetPredifinedRole(GlobalThesaurusUser user)
        {
            GlobalThesaurusRole viewerRole = _globalThesaurusRoleDAL.GetByName(PredifinedGlobalUserRole.Viewer.ToString());
            user.UpdateRoles(new List<int>() { viewerRole.GlobalThesaurusRoleId });
        }
    }
}