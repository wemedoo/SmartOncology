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

namespace sReportsV2.BusinessLayer.Implementations
{
    public class TrialManagementBLL : ITrialManagementBLL
    {
        private readonly ITrialManagementDAL trialManagementDAL;
        private readonly IMapper Mapper;

        public TrialManagementBLL(ITrialManagementDAL trialManagementDAL, IMapper mapper)
        {
            this.trialManagementDAL = trialManagementDAL;
            Mapper = mapper;
        }

        public async Task<ClinicalTrialDataOut> InsertOrUpdate(ClinicalTrialDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            ClinicalTrial trial = Mapper.Map<ClinicalTrial>(dataIn);
            return Mapper.Map<ClinicalTrialDataOut>(await trialManagementDAL.InsertOrUpdate(trial));
        }

        public async Task<int> Archive(int id)
        {
            return await trialManagementDAL.Archive(id).ConfigureAwait(false);
        }

        public async Task<AutocompleteResultDataOut> GetTrialAutoCompleteTitle(AutocompleteDataIn dataIn)
        {
            int pageSize = 10;
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            TrialManagementFilter filter = new TrialManagementFilter() { ClinicalTrialTitle = dataIn.Term, Page = dataIn.Page, PageSize = pageSize };
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
                pagination = new AutocompletePaginatioDataOut() { more = trialsAndCount.Count > dataIn.Page * pageSize, }
            };

            return result;
        }

        public List<ClinicalTrialDataOut> GetlClinicalTrialsByName(string name)
        {
            return Mapper.Map<List<ClinicalTrialDataOut>>(trialManagementDAL.GetlClinicalTrialsByName(name));
        }

    }
}
