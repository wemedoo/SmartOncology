using sReportsV2.Common.Entities.User;
using sReportsV2.Domain.Sql.Entities.EpisodeOfCare;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Interfaces
{
    public interface IEpisodeOfCareDAL
    {
        Task DeleteAsync(int eocId);
        EpisodeOfCare GetById(int id);
        Task<EpisodeOfCare> GetByIdAsync(int id);
        List<EpisodeOfCare> GetByPatientId(int patientId);
        Task<List<EpisodeOfCare>> GetAllAsync(EpisodeOfCareFilter filter);
        Task<long> GetAllEntriesCountAsync(EpisodeOfCareFilter filter);
        int InsertOrUpdate(EpisodeOfCare entity, UserData user);
        Task<int> InsertOrUpdateAsync(EpisodeOfCare entity, UserData user);
        bool ThesaurusExist(int thesaurusId);
        void UpdateManyWithThesaurus(int oldThesaurus, int newThesaurus);
        Task<List<EpisodeOfCare>> GetByPatientIdFilteredAsync(EpisodeOfCareFilter filter);
    }
}
