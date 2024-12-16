using sReportsV2.DTOs.EpisodeOfCare;
using sReportsV2.DTOs.Pagination;
using sReportsV2.DTOs.User.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sReportsV2.BusinessLayer.Interfaces
{
    public interface IEpisodeOfCareBLL
    {
        Task<PaginationDataOut<EpisodeOfCareDataOut, EpisodeOfCareFilterDataIn>> GetAllFilteredAsync(EpisodeOfCareFilterDataIn dataIn, UserCookieData userCookieData);
        Task<int> InsertOrUpdateAsync(EpisodeOfCareDataIn episodeOfCareDataIn, UserCookieData userCookieData);
        Task<EpisodeOfCareDataOut> GetByIdAsync(int episodeOfCareId, string language);
        Task<List<EpisodeOfCareDataOut>> GetByPatientIdAsync(EpisodeOfCareDataIn episodeOfCare, string language);
        Task DeleteAsync(int eocId);
    }
}
