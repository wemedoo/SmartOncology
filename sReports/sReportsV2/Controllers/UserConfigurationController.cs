using sReportsV2.Common.CustomAttributes;
using sReportsV2.Common.Extensions;
using sReportsV2.DTOs;
using sReportsV2.DTOs.User.DataIn;
using sReportsV2.DTOs.User.DTO;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace sReportsV2.Controllers
{
    public partial class UserController
    {
        [HttpPut]
        [SReportsAuditLog]
        [SReportsAuthorize]
        public ActionResult UpdateLanguage(string newLanguage)
        {
            newLanguage = Ensure.IsNotNull(newLanguage, nameof(newLanguage));
            UserCookieData userCookieData = GetSession().GetUserFromSession();
            this.userBLL.UpdateLanguage(newLanguage, userCookieData);

            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPut]
        [SReportsAuditLog]
        [SReportsAuthorize]
        public ActionResult UpdatePageSizeSettings(UserUpdatePageSizeDataIn data)
        {
            data = Ensure.IsNotNull(data, nameof(data));
            UserCookieData userCookieData = GetSession().GetUserFromSession();
            this.userBLL.UpdatePageSize(data.PageSize, userCookieData);

            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPut]
        [SReportsAuthorize]
        [SReportsAuditLog]
        public ActionResult AddSuggestedForm(string formId)
        {
            var session = GetSession();
            UserCookieData userCookieData = session.GetUserFromSession();
            userCookieData.SuggestedForms.Add(formId);
            this.userBLL.AddSuggestedForm(userCookieData.Username, formId);
            session.SetObjectAsJson("userData", userCookieData);

            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPut]
        [SReportsAuthorize]
        [SReportsAuditLog]
        public ActionResult RemoveSuggestedForm(string formId)
        {
            var session = GetSession();
            UserCookieData userCookieData = session.GetUserFromSession();
            this.userBLL.RemoveSuggestedForm(userCookieData.Username, formId);
            userCookieData.SuggestedForms.RemoveAt(userCookieData.SuggestedForms.IndexOf(formId));
            session.SetObjectAsJson("userData", userCookieData);

            return StatusCode(StatusCodes.Status201Created);
        }

        private ISession GetSession()
        {
            return _httpContextAccessor.HttpContext.Session;
        }
    }
}