using AutoMapper;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Sql.Entities.AccessManagment;
using sReportsV2.Domain.Sql.Entities.RoleEntry;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.DTOs.AccessManagment.DataIn;
using sReportsV2.DTOs.DTOs.AccessManagment.DataOut;

namespace sReportsV2.MapperProfiles
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<Module, ModuleDataOut>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.ModuleId));

            CreateMap<Permission, PermissionDataOut>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.PermissionId));

            CreateMap<PermissionModule, PermissionDataOut>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.PermissionModuleId))
                .ForMember(d => d.Name, opt => opt.MapFrom(src => src.Permission.Name))
                .ForMember(d => d.Description, opt => opt.MapFrom(src => src.Permission.Description));

            CreateMap<DataIn, RoleFilter>().IgnoreAllNonExisting();

            CreateMap<PositionPermissionDataIn, PositionPermission>()
                .IgnoreAllNonExisting()
                .ForMember(d => d.PositionCD, opt => opt.MapFrom(src => src.PositionId))
                .ForMember(d => d.PermissionModuleId, opt => opt.MapFrom(src => src.PermissionModuleId))
                .AfterMap<CommonGlobalAfterMapping<PositionPermission>>();
        }
    }
}