using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.Common.DTO
{
    public class EmailDTO
    {
        public string EmailAddress { get; set; }
        public string Body { get; set; }
        public string Subject { get; set; }
        public Dictionary<string, Stream> Attachments { get; set; }
        public bool IsCsv { get; set; }

        public EmailDTO(string emailAddress, string body, string subject)
        {
            EmailAddress = emailAddress;
            Body = body;
            Subject = subject;
        }
    }
}
