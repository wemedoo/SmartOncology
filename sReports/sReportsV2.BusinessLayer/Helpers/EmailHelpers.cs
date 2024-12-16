using sReportsV2.Common.Constants;
using sReportsV2.Domain.Entities.Consensus;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Sql.Entities.GlobalThesaurusUser;
using sReportsV2.Domain.Sql.Entities.User;
using sReportsV2.DTOs.DTOs.GlobalThesaurus.DataIn;
using sReportsV2.DTOs.User.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using sReportsV2.Common.Extensions;
using System.Web;
using sReportsV2.Common.Enums;
using sReportsV2.Domain.Sql.Entities.OutsideUser;
using sReportsV2.DTOs.DTOs.FormConsensus.DTO;

namespace sReportsV2.BusinessLayer.Helpers
{
    public static class EmailHelpers
    {
        private const string IfLoginLinkDoesNotWorkSentence = "If clicking the login link doesn't work, you can copy and paste it into your browser.";

        public static string GetRegistrationEmailContent(Personnel user, string generatedPassword)
        {
            string url = $"{GetDomain()}/User/Login?ReturnUrl=%2f";
            string mailContent = $@"<div> 
                Dear {user.GetFirstAndLastName()},<br><br>
                Welcome to the {EmailSenderNames.SoftwareName} clinical information system. The following are your credentials to log in. <br><br>
                Your username is: <b>{user.Username}</b><br>
                Your password is: <b>{generatedPassword}</b><br><br>
                You can use the following link to log into the {EmailSenderNames.SoftwareName}: <a href='{url}'>Login</a><br><br>
                {IfLoginLinkDoesNotWorkSentence}<br><br>
                Once you log into the system the first time, please change your password to one that you can remember and do not share it with anyone else.
                {GetFooter()}
                </div>";
            return mailContent;
        }

        public static string GetResetEmailContent(Personnel user, string generatedPassword)
        {
            string url = $"{GetDomain()}/User/Login?ReturnUrl=%2f";
            string mailContent = $@"<div> 
                Dear {user.GetFirstAndLastName()},<br><br>
                We received a request to reset the password for your SmartOncology(c) account. The following are your credentials to log into {EmailSenderNames.SoftwareName}.<br><br>
                Your username is: <b>{user.Username}</b><br>
                Your password is: <b>{generatedPassword}</b><br><br>
                You can use the following link to log into the {EmailSenderNames.SoftwareName}: <a href='{url}'>Login</a><br><br>
                {IfLoginLinkDoesNotWorkSentence}                
                {GetFooter()}
                </div>";
            return mailContent;
        }

        public static string GetExportEmailContent(UserCookieData userCookieData, IEnumerable<string> fileNames)
        {
            string pluralSuffix = fileNames.Count() > 1 ? "s" : "";
            string verb = fileNames.Count() > 1 ? "are" : "is";
            string mailContent = $@"<div>
                Dear {userCookieData.GetFirstAndLastName()},<br><br> 
                We hereby inform you that your export file{pluralSuffix} <b>{string.Join(", ", fileNames)}</b> {verb} ready for download. <br><br>
                Kindly locate the requested file{pluralSuffix} attached to this message.
                {GetFooter()}
                </div>";

            return mailContent;
        }

        public static string GetTaggedInCommentNotificationEmailContent(Form form, int commentId, Personnel taggedUser, Personnel commentAuthor)
        {
            string url = $@"{GetDomain()}/Form/Edit?thesaurusId={form.ThesaurusId}&versionId={form.Version.Id}&activeTab=comments&taggedCommentId={commentId}";
            string mailContent = $@"<div>
                Dear {taggedUser.GetFirstAndLastName()},<br><br> 
                We would like to inform you that the user {commentAuthor.GetFirstAndLastName()} ({commentAuthor.Username}) has mentioned you in a comment in the following document: <b>{form.Title} {form.Version.GetFullVersionString()}</b><br><br>
                For additional information, please refer to the following link: <a href=""{url}"">View comment</a> <br><br>
                {IfLoginLinkDoesNotWorkSentence}
                {GetFooter()}
                </div>";
            return mailContent;
        }

        public static string GetThesaurusMergeEmailContent(UserCookieData userCookieData, DateTime startTime, DateTime endTime, bool noErrors = true)
        {
            int actionDurationInMinutes = (int)Math.Ceiling((endTime - startTime).TotalMinutes);
            string outcome = noErrors ? "Completed with no errors" : "Completed with errors. (Contact your administrator)";
            string mailContent = $@"<div>
                Dear {userCookieData.GetFirstAndLastName()},<br><br>
                The thesaurus merge operation you initiated has been completed. Here are the results:<br><br>
                Operation started: <b>{startTime}</b><br>
                Operation ended: <b>{endTime}</b><br>
                Duration time (aprox.): <b>{actionDurationInMinutes} minutes</b><br>
                Outcome: <b>{outcome}</b>
                {GetFooter()}
                </div>";
            return mailContent;
        }

        public static string GetConsensusCreatorEmailContent(Consensus consensus, Form form, IterationState iterationState, Personnel creator, List<Personnel> users, List<OutsideUser> outsideUsers)
        {
            string url = $"{GetDomain()}/Form/Edit?thesaurusId={form.ThesaurusId}&versionId={form.Version.Id}&activeTab={"consensus"}";

            string mailTableContent = GetConsensusTableContent(consensus, iterationState, creator, users, outsideUsers);
            string mailContent = $@"<div>
                Dear {creator.GetFirstAndLastName()},<br><br>
                We would like to inform you that the consensus finding process you initiated for <b>{form.Title} {form.Version.GetFullVersionString()}</b> is {GetStateConclusionDisplay(iterationState)}.<br><br>
                {mailTableContent}<br><br>
                For additional information, please refer to the following link: <a href=""{url}"">link</a><br><br>
                {IfLoginLinkDoesNotWorkSentence}
                {GetFooter()}
                </div>";
            return mailContent;
        }

        public static string GetConsensusParticipantEmailContent(ConsensusUserMailDTO consensusUserMailDTO)
        {
            string url = GetConsensusInstanceUserUrl(consensusUserMailDTO);
            string customMessage = !string.IsNullOrEmpty(consensusUserMailDTO.CustomMessage) ? 
                $@"{consensusUserMailDTO.CustomMessage}.<br><br>" : string.Empty;
            string mailContent = $@"<div>
                Dear {consensusUserMailDTO.User.FirstName} {consensusUserMailDTO.User.LastName},<br><br>
                We would like to inform you that you have been included in a consensus finding process  in the following document: <b>{consensusUserMailDTO.FullFormNameWithVersion}</b><br><br>
                {customMessage}
                For additional information, please refer to the following link: <a href=""{url}"">link</a><br><br>
                {IfLoginLinkDoesNotWorkSentence}
                {GetFooter()}
            </div>";
            return mailContent;
        }

        public static string GetConsensusParticipantReminderEmailContent(ConsensusUserMailDTO consensusUserMailDTO)
        {
            string url = GetConsensusInstanceUserUrl(consensusUserMailDTO);
            string mailContent = $@"<div>
                Dear {consensusUserMailDTO.User.FirstName} {consensusUserMailDTO.User.LastName},<br><br>
                This is a reminder concerning the {EmailSenderNames.SoftwareName} questionnaire you have been invited to participate in.<br><br>
                To complete your {EmailSenderNames.SoftwareName} questionnaire, please use the following link: <a href=""{url}"">link</a><br><br>
                {IfLoginLinkDoesNotWorkSentence}
                {GetFooter()}
            </div>";
            return mailContent;
        }

        #region Global Thesaurus

        public static string GetGlobalThesaurusContactFormEmailContent(ContactFormDataIn contactFormData)
        {
            string mailContent = $@"<div>
                Dear {contactFormData.FullName},<br><br>We recieved your contact form. Here is the content of your inquiry<br><br>
                <table border=""1"" style=""width: 25%;"">
                    <tr><th>Full Name</th><td>{contactFormData.FullName}</td></tr>
                    <tr><th>Email</th><td>{contactFormData.Email}</td></tr>
                    <tr><th>Role</th><td>{contactFormData.Role}</td></tr>
                    <tr><th>Organization</th><td>{contactFormData.Organization}</td></tr>
                    <tr><th>Adress</th><td>{contactFormData.Address}</td></tr>
                    <tr><th>Phone</th><td>{contactFormData.Phone}</td></tr>
                    <tr><th>Message</th><td>{contactFormData.Message}</td></tr>
                </table><br><br>
                ------------------------------------------------------------------------------
                </div>";
            return mailContent;
        }

        public static string GetGlobalThesaurusActivationLinkEmailContent(GlobalThesaurusUser user)
        {
            string mailContent = $@"<div>
                    Dear {user.FirstName} {user.LastName},<br><br>you are granted access to the Smart Oncology clinical information system.<br>
                    Use the following link to activate your account in the system: <a href='{GetDomain()}/ThesaurusGlobal/ActivateUser?email={user.Email}'>Activation link</a><br>
                    Your username is your email: <b>{user.Email}</b><br>
                    Your password is: <b>{user.Password}</b><br><br>
                    If you need any help or you have any additional questions please write on the <a href='mailto:{EmailSenderNames.SoftwareEmail}'>{EmailSenderNames.SoftwareEmail}</a>.<br><br>
                    ------------------------------------------------------------------------------
                    </div>";
            return mailContent;
        }

        #endregion /Global Thesaurus

        #region Private methods

        private static string GetDomain()
        {
            return HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
        }

        private static string GetFooter()
        {
            return $@"<br><br>
                Please do not reply to this message. This is an automatically generated message, and replies will not be read.<br>
                In case of any problems or further questions regarding this e-mail please contact our support at <a href='mailto:{EmailSenderNames.SoftwareEmail}'>{EmailSenderNames.SoftwareEmail}</a>.<br><br>
                Your Wemedoo Team.<br><br>
                <hr><br>
                This e-mail is for authorized use by the intended recipient(s) only. It may contain proprietary material, confidential information and/or be subject to legal privilege. Any unauthorised copying, disclosure, or distribution of the material in this e-mail is strictly forbidden. If an agreement including clauses on non-disclosure and/or confidential information exchange is in force between the recipient and Wemedoo AG, this email and its attachments are hereby qualified as confidential and subject to the agreement. If you received this message in error, please notify the sender immediately and delete it from your system.";
        }

        private static string GetStateConclusionDisplay(IterationState iterationState)
        {
            return iterationState == IterationState.InProgress ? "currently in progress" : iterationState.ToString().ToLower();
        }

        private static string GetStateTableDisplay(IterationState iterationState)
        {
            return iterationState == IterationState.InProgress ? "In progress" : iterationState.ToString();
        }

        private static string GetConsensusTableContent(Consensus consensus, IterationState iterationState, Personnel creator, List<Personnel> users, List<OutsideUser> outsideUsers)
        {
            ConsensusIteration iteration = consensus.Iterations.Last();
            int iterationIndex = consensus.Iterations.IndexOf(iteration);

            string mailTableContent = $@"<table border=""1"" style=""width: 50%;"">
                    <tr>
                        <th>Consensus created on</th>
                        <td>{consensus.EntryDatetime.ToTimeZoned(creator.PersonnelConfig.TimeZoneOffset, DateConstants.DateFormat)}</td>
                    </tr>
                    <tr>
                        <th>Iteration order number</th>
                        <td>{iterationIndex + 1}</td>
                    </tr>
                    <tr>
                        <th>Iteration started on</th>
                        <td>{iteration.EntryDatetime.ToTimeZoned(creator.PersonnelConfig.TimeZoneOffset, DateConstants.DateFormat)}</td>
                    </tr>
                    <tr>
                        <th>Iteration users</th>
                        <td>{string.Join(", ", users.Select(x => x.GetFirstAndLastName()))}</td>
                    </tr>
                    <tr>
                        <th>Iteration outside users</th><td>{string.Join(", ", outsideUsers.Select(x => string.Concat(x.FirstName, " ", x.LastName)))}</td>
                    </tr>
                    <tr>
                        <th>Iteration status</th><td>{GetStateTableDisplay(iterationState)}</td>
                    </tr>
                </table>";

            return mailTableContent;
        }

        private static string GetConsensusInstanceUserUrl(ConsensusUserMailDTO consensusUserMailDTO)
        {
            string actionName = consensusUserMailDTO.IsOutsideUser ? "CreateConsensusInstanceExternal" : "CreateConsensusInstance";
            return $"{GetDomain()}/FormConsensus/{actionName}?userId={consensusUserMailDTO.User.Id}&consensusId={consensusUserMailDTO.ConsensusId}&iterationId={consensusUserMailDTO.IterationId}";
        }

        #endregion /Private methods
    }
}
