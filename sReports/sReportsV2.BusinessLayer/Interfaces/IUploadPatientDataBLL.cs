using Microsoft.AspNetCore.Http;
using sReportsV2.DTOs.Pagination;
using System.Threading.Tasks;
using sReportsV2.DTOs.DTOs.UploadPatientData.DataOut;
using sReportsV2.DTOs.DTOs.UploadPatientData.DataIn;
using System.Collections.Generic;

namespace sReportsV2.BusinessLayer.Interfaces
{
    public interface IUploadPatientDataBLL
    {
        Task UploadPatientData(IFormFileCollection files, string domain);
        Task<PaginationDataOut<UploadPatientDataOut, UploadPatientFilterDataIn>> ReloadData(UploadPatientFilterDataIn dataIn);
        Task<List<PromptResultDataOut>> ProceedLLM(int uploadPatientDataId);
    }
}
