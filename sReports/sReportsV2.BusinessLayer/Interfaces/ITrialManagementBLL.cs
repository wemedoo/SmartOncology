using sReportsV2.DTOs.DTOs.TrialManagement;
using System.Collections.Generic;
using System.Threading.Tasks;
using sReportsV2.DTOs.Autocomplete;

namespace sReportsV2.BusinessLayer.Interfaces
{
    public interface ITrialManagementBLL
    {
        Task<ClinicalTrialDataOut> InsertOrUpdate(ClinicalTrialDataIn dataIn);
        Task<int> Archive(int id);
        Task<AutocompleteResultDataOut> GetTrialAutoCompleteTitle(AutocompleteDataIn dataIn);
        List<ClinicalTrialDataOut> GetlClinicalTrialsByName(string name);
    }
}
