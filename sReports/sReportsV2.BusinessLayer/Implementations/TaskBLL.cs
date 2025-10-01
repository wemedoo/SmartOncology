using AutoMapper;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Sql.Entities.TaskEntry;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.DTOs.TaskEntry.DataIn;
using sReportsV2.DTOs.DTOs.TaskEntry.DataOut;
using sReportsV2.DTOs.Pagination;
using sReportsV2.SqlDomain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sReportsV2.BusinessLayer.Implementations
{
    public class TaskBLL : ITaskBLL
    {
        private readonly ITaskDAL taskDAL;
        private readonly IMapper mapper;

        public TaskBLL(ITaskDAL taskDAL, IMapper mapper) 
        {
            this.taskDAL = taskDAL;
            this.mapper = mapper;
        }

        public async Task<TaskDataOut> GetByIdAsync(int taskId)
        {
            Domain.Sql.Entities.TaskEntry.Task task = await taskDAL.GetByIdAsync(taskId);

            return mapper.Map<TaskDataOut>(task);
        }

        public async Task<int> InsertOrUpdateAsync(TaskDataIn taskData)
        {
            taskData = Ensure.IsNotNull(taskData, nameof(taskData));
            Domain.Sql.Entities.TaskEntry.Task task = mapper.Map<Domain.Sql.Entities.TaskEntry.Task>(taskData);

            return await taskDAL.InsertOrUpdateAsync(task).ConfigureAwait(false);
        }

        public async Task<PaginationDataOut<TaskDataOut, DataIn>> GetAllFiltered(TaskFilterDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));

            TaskFilter filter = mapper.Map<TaskFilter>(dataIn);

            var dataTask = await taskDAL.GetAllAsync(filter);
            var countTask = await taskDAL.GetAllEntriesCountAsync(filter);

            PaginationDataOut<TaskDataOut, DataIn> result = new PaginationDataOut<TaskDataOut, DataIn>()
            {
                Count = countTask,
                Data = mapper.Map<List<TaskDataOut>>(dataTask),
                DataIn = dataIn
            };

            return result;
        }
    }
}