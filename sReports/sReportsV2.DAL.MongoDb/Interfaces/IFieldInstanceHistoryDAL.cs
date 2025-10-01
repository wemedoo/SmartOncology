using sReportsV2.Domain.Entities.FieldInstanceHistory;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Services.Interfaces
{
    public interface IFieldInstanceHistoryDAL
    {
        Task InsertManyAsync(List<FieldInstanceHistory> fieldInstanceHistories);
        Task<int> UpdateManyAsync(List<FieldInstanceHistory> fieldInstanceHistories);
        Task<List<FieldInstanceHistory>> GetAllFilteredAsync(FieldInstanceHistoryFilterData filter);
        Task<int> CountFilteredAsync(FieldInstanceHistoryFilterData filter);
    }
}
