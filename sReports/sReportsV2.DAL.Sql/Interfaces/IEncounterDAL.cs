using sReportsV2.Domain.Sql.Entities.Encounter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Interfaces
{
    public interface IEncounterDAL
    {
        int InsertOrUpdate(Encounter encounter);
        Task<int> InsertOrUpdateAsync(Encounter encounter);
        Task<List<Encounter>> GetByEOCIdAsync(int eocId);
        List<Encounter> GetAllByEocId(int eocId);
        Task<List<Encounter>> GetAllByEocIdAsync(int eocId);
        Task<Encounter> GetByIdAsync(int id);
        Encounter GetById(int id);
        Task DeleteAsync(int encounterId);
        bool ThesaurusExist(int thesaurusId);
        List<Encounter> GetByEncounterTypeAndEpisodeOfCareId(int encounterTypeId, int episodeOfCareId);
        int GetEncounterTypeByEncounterId(int encounterId);
        int CountAllEncounters(int episodeOfCareId);
        Task<int> CountAllEncountersAsync(int episodeOfCareId);
        Task<int> GetAllEntriesCountAsync(EncounterFilter filter);
        Task<List<EncounterView>> GetAllAsync(EncounterFilter filter);
        List<Encounter> GetAll();
        List<Encounter> GetAllInactive();
    }
}
