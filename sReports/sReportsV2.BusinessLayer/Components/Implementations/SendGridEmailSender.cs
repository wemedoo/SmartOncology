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
        public SendGridEmailSender(IConfiguration configuration) : base(configuration)
        {
        }

        public override async void SendAsync(EmailDTO messageDto)
        {
            var apiKey = configuration["AppEmailKey"];
            var email = configuration["AppEmail"];
            var sendGridClient = new SendGridClient(apiKey);
            var from = new EmailAddress(email, EmailSenderNames.SoftwareName);
            var msg = MailHelper.CreateSingleEmail(from, new EmailAddress(messageDto.EmailAddress), messageDto.Subject, string.Empty, messageDto.Body);

            if (messageDto.Attachments != null)
            {
                foreach (KeyValuePair<string, Stream> file in messageDto.Attachments)
                {
                    string extension = ".zip";
                    string sanitizedFileName = file.Key.SanitizeFileName();
                    string outputPath = outputDirectory.CombineFilePath(sanitizedFileName);
                    CreateFileZip(file.Value, sanitizedFileName, outputPath, messageDto.IsCsv);
                    using (var attachmentStream = new MemoryStream(System.IO.File.ReadAllBytes(outputPath)))
                    {
                        var attachment = new Attachment
                        {
                            Content = Convert.ToBase64String(attachmentStream.ToArray()),
                            Filename = sanitizedFileName + extension
                        };
                        msg.Attachments = new List<Attachment> { attachment };
                    }

                    file.Value.Dispose();
                }
            }

            await sendGridClient.SendEmailAsync(msg).ConfigureAwait(false);

            DeleteFile(messageDto.Attachments, outputDirectory);
        }
    }
}
