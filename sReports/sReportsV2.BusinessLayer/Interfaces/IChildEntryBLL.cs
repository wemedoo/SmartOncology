using sReportsV2.DTOs.Common.DTO;
using System.Threading.Tasks;

namespace sReportsV2.BusinessLayer.Interfaces
{
    public interface IChildEntryBLL<T>
    {
        Task<ResourceCreatedDTO> InsertOrUpdate(T childDataIn);
        Task Delete(T childDataIn);
    }
}
