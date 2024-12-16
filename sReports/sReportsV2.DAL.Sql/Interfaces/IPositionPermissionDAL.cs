using sReportsV2.Domain.Sql.Entities.AccessManagment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Interfaces
{
    public interface IPositionPermissionDAL
    {
        void InsertMany(List<PositionPermission> positionPermissions);
        List<PositionPermission> GetPermissionsForRole(int positionCD);
        void InsertOrUpdate(List<PositionPermission> checkedPermissionModules);
        int Count();

    }
}
