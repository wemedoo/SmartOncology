using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.DTOs.AccessManagment.DataIn;
using sReportsV2.DTOs.DTOs.AccessManagment.DataOut;
using System.Collections.Generic;

namespace sReportsV2.BusinessLayer.Interfaces
{
    public interface IPositionPermissionBLL
    {
        List<int> GetPermissionsForRole(int positionCD);
        CreateResponseResult InsertOrUpdate(PositionDataIn positionDataIn);
        List<ModuleDataOut> GetModules();
    }
}
