using System.Collections.Generic;

namespace sReportsV2.DTOs.DTOs.AccessManagment.DataOut
{
    public class RoleDataOut
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<PermissionRoleDataOut> Permissions { get; set; } = new List<PermissionRoleDataOut>();

    }
}
