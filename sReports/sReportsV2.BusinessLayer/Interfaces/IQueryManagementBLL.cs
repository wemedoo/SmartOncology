using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.DTOs.QueryManagement.DataIn;
using sReportsV2.DTOs.DTOs.QueryManagement.DataOut;
using sReportsV2.DTOs.Pagination;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sReportsV2.BusinessLayer.Interfaces
{
    public interface IQueryManagementBLL
    {
        Task<QueryDataOut> GetById(int id);
        Task<List<QueryDataOut>> GetListById(int id);
        Task<int> Create(QueryDataIn dataIn, int userId);
        Task<List<int>> Update(List<QueryDataIn> dataInList, int userId);
        Task Delete(int id);
        Task<PaginationDataOut<QueryDataOut, DataIn>> GetAllFiltered(QueryFilterDataIn dataIn);
        Task<List<QueryDataOut>> GetByFieldId(QueryFilterDataIn dataIn);
    }
}
