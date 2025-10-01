using AutoMapper;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Domain.Entities.DigitalGuideline;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.DigitalGuideline.DataIn;
using sReportsV2.DTOs.DigitalGuideline.DataOut;
using sReportsV2.DTOs.DTOs.Autocomplete.DataOut;
using sReportsV2.DTOs.Pagination;
using sReportsV2.DTOs.ThesaurusEntry.DataOut;
using sReportsV2.DTOs.User.DTO;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sReportsV2.BusinessLayer.Implementations
{
    public class DigitalGuidelineBLL : IDigitalGuidelineBLL
    {
        private readonly IDigitalGuidelineDAL digitalGuidelineDAL;
        private readonly IThesaurusDAL thesaurusDAL;
        private readonly IMapper mapper;

        public DigitalGuidelineBLL(IDigitalGuidelineDAL digitalGuidelineDAL, IThesaurusDAL thesaurusDAL, IMapper mapper)
        {
            this.digitalGuidelineDAL = digitalGuidelineDAL;
            this.thesaurusDAL = thesaurusDAL;
            this.mapper = mapper;
        }

        public void Delete(string id, DateTime lastUpdate)
        {
            digitalGuidelineDAL.Delete(id, lastUpdate);
        }

        public async Task<GuidelineDataOut> GetById(string id)
        {
            var data = await this.digitalGuidelineDAL.GetByIdAsync(id).ConfigureAwait(false);
            GuidelineDataOut dataOut = mapper.Map<GuidelineDataOut>(data);

            return dataOut;
        }

        public PaginationDataOut<GuidelineDataOut, GuidelineFilterDataIn> GetAll(GuidelineFilterDataIn dataIn)
        {
            GuidelineFilter filter = mapper.Map<GuidelineFilter>(dataIn);

            PaginationDataOut<GuidelineDataOut, GuidelineFilterDataIn> result = new PaginationDataOut<GuidelineDataOut, GuidelineFilterDataIn>()
            {
                Count = this.digitalGuidelineDAL.GetAllCount(filter),
                Data = mapper.Map<List<GuidelineDataOut>>(this.digitalGuidelineDAL.GetAll(filter)),
                DataIn = dataIn
            };

            return result;
        }

        public PaginationDataOut<GuidelineDataOut, GuidelineFilterDataIn> GetVersionHistory(GuidelineFilterDataIn dataIn)
        {
            GuidelineFilter filter = mapper.Map<GuidelineFilter>(dataIn);

            PaginationDataOut<GuidelineDataOut, GuidelineFilterDataIn> result = new PaginationDataOut<GuidelineDataOut, GuidelineFilterDataIn>()
            {
                Count = this.digitalGuidelineDAL.GetAllCount(filter),
                Data = mapper.Map<List<GuidelineDataOut>>(this.digitalGuidelineDAL.GetAllByThesaurus(filter)),
                DataIn = dataIn
            };

            return result;
        }

        public GuidelineElementDataDataOut PreviewNode(GuidelineElementDataDataIn dataIn)
        {
            GuidelineElementDataDataOut data = mapper.Map<GuidelineElementDataDataOut>(dataIn);
            data.Thesaurus = mapper.Map<ThesaurusEntryDataOut>(this.thesaurusDAL.GetById(dataIn.ThesaurusId));

            return data;
        }

        public async Task<ResourceCreatedDTO> InsertOrUpdate(GuidelineDataIn dataIn, int userId)
        {
            Guideline guideline = await this.digitalGuidelineDAL.InsertOrUpdateAsync(mapper.Map<Guideline>(dataIn), userId).ConfigureAwait(false);

            return new ResourceCreatedDTO()
            {
                Id = guideline?.Id,
                LastUpdate = guideline.LastUpdate.Value.ToString("o")
            };
        }

        public List<AutocompleteOptionDataOut> SearchByTitle(string title, UserCookieData userCookieData)
        {
            return digitalGuidelineDAL.SearchByTitle(title).Select(x => new AutocompleteOptionDataOut(x, userCookieData)).ToList();
        }
    }
}
