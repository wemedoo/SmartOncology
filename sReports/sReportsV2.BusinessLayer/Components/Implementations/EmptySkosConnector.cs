using sReportsV2.BusinessLayer.Components.Interfaces;
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

namespace sReportsV2.BusinessLayer.Components.Implementations
{
    public class EmptySkosConnector : ISkosConnector
    {
        public Task<PatientQueryResultDataOut> GetNarrowerConcepts(PatientQueryFilterDataIn patientQueryFilterDataIn)
        {
            return GetDefaultTask(new PatientQueryResultDataOut());
        }

        public Task<List<AutocompleteDataOut>> GetConceptSchemes()
        {
            return GetDefaultTask(new List<AutocompleteDataOut>());
        }

        public Task GetSkosData(ThesaurusEntry thesaurusEntry, ThesaurusEntryDataOut viewModel)
        {
            return GetDefaultTask();
        }

        public Task InsertConcept(ThesaurusEntry thesaurusEntry, SkosDataIn skosData)
        {
            return GetDefaultTask();
        }

        public Task InsertConcepts(Tuple<Dictionary<string, ThesaurusEntry>, List<ParentChildDTO>> results)
        {
            return GetDefaultTask();
        }

        public bool UseSkosData()
        {
            return false;
        }

        public Task<List<SkosLabelAutocompleteDataOut>> SearchByPrefAndAltTerm(string searchTerm)
        {
            return GetDefaultTask(new List<SkosLabelAutocompleteDataOut>());
        }

        private Task GetDefaultTask()
        {
            return Task.FromResult(0);
        }

        private Task<T> GetDefaultTask<T>(T returnObject)
        {
            return Task.FromResult(returnObject);
        }

        public Task DeleteConcept(int thesaurusEntryId)
        {
            return GetDefaultTask();
        }

        public Task<string> ExportSkos(int thesaurusEntryId)
        {
            return GetDefaultTask(new string(string.Empty));
        }
    }
}
