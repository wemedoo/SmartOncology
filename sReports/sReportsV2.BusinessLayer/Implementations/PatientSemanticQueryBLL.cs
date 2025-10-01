using sReportsV2.Common.Extensions;
using sReportsV2.DTOs.Autocomplete;
using sReportsV2.DTOs.DTOs.Autocomplete.DataOut;
using sReportsV2.DTOs.DTOs.PatientQuery.DataIn;
using sReportsV2.DTOs.DTOs.PatientQuery.DataOut;
using sReportsV2.DTOs.FormInstance;
using sReportsV2.DTOs.FormInstance.DataOut;
using sReportsV2.DTOs.Pagination;
using sReportsV2.DTOs.User.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sReportsV2.BusinessLayer.Implementations
{
    public partial class FormInstanceBLL
    {
        public async Task<PatientSemanticQueryResultDataOut> GetPatientSemanticResult(PatientQueryFilterDataIn patientQueryFilterDataIn, UserCookieData userCookieData)
        {
            PatientSemanticQueryResultDataOut viewModel = new PatientSemanticQueryResultDataOut();
            if (!patientQueryFilterDataIn.InitialLoad)
            {
                viewModel.PatientQueryResultData = await skosConnector.GetNarrowerConcepts(patientQueryFilterDataIn);
                FormInstanceFilterDataIn filterDataIn = new FormInstanceFilterDataIn();
                filterDataIn.DoPatientSemanticQuery = true;
                filterDataIn.ThesaurusIds = viewModel.PatientQueryResultData.MatchingFieldInstanceThesaurusIds;
                var result = await ReloadData(filterDataIn, userCookieData);
                viewModel.FormInstanceData = new PaginationDataOut<FormInstanceTableDataOut, PatientQueryFilterDataIn>
                {
                    Count = result.Count,
                    Data = result.Data,
                    DataIn = patientQueryFilterDataIn
                };
            }
            
            return viewModel;
        }

        public async Task<AutocompleteResultDataOut<SkosLabelAutocompleteDataOut>> GetDataForAutocomplete(AutocompleteDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            List<SkosLabelAutocompleteDataOut> filtered = await skosConnector.SearchByPrefAndAltTerm(dataIn.Term);
            List<SkosLabelAutocompleteDataOut> skosLabels = filtered.OrderBy(x => x.text)
                //.Skip(dataIn.Page * 15)
                //.Take(15)
                .Where(x => string.IsNullOrEmpty(dataIn.ExcludeId) || !x.id.Equals(dataIn.ExcludeId))
                .ToList();

            AutocompleteResultDataOut<SkosLabelAutocompleteDataOut> result = new AutocompleteResultDataOut<SkosLabelAutocompleteDataOut>()
            {
                pagination = new AutocompletePaginatioDataOut()
                {
                    more = dataIn.ShouldLoadMore(filtered.Count())
                },
                results = skosLabels
            };

            return result;
        }
    }
}
