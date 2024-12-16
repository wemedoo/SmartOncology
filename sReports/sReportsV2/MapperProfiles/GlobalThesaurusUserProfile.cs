using AutoMapper;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Sql.Entities.GlobalThesaurusUser;
using sReportsV2.DTOs.DTOs.AccessManagment.DataOut;
using sReportsV2.DTOs.DTOs.GlobalThesaurusUser.DataIn;
using sReportsV2.DTOs.DTOs.GlobalThesaurusUser.DataOut;
using sReportsV2.DTOs.User.DTO;
using System.Linq;

namespace sReportsV2.MapperProfiles
{
    public class GlobalThesaurusUserProfile : Profile
    {
        public GlobalThesaurusUserProfile() 
        {
            CreateMap<GlobalThesaurusUser, GlobalThesaurusUserDataIn>()
                .ForMember(u => u.Roles , opt => opt.Ignore())
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.GlobalThesaurusUserId))
                .ReverseMap();

            CreateMap<GlobalThesaurusUser, GlobalThesaurusUserDataOut>()
                .ForMember(d => d.Roles, opt => opt.MapFrom(src => src.GlobalThesaurusUserRoles.Where(x => !x.IsDeleted()).ToList()))
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.GlobalThesaurusUserId));

            CreateMap<GlobalThesaurusUserRole, RoleDataOut>()
                .IgnoreAllNonExisting()
                .ForMember(u => u.Id, opt => opt.MapFrom(src => src.GlobalThesaurusRole.GlobalThesaurusRoleId))
                .ForMember(u => u.Name, opt => opt.MapFrom(src => src.GlobalThesaurusRole.Name))
                .ForMember(u => u.Description, opt => opt.MapFrom(src => src.GlobalThesaurusRole.Description));

            CreateMap<GlobalThesaurusRole, RoleDataOut>()
                .IgnoreAllNonExisting()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.GlobalThesaurusRoleId));

            CreateMap<GlobalThesaurusUser, UserCookieData>()
                .IgnoreAllNonExisting()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.GlobalThesaurusUserId))
                .ForMember(d => d.Username, opt => opt.MapFrom(src => src.Email))
                .ForMember(d => d.Roles, opt => opt.MapFrom(src => src.GlobalThesaurusUserRoles.Where(x => !x.IsDeleted()).ToList()))
                .ReverseMap();
        }
    }
}