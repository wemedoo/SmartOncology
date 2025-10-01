using sReportsV2.Common.Helpers;
using System;
using System.ComponentModel;
using System.Net.Mail;
using System.Net;
using sReportsV2.BusinessLayer.Components.Interfaces;
using sReportsV2.DTOs.Common.DTO;
using Microsoft.Extensions.Configuration;

namespace sReportsV2.BusinessLayer.Components.Implementations
{
    public class SmtpEmailSender : EmailSenderBase
    {
        private MailMessage _message;
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

            _message = new MailMessage
            {
                From = new MailAddress(smtpServerEmail, smtpServerEmailDisplayName),
                Subject = messageDto.Subject,
                IsBodyHtml = true,
                Body = messageDto.Body
            };
            _message.To.Add(new MailAddress(messageDto.EmailAddress));

            AddAttachments(messageDto);

            try
            {
                await smtpClient.SendMailAsync(_message);
            }
            catch (Exception e)
            {
                LogHelper.Error($"Sending email ended up with error: ({e.GetExceptionStackMessages()})");
            }
            finally
            {
                _message.Dispose();
                _message = null;
                DeleteFile(messageDto.Attachments, outputDirectory);
            }
        }

        protected override void AddAttachmentsToMail(string outputPath, string zipFileName)
        {
            Attachment attachment = new Attachment(outputPath);
            attachment.Name = zipFileName;
            _message.Attachments.Add(attachment);
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
