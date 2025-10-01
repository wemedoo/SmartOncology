using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using sReportsV2.DTOs.Autocomplete;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.DTOs.Autocomplete.DataOut;
using sReportsV2.DTOs.DTOs.PatientQuery.DataIn;
using sReportsV2.DTOs.DTOs.PatientQuery.DataOut;
using sReportsV2.DTOs.DTOs.ThesaurusEntry.DataIn;
using sReportsV2.DTOs.ThesaurusEntry.DataOut;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sReportsV2.BusinessLayer.Components.Interfaces
{
    public interface ISkosConnector
    {
        bool UseSkosData();
        Task<List<AutocompleteDataOut>> GetConceptSchemes();
        Task<List<SkosLabelAutocompleteDataOut>> SearchByPrefAndAltTerm(string searchTerm);
        Task GetSkosData(ThesaurusEntry thesaurusEntry, ThesaurusEntryDataOut viewModel);
        Task InsertConcept(ThesaurusEntry thesaurusEntry, SkosDataIn skosData);
        Task DeleteConcept(int thesaurusEntryId);
        Task InsertConcepts(Tuple<Dictionary<string, ThesaurusEntry>, List<ParentChildDTO>> results);
        Task<PatientQueryResultDataOut> GetNarrowerConcepts(PatientQueryFilterDataIn patientQueryFilterDataIn);
        Task<string> ExportSkos(int thesaurusEntryId);
    }
}
