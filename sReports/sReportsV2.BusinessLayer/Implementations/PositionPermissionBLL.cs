using AutoMapper;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Domain.Sql.Entities.AccessManagment;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.DTOs.AccessManagment.DataIn;
using sReportsV2.DTOs.DTOs.AccessManagment.DataOut;
using sReportsV2.SqlDomain.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.BusinessLayer.Implementations
{
    public class PositionPermissionBLL : IPositionPermissionBLL
    {
        private readonly IModuleDAL moduleDAL;
        private readonly IPositionPermissionDAL positionPermissionDAL;
        private readonly IMapper Mapper;

        public PositionPermissionBLL(IModuleDAL moduleDAL, IPositionPermissionDAL positionPermissionDAL, IMapper mapper)
        {
            this.moduleDAL = moduleDAL;
            this.positionPermissionDAL = positionPermissionDAL;
            Mapper = mapper;
        }

        public List<ModuleDataOut> GetModules()
        {
            return Mapper.Map<List<ModuleDataOut>>(moduleDAL.GetAll());
        }

        public List<int> GetPermissionsForRole(int positionCD)
        {
            return positionPermissionDAL.GetPermissionsForRole(positionCD).Select(x => x.PermissionModuleId.GetValueOrDefault()).ToList();
        }

        public CreateResponseResult InsertOrUpdate(PositionDataIn positionDataIn)
        {
            positionPermissionDAL.InsertOrUpdate(Mapper.Map<List<PositionPermission>>(positionDataIn.CheckedPermissionModules));
            return new CreateResponseResult()
            {
                Id = positionDataIn.Id,
            };
        }
    }
}
