using sReportsV2.Domain.Sql.Entities.QueryManagement;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Interfaces
{
    public interface IQueryManagementDAL
    {
        Task<Query> GetById(int id);
        Task<List<Query>> GetListById(int id);
        Task<int> Create(Query query, int? userId = null);
        Task<int> Update(Query query, int userId, List<QueryHistory> historyList = null);
        Task Delete(int queryId);
        Task<List<Query>> GetAll(QueryFilter filter);
        Task<int> GetAllEntriesCount(QueryFilter filter);
        Task<List<Query>> GetByFieldId(QueryFilter filter);
        List<Query> GetByFieldIds(List<string> fieldIds);
    }
}
