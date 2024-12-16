using AutoMapper;
using Newtonsoft.Json.Linq;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Enums;
using sReportsV2.BusinessLayer.Components.Interfaces;
using sReportsV2.Domain.Sql.Entities.GlobalThesaurusUser;
using sReportsV2.DTOs.DTOs.AccessManagment.DataOut;
using sReportsV2.DTOs.DTOs.GlobalThesaurus.DataIn;
using sReportsV2.DTOs.DTOs.GlobalThesaurusUser.DataIn;
using sReportsV2.DTOs.DTOs.GlobalThesaurusUser.DataOut;
using sReportsV2.SqlDomain.Interfaces;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.BusinessLayer.Helpers;
using Microsoft.Extensions.Configuration;
using sReportsV2.DAL.Sql.Interfaces;

namespace sReportsV2.BusinessLayer.Implementations
{
    public class GlobalUserBLL : IGlobalUserBLL
    {
        private readonly IGlobalThesaurusUserDAL globalUserDAL;
        private readonly IGlobalThesaurusRoleDAL globalThesaurusRoleDAL;
        private readonly ICodeDAL codeDAL;
        private readonly IEmailSender emailSender;
        private readonly IConfiguration configuration;
        private readonly IMapper Mapper;

        public GlobalUserBLL(IGlobalThesaurusUserDAL globalUserDAL, IGlobalThesaurusRoleDAL globalThesaurusRoleDAL, ICodeDAL codeDAL, IEmailSender emailSender, IConfiguration configuration, IMapper mapper)
        {
            this.globalUserDAL = globalUserDAL;
            this.globalThesaurusRoleDAL = globalThesaurusRoleDAL;
            this.codeDAL = codeDAL;
            this.emailSender = emailSender;
            this.configuration = configuration;
            Mapper = mapper;
        }

        public void ActivateUser(string email)
        {
            var user = globalUserDAL.GetByEmail(email);
            int? activeStatusCD = codeDAL.GetByCodeSetIdAndPreferredTerm((int)CodeSetList.GlobalUserStatus, CodeAttributeNames.Active);
            int? notVerifiedStatusCD = codeDAL.GetByCodeSetIdAndPreferredTerm((int)CodeSetList.GlobalUserStatus, CodeAttributeNames.NotVerified);

            if (user != null && user.StatusCD == notVerifiedStatusCD)
            {
                user.StatusCD = activeStatusCD;
                globalUserDAL.InsertOrUpdate(user);
            }
        }

        public bool ExistByEmailAndSource(string email, int? sourceCD)
        {
            return globalUserDAL.ExistByEmailAndSource(email, sourceCD);
        }

        public GlobalThesaurusUserDataOut GetByEmail(string email)
        {
            return Mapper.Map<GlobalThesaurusUserDataOut>(globalUserDAL.GetByEmail(email));
        }

        public List<RoleDataOut> GetRoles()
        {
            return Mapper.Map<List<RoleDataOut>>(globalThesaurusRoleDAL.GetAll());
        }

        public List<GlobalThesaurusUserDataOut> GetUsers()
        {
            List<GlobalThesaurusUser> filteredUsers = new List<GlobalThesaurusUser>();
            foreach (var user in globalUserDAL.GetAll())
            {
                if (!user.HasRole(SmartOncologyRoleNames.SuperAdministrator))
                {
                    filteredUsers.Add(user);
                }
            }
            return Mapper.Map<List<GlobalThesaurusUserDataOut>>(filteredUsers);
        }

        public GlobalThesaurusUserDataOut InsertOrUpdate(GlobalThesaurusUserDataIn user)
        {
            GlobalThesaurusUser userDb = Mapper.Map<GlobalThesaurusUser>(user);
            if (userDb.GlobalThesaurusUserId == 0)
            {
                SetPredifinedRole(userDb, PredifinedGlobalUserRole.Viewer);
                SendActivationLink(userDb);
            }
            return Mapper.Map<GlobalThesaurusUserDataOut>(globalUserDAL.InsertOrUpdate(userDb));
        }

        public bool IsReCaptchaInputValid(string response, string secretKey)
        {
            using (var client = new WebClient())
            {
                var result = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secretKey, response));
                var obj = JObject.Parse(result);
                bool reCaptchaInputSuccess = (bool)obj.SelectToken("success");
                var score = obj.SelectToken("score");
                double reCaptchaInputScore = score != null ? (double)score : 0;

                return reCaptchaInputSuccess && reCaptchaInputScore > 0.5;
            }
        }

        public void SetUserStatus(int id, int? status)
        {
            var user = globalUserDAL.GetById(id);
            if (user != null)
            {
                user.StatusCD = status;
                globalUserDAL.InsertOrUpdate(user);
            }
        }

        public void SubmitContactForm(ContactFormDataIn contactFormData)
        {
            string mailContent = EmailHelpers.GetGlobalThesaurusContactFormEmailContent(contactFormData);
            string hostEmail = configuration["AppEmail"];

            Task.Run(() => emailSender.SendAsync(new EmailDTO(contactFormData.Email, mailContent, $"{EmailSenderNames.SoftwareName} Contact Form")));
            Task.Run(() => emailSender.SendAsync(new EmailDTO(hostEmail, mailContent, $"{EmailSenderNames.SoftwareName} Contact Form")));
        }

        public GlobalThesaurusUserDataOut TryLoginUser(string username, string password)
        {
            GlobalThesaurusUser user = null;
            int? activeStatusCD = codeDAL.GetByCodeSetIdAndPreferredTerm((int)CodeSetList.GlobalUserStatus, CodeAttributeNames.Active);

            if (globalUserDAL.IsValidUser(username, password, activeStatusCD))
            {
                user = globalUserDAL.GetByEmail(username);
            }
            return Mapper.Map<GlobalThesaurusUserDataOut>(user);
        }

        public void UpdateRoles(GlobalThesaurusUserDataIn user)
        {
            GlobalThesaurusUser userDb = globalUserDAL.GetById(user.Id);
            userDb.UpdateRoles(user.Roles);
            globalUserDAL.InsertOrUpdate(userDb);
        }

        private void SendActivationLink(GlobalThesaurusUser user)
        {
            string mailContent = EmailHelpers.GetGlobalThesaurusActivationLinkEmailContent(user);
            Task.Run(() => emailSender.SendAsync(new EmailDTO(user.Email, mailContent, $"{EmailSenderNames.SoftwareName} Registration")));
        }

        private void SetPredifinedRole(GlobalThesaurusUser user, PredifinedGlobalUserRole predifinedGlobalUserRole)
        {
            GlobalThesaurusRole role = globalThesaurusRoleDAL.GetByName(predifinedGlobalUserRole.ToString());
            user.UpdateRoles(new List<int>() { role.GlobalThesaurusRoleId });
        }
    }
}
