using Microsoft.AspNetCore.Http;
using sReportsV2.DTOs.DTOs.PDF.DataIn;
using sReportsV2.DTOs.DTOs.PDF.DataOut;
using sReportsV2.DTOs.User.DTO;
using System.Web;

namespace sReportsV2.BusinessLayer.Interfaces
{
    public interface IPdfBLL
    {
        PdfDocumentDataOut Generate(PdfDocumentDataIn documentDataIn);
        PdfDocumentDataOut GenerateSynoptic(PdfDocumentDataIn documentDataIn);
        void UploadFile(IFormFile file, UserCookieData userCookieData);
    }
}
