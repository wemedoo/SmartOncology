using SendGrid.Helpers.Mail;
using SendGrid;
using sReportsV2.Common.Constants;
using System.Web.Configuration;
using System.IO;
using System;
using System.Collections.Generic;
using sReportsV2.BusinessLayer.Components.Interfaces;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.Common.Extensions;
using Microsoft.Extensions.Configuration;

namespace sReportsV2.BusinessLayer.Components.Implementations
{
    public class SendGridEmailSender : EmailSenderBase
    {
        private SendGridMessage _message;
        public SendGridEmailSender(IConfiguration configuration) : base(configuration)
        {
        }

        public override async void SendAsync(EmailDTO messageDto)
        {
            var apiKey = configuration["AppEmailKey"];
            var email = configuration["AppEmail"];
            var sendGridClient = new SendGridClient(apiKey);
            var from = new EmailAddress(email, EmailSenderNames.SoftwareName);
            _message = MailHelper.CreateSingleEmail(from, new EmailAddress(messageDto.EmailAddress), messageDto.Subject, string.Empty, messageDto.Body);

            AddAttachments(messageDto);

            await sendGridClient.SendEmailAsync(_message).ConfigureAwait(false);
            _message = null;

            DeleteFile(messageDto.Attachments, outputDirectory);
        }

        protected override void AddAttachmentsToMail(string outputPath, string zipFileName)
        {
            using (var attachmentStream = new MemoryStream(System.IO.File.ReadAllBytes(outputPath)))
            {
                var attachment = new Attachment
                {
                    Content = Convert.ToBase64String(attachmentStream.ToArray()),
                    Filename = zipFileName
                };
                _message.Attachments = new List<Attachment> { attachment };
            }
        }
    }
}
