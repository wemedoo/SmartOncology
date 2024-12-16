using sReportsV2.Domain.Sql.Entities.ClinicalTrial;
using sReportsV2.Domain.Sql.Entities.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Interfaces
{
    public interface ITrialManagementDAL
    {
        Task<ClinicalTrial> InsertOrUpdate(ClinicalTrial trial);
        Task<ClinicalTrial> GetByProjectId(int projectId);
        Task<int> Delete(int id);
        Task<int> Archive(int id);
        Task<PaginationData<AutoCompleteData>> GetTrialAutoCompleteTitleAndCount(TrialManagementFilter filter);
        List<ClinicalTrial> GetlClinicalTrialsByName(string name);
        List<ClinicalTrial> GetlClinicalTrialByIds(List<int> ids);
    }
}
