using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.DTOs.TaskEntry.DataIn;
using sReportsV2.DTOs.DTOs.TaskEntry.DataOut;
using sReportsV2.DTOs.Pagination;
using System.Threading.Tasks;

namespace sReportsV2.BusinessLayer.Interfaces
{
    public interface ITaskBLL
    {
        Task<TaskDataOut> GetByIdAsync(int taskId);
        Task<int> InsertOrUpdateAsync(TaskDataIn taskData);
        Task<PaginationDataOut<TaskDataOut, DataIn>> GetAllFiltered(TaskFilterDataIn dataIn);
    }
}