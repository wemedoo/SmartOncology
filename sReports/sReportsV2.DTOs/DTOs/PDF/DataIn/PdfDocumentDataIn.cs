using sReportsV2.DTOs.User.DTO;
using System.Collections.Generic;

namespace sReportsV2.DTOs.DTOs.PDF.DataIn
{
    public class PdfDocumentDataIn
    {
        public UserCookieData UserCookieData { get; set; }
        public string ResourceId { get; set; }
    }
}
