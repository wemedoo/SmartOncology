using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.DigitalGuideline.DataIn;
using sReportsV2.DTOs.DigitalGuideline.DataOut;
using sReportsV2.DTOs.DTOs.Autocomplete.DataOut;
using sReportsV2.DTOs.Pagination;
using sReportsV2.DTOs.User.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sReportsV2.BusinessLayer.Interfaces
{
    public interface IDigitalGuidelineBLL
    {
        PaginationDataOut<GuidelineDataOut, GuidelineFilterDataIn> GetAll(GuidelineFilterDataIn dataIn);
        PaginationDataOut<GuidelineDataOut, GuidelineFilterDataIn> GetVersionHistory(GuidelineFilterDataIn dataIn);
        Task<GuidelineDataOut> GetById(string id);
        Task<ResourceCreatedDTO> InsertOrUpdate(GuidelineDataIn dataIn, int userId);
        GuidelineElementDataDataOut PreviewNode(GuidelineElementDataDataIn dataIn);
        void Delete(string id, DateTime lastUpdate);
        List<AutocompleteOptionDataOut> SearchByTitle(string title, UserCookieData userCookieData);
    }
}
