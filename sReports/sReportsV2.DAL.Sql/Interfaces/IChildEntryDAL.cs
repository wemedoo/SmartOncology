using sReportsV2.Domain.Sql.Entities.Common;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Interfaces
{
    public interface IChildEntryDAL<T>
    {
        public Task<T> GetById(QueryEntityParam<T> queryEntityParams);
        public Task InsertOrUpdate(T entry);
        public Task Delete(T entry);
    }
}
