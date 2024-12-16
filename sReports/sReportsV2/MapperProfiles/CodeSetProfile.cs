using AutoMapper;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Sql.Entities.CodeSetEntry;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.DTOs.Autocomplete;
using sReportsV2.DTOs.DTOs.CodeSetEntry.DataIn;
using sReportsV2.DTOs.DTOs.CodeSetEntry.DataOut;

namespace sReportsV2.MapperProfiles
{
    public class CodeSetProfile : Profile
    {
        public CodeSetProfile() 
        {
            CreateMap<CodeSet, CodeSetDataOut>()
                .IgnoreAllNonExisting()
                .ForMember(o => o.CodeSetId, opt => opt.MapFrom(src => src.CodeSetId))
                .ForMember(o => o.Thesaurus, opt => opt.MapFrom(src => src.ThesaurusEntry))
                .ForMember(o => o.ActiveFrom, opt => opt.MapFrom(src => src.ActiveFrom))
                .ForMember(o => o.ActiveTo, opt => opt.MapFrom(src => src.ActiveTo))
                .ForMember(o => o.ApplicableInDesigner, opt => opt.MapFrom(src => src.ApplicableInDesigner));

            CreateMap<CodeSetDataIn, CodeSet>()
                .ForMember(dest => dest.RowVersion, opt => opt.Ignore())
                .IgnoreAllNonExisting()
                .ForMember(o => o.CodeSetId, opt => opt.MapFrom(src => src.CodeSetId))
                .ForMember(o => o.NewCodeSetId, opt => opt.MapFrom(src => src.NewCodeSetId))
                .ForMember(o => o.ThesaurusEntryId, opt => opt.MapFrom(src => src.ThesaurusEntryId))
                .ForMember(o => o.ActiveFrom, opt => opt.MapFrom(src => src.ActiveFrom))
                .ForMember(o => o.ActiveTo, opt => opt.MapFrom(src => src.ActiveTo))
                .ForMember(o => o.ApplicableInDesigner, opt => opt.MapFrom(src => src.ApplicableInDesigner))
                .AfterMap<CommonGlobalAfterMapping<CodeSet>>();

            CreateMap<CodeSetFilterDataIn, CodeSetFilter>()
                .IgnoreAllNonExisting();

            CreateMap<AutocompleteDataIn, CodeSetFilter>()
                .IgnoreAllNonExisting()
                .ForMember(o => o.CodeSetDisplay, opt => opt.MapFrom(src => src.Term))
                .ForMember(o => o.Page, opt => opt.MapFrom(src => src.Page));
        }
    }
}