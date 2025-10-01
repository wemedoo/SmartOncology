using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Domain.Sql.Entities.UploadPatientData;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Interfaces
{
    public interface IUploadPatientDataDAL
    {
        Task<UploadPatientData> GetById(int uploadPatientDataId);
        void InsertOrUpdate(List<UploadPatientData> patient);
        Task<PaginationData<UploadPatientData>> GetAllAndCount(UploadPatientDataFilter filter);
    }
}
