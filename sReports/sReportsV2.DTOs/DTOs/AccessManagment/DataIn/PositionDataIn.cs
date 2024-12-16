using System.Collections.Generic;

namespace sReportsV2.DTOs.DTOs.AccessManagment.DataIn
{
    public class PositionDataIn
    {
        public int Id { get; set; }
        public List<PositionPermissionDataIn> CheckedPermissionModules { get; set; }
    }
}
