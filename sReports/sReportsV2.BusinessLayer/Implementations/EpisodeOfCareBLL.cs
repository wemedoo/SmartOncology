using AutoMapper;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Entities.User;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.Domain.Sql.Entities.EpisodeOfCare;
using sReportsV2.DTOs.EpisodeOfCare;
using sReportsV2.DTOs.Pagination;
using sReportsV2.DTOs.User.DTO;
using sReportsV2.SqlDomain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sReportsV2.BusinessLayer.Implementations
{
    public class EpisodeOfCareBLL : IEpisodeOfCareBLL
    {
        private readonly IPatientDAL patientDAL;
        private readonly IEpisodeOfCareDAL episodeOfCareDAL;
        private readonly IEncounterDAL encounterDAL;
        private readonly IFormInstanceDAL formInstanceDAL;
        private readonly IMapper Mapper;

        public EpisodeOfCareBLL(IEpisodeOfCareDAL episodeOfCareDAL, IPatientDAL patientDAL, IEncounterDAL encounterDAL, IFormInstanceDAL formInstanceDAL, IMapper mapper)
        {
            this.patientDAL = patientDAL;
            this.episodeOfCareDAL = episodeOfCareDAL;
            this.encounterDAL = encounterDAL;
            this.formInstanceDAL = formInstanceDAL;
            Mapper = mapper;
        }

        public async Task DeleteAsync(int eocId)
        {
            await episodeOfCareDAL.DeleteAsync(eocId);
            await formInstanceDAL.DeleteByEpisodeOfCareIdAsync(eocId);
        }

        public async Task<PaginationDataOut<EpisodeOfCareDataOut, EpisodeOfCareFilterDataIn>> GetAllFilteredAsync(EpisodeOfCareFilterDataIn dataIn, UserCookieData userCookieData)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            EpisodeOfCareFilter filter = GetFilterData(dataIn, userCookieData);

            var countTask = episodeOfCareDAL.GetAllEntriesCountAsync(filter);
            var dataTask = episodeOfCareDAL.GetAllAsync(filter);

            await Task.WhenAll(countTask, dataTask);

            PaginationDataOut<EpisodeOfCareDataOut, EpisodeOfCareFilterDataIn> result = new PaginationDataOut<EpisodeOfCareDataOut, EpisodeOfCareFilterDataIn>()
            {
                Count = (int)await countTask,
                Data = Mapper.Map<List<EpisodeOfCareDataOut>>(await dataTask),
                DataIn = dataIn
            };

            return result;
        }

        public async Task<EpisodeOfCareDataOut> GetByIdAsync(int episodeOfCareId, string language)
        {
            if (episodeOfCareId == 0) return null;
            var eoc = await episodeOfCareDAL.GetByIdAsync(episodeOfCareId);
            if (eoc == null) return null;

            var episodeOfCareDataOut = Mapper.Map<EpisodeOfCareDataOut>(eoc);
            episodeOfCareDataOut.NumOfDocuments = await formInstanceDAL.CountAllEOCDocumentsAsync(episodeOfCareDataOut.Id, episodeOfCareDataOut.PatientId);
            episodeOfCareDataOut.NumOfEncounters = episodeOfCareDataOut.Encounters.Count;

            return episodeOfCareDataOut;
        }

        public async Task<List<EpisodeOfCareDataOut>> GetByPatientIdAsync(EpisodeOfCareDataIn episodeOfCare, string language)
        {
            EpisodeOfCareFilter filter = Mapper.Map<EpisodeOfCareFilter>(episodeOfCare);
            List<EpisodeOfCare> episodeOfCareTask = await episodeOfCareDAL.GetByPatientIdFilteredAsync(filter);
            List<EpisodeOfCareDataOut> episodesOfCareDataOut = Mapper.Map<List<EpisodeOfCareDataOut>>(episodeOfCareTask);

            foreach (var eoc in episodesOfCareDataOut)
            {
                eoc.NumOfDocuments = await formInstanceDAL.CountAllEOCDocumentsAsync(eoc.Id, episodeOfCare.PatientId);
                eoc.NumOfEncounters = await encounterDAL.CountAllEncountersAsync(eoc.Id);
            }

            return episodesOfCareDataOut;
        }

        public async Task<int> InsertOrUpdateAsync(EpisodeOfCareDataIn episodeOfCareDataIn, UserCookieData userCookieData)
        {
            episodeOfCareDataIn = Ensure.IsNotNull(episodeOfCareDataIn, nameof(episodeOfCareDataIn));

            EpisodeOfCare episodeOfCare = Mapper.Map<EpisodeOfCare>(episodeOfCareDataIn);
            episodeOfCare.OrganizationId = userCookieData.ActiveOrganization;
            UserData userData = Mapper.Map<UserData>(userCookieData);

            return await episodeOfCareDAL.InsertOrUpdateAsync(episodeOfCare, userData).ConfigureAwait(false);
        }

        private EpisodeOfCareFilter GetFilterData(EpisodeOfCareFilterDataIn dataIn, UserCookieData userCookieData)
        {
            EpisodeOfCareFilter result = Mapper.Map<EpisodeOfCareFilter>(dataIn);
            if (dataIn.IdentifierType.HasValue && !string.IsNullOrEmpty(dataIn.IdentifierValue))
            {
                result.FilterByIdentifier = true;
                result.PatientId = this.patientDAL.GetByIdentifier(new Domain.Sql.Entities.Patient.PatientIdentifier(dataIn.IdentifierType, dataIn.IdentifierValue, null)) != null ? this.patientDAL.GetByIdentifier(new Domain.Sql.Entities.Patient.PatientIdentifier(dataIn.IdentifierType, dataIn.IdentifierValue, null)).PatientId : 0;
            }

            result.OrganizationId = userCookieData.ActiveOrganization;
            return result;
        }
    }
}
