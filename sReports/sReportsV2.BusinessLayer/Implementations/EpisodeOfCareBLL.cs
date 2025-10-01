using AutoMapper;
using sReportsV2.BusinessLayer.Components.Interfaces;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Entities.User;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.Domain.Sql.Entities.EpisodeOfCare;
using sReportsV2.DTOs.EpisodeOfCare;
using sReportsV2.DTOs.User.DTO;
using sReportsV2.SqlDomain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sReportsV2.BusinessLayer.Implementations
{
    public class EpisodeOfCareBLL : IEpisodeOfCareBLL
    {
        private readonly IEpisodeOfCareDAL episodeOfCareDAL;
        private readonly IEncounterDAL encounterDAL;
        private readonly IFormInstanceDAL formInstanceDAL;
        private readonly IMapper mapper;
        private readonly ISkosConnector skosConnector;

        public EpisodeOfCareBLL(IEpisodeOfCareDAL episodeOfCareDAL, IEncounterDAL encounterDAL, IFormInstanceDAL formInstanceDAL, IMapper mapper, ISkosConnector skosConnector)
        {
            this.episodeOfCareDAL = episodeOfCareDAL;
            this.encounterDAL = encounterDAL;
            this.formInstanceDAL = formInstanceDAL;
            this.mapper = mapper;
            this.skosConnector = skosConnector;
        }

        public async Task DeleteAsync(int eocId)
        {
            await episodeOfCareDAL.DeleteAsync(eocId);
            await formInstanceDAL.DeleteByEpisodeOfCareIdAsync(eocId);
        }

        public async Task<EpisodeOfCareDataOut> GetByIdAsync(int episodeOfCareId)
        {
            EpisodeOfCareDataOut episodeOfCareDataOut = mapper.Map<EpisodeOfCareDataOut>(await episodeOfCareDAL.GetByIdAsync(episodeOfCareId));
            if (episodeOfCareDataOut != null)
            {
                episodeOfCareDataOut.NumOfDocuments = await formInstanceDAL.CountAllEOCDocumentsAsync(episodeOfCareDataOut.Id, episodeOfCareDataOut.PatientId);
                episodeOfCareDataOut.NumOfEncounters = episodeOfCareDataOut.Encounters.Count;
            }
            else
            {
                episodeOfCareDataOut = new EpisodeOfCareDataOut();
            }
            episodeOfCareDataOut.UseSkosData = skosConnector.UseSkosData();

            return episodeOfCareDataOut;
        }

        public async Task<List<EpisodeOfCareDataOut>> GetByPatientIdAsync(EpisodeOfCareDataIn episodeOfCare)
        {
            EpisodeOfCareFilter filter = mapper.Map<EpisodeOfCareFilter>(episodeOfCare);
            List<EpisodeOfCare> episodeOfCareTask = await episodeOfCareDAL.GetByPatientIdFilteredAsync(filter);
            List<EpisodeOfCareDataOut> episodesOfCareDataOut = mapper.Map<List<EpisodeOfCareDataOut>>(episodeOfCareTask);

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

            EpisodeOfCare episodeOfCare = mapper.Map<EpisodeOfCare>(episodeOfCareDataIn);
            episodeOfCare.OrganizationId = userCookieData.ActiveOrganization;
            UserData userData = mapper.Map<UserData>(userCookieData);

            return await episodeOfCareDAL.InsertOrUpdateAsync(episodeOfCare, userData).ConfigureAwait(false);
        }
    }
}
