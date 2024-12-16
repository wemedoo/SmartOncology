using AutoMapper;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Sql.Entities.CodeEntry;
using sReportsV2.DTOs.DTOs.CodeAssociation.DataIn;
using sReportsV2.DTOs.DTOs.CodeAssociation.DataOut;

namespace sReportsV2.MapperProfiles
{
    public class CodeAssociationProfile : Profile
    {
        public CodeAssociationProfile() 
        {
            CreateMap<CodeAssociation, CodeAssociationDataOut>()
                .ForMember(o => o.CodeAssociationId, opt => opt.MapFrom(src => src.CodeAssociationId))
                .ForMember(o => o.ParentId, opt => opt.MapFrom(src => src.ParentId))
                .ForMember(o => o.Parent, opt => opt.MapFrom(src => src.Parent))
                .ForMember(o => o.ChildId, opt => opt.MapFrom(src => src.ChildId))
                .ForMember(o => o.Child, opt => opt.MapFrom(src => src.Child))
                ;

            CreateMap<CodeAssociationDataIn, CodeAssociation>()
                .IgnoreAllNonExisting()
                .ForMember(o => o.ParentId, opt => opt.MapFrom(src => src.ParentId))
                .ForMember(o => o.ChildId, opt => opt.MapFrom(src => src.ChildId))
                .AfterMap<CommonGlobalAfterMapping<CodeAssociation>>();

            CreateMap<CodeAssociationFilterDataIn, CodeAssociationFilter>()
                .IgnoreAllNonExisting();
        }
    }
}