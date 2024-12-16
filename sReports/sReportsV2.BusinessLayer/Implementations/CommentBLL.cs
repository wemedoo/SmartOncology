using AutoMapper;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.DTOs.Common.DataOut;
using sReportsV2.Domain.Sql.Entities.FormComment;
using sReportsV2.DTOs.Form.DataOut;
using sReportsV2.SqlDomain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sReportsV2.DTOs.Form.DataIn;
using sReportsV2.Common.Enums;
using sReportsV2.Domain.Sql.Entities.User;
using sReportsV2.Common.Constants;
using sReportsV2.BusinessLayer.Components.Interfaces;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.BusinessLayer.Helpers;

namespace sReportsV2.BusinessLayer.Implementations
{

    public class CommentBLL : ICommentBLL
    {
        private readonly ICommentDAL commentDAL;
        private readonly IPersonnelDAL userDAL;
        private readonly IFormDAL formDAL;
        private readonly IEmailSender emailSender;
        private readonly IMapper Mapper;

        public CommentBLL(ICommentDAL commentDAL, IPersonnelDAL userDAL, IFormDAL formDAL, IEmailSender emailSender, IMapper mapper)
        {
            this.commentDAL = commentDAL;
            this.userDAL = userDAL;
            this.formDAL = formDAL;
            this.emailSender = emailSender;
            Mapper = mapper;
        }

        public List<FormCommentDataOut> GetComentsDataOut(string formId, List<string> formItemsOrderIds)
        {
            List<Comment> comments = commentDAL.FindCommentsByFormId(formId);

            List<FormCommentDataOut> commentsDataOut = Mapper.Map<List<FormCommentDataOut>>(comments);
            List<int> userIds = comments.Select(x => x.PersonnelId).Distinct().ToList();
            List<UserDataOut> users = Mapper.Map<List<UserDataOut>>(userDAL.GetAllByIds(userIds));
            foreach (var comment in commentsDataOut)
            {
                comment.User = users.Find(x => x.Id == comment.UserId);
            }
            
            commentsDataOut = commentsDataOut.OrderBy(x => formItemsOrderIds.IndexOf(x.ItemRef)).ToList();

            return commentsDataOut;
        }

        public void InsertOrUpdate(FormCommentDataIn commentDataIn)
        {
            Comment comment = Mapper.Map<Comment>(commentDataIn);
            commentDAL.InsertOrUpdate(comment);
            NotifyTaggedUserInComments(commentDataIn.TaggedUsers, commentDataIn.FormRef, commentDataIn.UserId, comment.CommentId);
        }

        public string UpdateState(int commentId, int? stateCD)
        {
            var comment = commentDAL.FindById(commentId);
            commentDAL.UpdateState(commentId, stateCD);

            return comment.FormRef;
        }

        private void NotifyTaggedUserInComments(List<int> taggedUsers, string formRef, int commentAuthorId, int commentId)
        {
            if (taggedUsers != null && taggedUsers.Any())
            {
                Form form = formDAL.GetForm(formRef);

                Personnel commentAuthor = userDAL.GetById(commentAuthorId);
                string title = GetTaggedUserEmailTitle(commentAuthor);

                foreach (Personnel taggedUser in userDAL.GetAllByIds(taggedUsers))
                {
                    string mailContent = EmailHelpers.GetTaggedInCommentNotificationEmailContent(form, commentId, taggedUser, commentAuthor);
                    Task.Run(() => emailSender.SendAsync(new EmailDTO(taggedUser.Email, mailContent, title)));
                }
            }
        }

        private string GetTaggedUserEmailTitle(Personnel commentAuthor)
        {
            string title = $"User({commentAuthor.GetFirstAndLastName()}) mentioned you in a comment on {EmailSenderNames.SoftwareName}";
            return title;
        }
    }
}
