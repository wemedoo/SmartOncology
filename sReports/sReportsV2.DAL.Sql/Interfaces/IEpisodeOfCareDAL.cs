using sReportsV2.Common.Entities.User;
using sReportsV2.Domain.Sql.Entities.EpisodeOfCare;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Interfaces
{
    public interface IEpisodeOfCareDAL : IReplaceThesaurusDAL
    {
        Task DeleteAsync(int eocId);
        EpisodeOfCare GetById(int id);
        Task<EpisodeOfCare> GetByIdAsync(int id);
        List<EpisodeOfCare> GetByPatientId(int patientId);
        int InsertOrUpdate(EpisodeOfCare entity, UserData user);
        Task<int> InsertOrUpdateAsync(EpisodeOfCare entity, UserData user);
        Task<List<EpisodeOfCare>> GetByPatientIdFilteredAsync(EpisodeOfCareFilter filter);
    }
}
