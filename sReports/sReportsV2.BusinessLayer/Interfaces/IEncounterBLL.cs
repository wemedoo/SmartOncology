using sReportsV2.Domain.Entities.Form;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.DTOs.Encounter.DataOut;
using sReportsV2.DTOs.Encounter;
using sReportsV2.DTOs.Encounter.DataOut;
using sReportsV2.DTOs.Form.DataOut;
using sReportsV2.DTOs.Pagination;
using sReportsV2.DTOs.User.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sReportsV2.BusinessLayer.Interfaces
{
    public interface IEncounterBLL
    {
        Task<int> InsertOrUpdateAsync(EncounterDataIn encounterData);
        int InsertOrUpdate(EncounterDataIn encounterData);
        Task<EncounterDetailsPatientTreeDataOut> ListReferralsAndForms(int encounterId, int episodeOfCareId, UserCookieData userCookieData);
        Task<List<FormDataOut>> ListForms(string condition, UserCookieData userCookieData);
        List<EncounterDataOut> GetAllByEocId(int episodeOfCareId);
        Task<EncounterDetailsPatientTreeDataOut> GetEncounterAndFormInstancesAndSuggestedForms(int encounterId, UserCookieData userCookieData);
        Task DeleteAsync(int id);
        Task<List<Form>> GetSuggestedForms(List<string> suggestedFormsIds);
        Task<EncounterDataOut> GetByEncounterIdAsync(int encounterId, int organizationid, bool onlyEncounter);
        Task<List<EncounterDataOut>> GetEncountersByTypeAndEocIdAsync(int encounterTypeId, int episodeOfCareId);
        int GetEncounterTypeByEncounterId(int encounterId);
        Task<PaginationDataOut<EncounterViewDataOut, DataIn>> GetAllFiltered(EncounterFilterDataIn dataIn);
    }
}
