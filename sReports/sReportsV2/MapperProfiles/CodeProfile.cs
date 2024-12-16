using AutoMapper;
using sReportsV2.Cache.Singleton;
using sReportsV2.Domain.Sql.Entities.CodeEntry;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.DTOs.CodeEntry.DataOut;
using sReportsV2.DTOs.CodeEntry.DataIn;
using System.Linq;
using sReportsV2.DTOs.DTOs.CodeEntry.DataIn;
using sReportsV2.Common.Extensions;

namespace sReportsV2.MapperProfiles
{
    public class CodeProfile : Profile
    {
        public CodeProfile()
        {
            CreateMap<Code, CodeDataOut>()
                .IgnoreAllNonExisting()
                .ForMember(o => o.Id, opt => opt.MapFrom(src => src.CodeId))
                .ForMember(o => o.Thesaurus, opt => opt.MapFrom(src => src.ThesaurusEntry))
                .ForMember(o => o.CodeSetId, opt => opt.MapFrom(src => src.CodeSetId))
                .ForMember(o => o.ActiveFrom, opt => opt.MapFrom(src => src.ActiveFrom))
                .ForMember(o => o.ActiveTo, opt => opt.MapFrom(src => src.ActiveTo));

            CreateMap<CodeDataIn, Code>()
                .ForMember(dest => dest.RowVersion, opt => opt.Ignore())
                .IgnoreAllNonExisting()
                .ForMember(o => o.CodeId, opt => opt.MapFrom(src => src.Id))
                .ForMember(o => o.ThesaurusEntryId, opt => opt.MapFrom(src => src.ThesaurusEntryId))
                .ForMember(o => o.CodeSetId, opt => opt.MapFrom(src => src.CodeSetId))
                .ForMember(o => o.ActiveFrom, opt => opt.MapFrom(src => src.ActiveFrom))
                .ForMember(o => o.ActiveTo, opt => opt.MapFrom(src => src.ActiveTo))
                .AfterMap<CommonGlobalAfterMapping<Code>>();

            CreateMap<CodeFilterDataIn, CodeFilter>();

            CreateMap<FormCodeRelationDataIn, FormCodeRelation>()
                .IgnoreAllNonExisting()
                .ForMember(o => o.CodeCD, opt => opt.MapFrom(src => src.CodeId))
                .ForMember(o => o.FormId, opt => opt.MapFrom(src => src.FormId))
                .AfterMap<CommonGlobalAfterMapping<FormCodeRelation>>();
        }
    }
}