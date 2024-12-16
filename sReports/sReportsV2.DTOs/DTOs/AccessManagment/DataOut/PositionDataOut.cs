using sReportsV2.DTOs.CodeEntry.DataOut;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.DTOs.DTOs.AccessManagment.DataOut
{
    public class PositionDataOut
    {
        public CodeDataOut Position { get; set; }
        public List<ModuleDataOut> Modules { get; set; }
        public List<int> Permissions { get; set; }
        public bool IsPermissionChecked(int permissionId)
        {
            return Permissions.Contains(permissionId);
        }
    }
}
