using AutoMapper;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Sql.Entities.ClinicalTrial;
using sReportsV2.DTOs.DTOs.TrialManagement;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sReportsV2.DTOs.Autocomplete;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Common.Constants;

namespace sReportsV2.BusinessLayer.Implementations
{
    public class TrialManagementBLL : ITrialManagementBLL
    {
        private readonly ITrialManagementDAL trialManagementDAL;
        private readonly IMapper mapper;

        public TrialManagementBLL(ITrialManagementDAL trialManagementDAL, IMapper mapper)
        {
            this.trialManagementDAL = trialManagementDAL;
            this.mapper = mapper;
        }

        public async Task<ClinicalTrialDataOut> InsertOrUpdate(ClinicalTrialDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            ClinicalTrial trial = mapper.Map<ClinicalTrial>(dataIn);
            return mapper.Map<ClinicalTrialDataOut>(await trialManagementDAL.InsertOrUpdate(trial));
        }

        public async Task<int> Archive(int id)
        {
            return await trialManagementDAL.Archive(id).ConfigureAwait(false);
        }

        public async Task<AutocompleteResultDataOut> GetTrialAutoCompleteTitle(AutocompleteDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            TrialManagementFilter filter = new TrialManagementFilter() { ClinicalTrialTitle = dataIn.Term, Page = dataIn.Page, PageSize = FilterConstants.DefaultPageSize };
            PaginationData<AutoCompleteData> trialsAndCount = await trialManagementDAL.GetTrialAutoCompleteTitleAndCount(filter);

            List<AutocompleteDataOut> autocompleteDataDataOuts = trialsAndCount.Data
                .Select(x => new AutocompleteDataOut()
                {
                    id = x.Id,
                    text = x.Text,
                })
                .ToList();
            AutocompleteResultDataOut result = new AutocompleteResultDataOut()
            {
                results = autocompleteDataDataOuts,
                pagination = new AutocompletePaginatioDataOut() { more = dataIn.ShouldLoadMore(trialsAndCount.Count) }
            };

            return result;
        }

        public List<ClinicalTrialDataOut> GetlClinicalTrialsByName(string name)
        {
            return mapper.Map<List<ClinicalTrialDataOut>>(trialManagementDAL.GetlClinicalTrialsByName(name));
        }

    }
}
