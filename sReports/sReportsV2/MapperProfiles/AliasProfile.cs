using AutoMapper;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Sql.Entities.Aliases;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.DTOs.DTOs.CodeAliases.DataIn;
using sReportsV2.DTOs.DTOs.CodeAliases.DataOut;

namespace sReportsV2.MapperProfiles
{
    public class AliasProfile : Profile
    {
        public AliasProfile() 
        {
            CreateMap<CodeAliasView, CodeAliasViewDataOut>()
                .ForMember(o => o.CodeId, opt => opt.MapFrom(src => src.CodeId))
                .ForMember(o => o.CodeSetId, opt => opt.MapFrom(src => src.CodeSetId))
                .ForMember(o => o.InboundAlias, opt => opt.MapFrom(src => src.InboundAlias))
                .ForMember(o => o.AliasId, opt => opt.MapFrom(src => src.AliasId))
                .ForMember(o => o.System, opt => opt.MapFrom(src => src.System))
                .ForMember(o => o.OutboundAlias, opt => opt.MapFrom(src => src.OutboundAlias))
                .ForMember(o => o.ActiveFrom, opt => opt.MapFrom(src => src.ActiveFrom))
                .ForMember(o => o.ActiveTo, opt => opt.MapFrom(src => src.ActiveTo))
                .ForMember(o => o.InboundAliasId, opt => opt.MapFrom(src => src.InboundAliasId))
                .ForMember(o => o.OutboundAliasId, opt => opt.MapFrom(src => src.OutboundAliasId))
                ;

            CreateMap<CodeAliasDataIn, InboundAlias>()
                .IgnoreAllNonExisting()
                .ForMember(o => o.CodeId, opt => opt.MapFrom(src => src.CodeId))
                .ForMember(o => o.AliasId, opt => opt.MapFrom(src => src.InboundAliasId))
                .ForMember(o => o.Alias, opt => opt.MapFrom(src => src.Inbound))
                .ForMember(o => o.System, opt => opt.MapFrom(src => src.System))
                .ForMember(o => o.ActiveFrom, opt => opt.MapFrom(src => src.ActiveFrom))
                .ForMember(o => o.ActiveTo, opt => opt.MapFrom(src => src.ActiveTo))
                .AfterMap<CommonGlobalAfterMapping<InboundAlias>>()
                ;

            CreateMap<CodeAliasDataIn, OutboundAlias>()
                .IgnoreAllNonExisting()
                .ForMember(o => o.CodeId, opt => opt.MapFrom(src => src.CodeId))
                .ForMember(o => o.AliasId, opt => opt.MapFrom(src => src.OutboundAliasId))
                .ForMember(o => o.Alias, opt => opt.MapFrom(src => src.Outbound))
                .ForMember(o => o.System, opt => opt.MapFrom(src => src.System))
                .ForMember(o => o.ActiveFrom, opt => opt.MapFrom(src => src.ActiveFrom))
                .ForMember(o => o.ActiveTo, opt => opt.MapFrom(src => src.ActiveTo))
                .AfterMap<CommonGlobalAfterMapping<OutboundAlias>>()
                ;

            CreateMap<InboundAlias, AliasDataOut>();

            CreateMap<OutboundAlias, AliasDataOut>();

            CreateMap<CodeAliasFilterDataIn, CodeAliasFilter>();
        }
    }
}