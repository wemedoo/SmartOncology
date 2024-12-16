using sReportsV2.Common.Helpers;
using System;
using System.ComponentModel;
using System.Net.Mail;
using System.Net;
using System.Web.Configuration;
using sReportsV2.BusinessLayer.Components.Interfaces;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.Common.Extensions;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace sReportsV2.BusinessLayer.Components.Implementations
{
    public class SmtpEmailSender : EmailSenderBase
    {
        public SmtpEmailSender(IConfiguration configuration) : base(configuration)
        {
        }

        public override async void SendAsync(EmailDTO messageDto)
        {
            string smtpServerEmail = configuration["SmtpServerEmail"];
            string smtpServerPassword = configuration["SmtpServerPassword"];
            string smtpServerEmailDisplayName = configuration["SmtpServerEmailDisplayName"];
            string smtpServerHost = configuration["SmtpServerHost"];
            int.TryParse(configuration["SmtpServerPort"], out int smtpServerPort);
            smtpServerPort = smtpServerPort > 0 ? smtpServerPort : 22;
            bool.TryParse(configuration["SmtpServerEnableSsl"], out bool enableSsl);

            SmtpClient smtpClient = new SmtpClient
            {
                Port = smtpServerPort,
                Host = smtpServerHost,
                EnableSsl = enableSsl,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(smtpServerEmail, smtpServerPassword),
                DeliveryMethod = SmtpDeliveryMethod.Network
            };
            smtpClient.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);

            MailMessage message = new MailMessage
            {
                From = new MailAddress(smtpServerEmail, smtpServerEmailDisplayName),
                Subject = messageDto.Subject,
                IsBodyHtml = true,
                Body = messageDto.Body
            };
            message.To.Add(new MailAddress(messageDto.EmailAddress));

            if (messageDto.Attachments != null)
            {
                foreach (var file in messageDto.Attachments)
                {
                    string extension = ".zip";
                    string sanitizedFileName = file.Key.SanitizeFileName();
                    string outputPath = outputDirectory.CombineFilePath(sanitizedFileName);
                    CreateFileZip(file.Value, sanitizedFileName, outputPath, messageDto.IsCsv);
                    Attachment attachment = new Attachment(outputPath);
                    attachment.Name = sanitizedFileName + extension;
                    message.Attachments.Add(attachment);

                    file.Value.Dispose();
                }
            }

            try
            {
                await smtpClient.SendMailAsync(message);
            }
            catch (Exception e)
            {
                LogHelper.Error($"Sending email ended up with error: ({e.Message})");
            }
            finally
            {
                message.Dispose();
                DeleteFile(messageDto.Attachments, outputDirectory);
            }
        }

        private void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                LogHelper.Error("Send canceled.");
            }
            if (e.Error != null)
            {
                LogHelper.Error($"Sending email ended up with error: ({e.Error})");
            }
        }
    }
}
