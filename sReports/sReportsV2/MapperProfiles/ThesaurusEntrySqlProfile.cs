using AutoMapper;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Sql.Entities.CodeSystem;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using sReportsV2.DTOs.Administration;
using sReportsV2.DTOs.CodeSystem;
using sReportsV2.DTOs.DTOs.CodeSystem;
using sReportsV2.DTOs.DTOs.GlobalThesaurus.DataIn;
using sReportsV2.DTOs.DTOs.ThesaurusEntry.DataOut;
using sReportsV2.DTOs.O4CodeableConcept.DataIn;
using sReportsV2.DTOs.O4CodeableConcept.DataOut;
using sReportsV2.DTOs.ThesaurusEntry;
using sReportsV2.DTOs.ThesaurusEntry.DataIn;
using sReportsV2.DTOs.ThesaurusEntry.DataOut;
using sReportsV2.DTOs.ThesaurusEntry.DTO;
using System.Linq;

namespace sReportsV2.MapperProfiles
{
    public class ThesaurusEntrySqlProfile : Profile
    {
        public ThesaurusEntrySqlProfile() 
        {
            CreateMap<ThesaurusEntryTranslation, ThesaurusEntryTranslationDTO>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.ThesaurusEntryTranslationId));

            CreateMap<ThesaurusFilterDataIn, ThesaurusFilter>()
                .ReverseMap();

            CreateMap<O4CodeableConcept, O4CodeableConceptDataOut>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.O4CodeableConceptId))
                .ReverseMap();

            CreateMap<O4CodeableConceptDataIn, O4CodeableConcept>()
                .IgnoreAllNonExisting()
                .ForMember(d => d.O4CodeableConceptId, opt => opt.MapFrom(src => src.Id))
                .ReverseMap();

            CreateMap<ThesaurusEntryDataIn, ThesaurusEntry>()
                .IgnoreAllNonExisting()
                .ForMember(d => d.ThesaurusEntryId, opt => opt.MapFrom(src => src.Id))
                .AfterMap<CommonGlobalAfterMapping<ThesaurusEntry>>();

            CreateMap<ThesaurusEntry, ThesaurusEntryDataIn>()
                .IgnoreAllNonExisting()
                .ForMember(x => x.UmlsCode, opt => opt.Ignore())
                .ForMember(x => x.UmlsDefinitions, opt => opt.Ignore())
                .ForMember(x => x.UmlsName, opt => opt.Ignore())
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.ThesaurusEntryId));

            CreateMap<ThesaurusEntry, ThesaurusEntryDataOut>()
                .IgnoreAllNonExisting()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.ThesaurusEntryId))
                .ForMember(d => d.Codes, opt => opt.MapFrom(src => src.Codes.Where(x => !x.IsDeleted)))
                .ReverseMap();

            CreateMap<AdministrativeDataDataOut, AdministrativeData>()
                .IgnoreAllNonExisting()
                .ReverseMap();

            CreateMap<Version, VersionDataOut>()
                .IgnoreAllNonExisting()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.VersionId))
                .ReverseMap();

            CreateMap<ThesaurusEntryTranslation, ThesaurusEntryTranslationDataIn>()
                .IgnoreAllNonExisting()
                .ReverseMap();

            CreateMap<GlobalThesaurusFilter, GlobalThesaurusFilterDataIn>()
                .ReverseMap();

            CreateMap<ThesaurusEntryFilterDataIn, ThesaurusEntryFilterData>()
                .IgnoreAllNonExisting()
                .ReverseMap();

            CreateMap<ThesaurusReviewFilterDataIn, ThesaurusReviewFilterData>()
                .IgnoreAllNonExisting()
                .ReverseMap();

            CreateMap<CodeSystem, CodeSystemDataOut>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.CodeSystemId))
                .ForMember(d => d.Label, opt => opt.MapFrom(src => src.Label))
                .ForMember(d => d.SAB, opt => opt.MapFrom(src => src.SAB))
                .ForMember(d => d.Value, opt => opt.MapFrom(src => src.Value));

            CreateMap<AdministrationFilterDataIn, ThesaurusEntryFilterData>()
                .IgnoreAllNonExisting()
                .ForMember(d => d.PreferredTerm, opt => opt.MapFrom(src => src.PreferredTerm))
                .ForMember(d => d.Page, opt => opt.MapFrom(src => src.Page))
                .ForMember(d => d.PageSize, opt => opt.MapFrom(src => src.PageSize))
                .ReverseMap();

            CreateMap<CodeSystem, CodeSystemDataIn>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.CodeSystemId))
                .ReverseMap();

            CreateMap<ThesaurusMergeDataIn, ThesaurusMerge>()
                .IgnoreAllNonExisting()
                .ForMember(d => d.StateCD, opt => opt.MapFrom(src => src.StateCD))
                .ForMember(d => d.OldThesaurus, opt => opt.MapFrom(src => src.CurrentId))
                .ForMember(d => d.NewThesaurus, opt => opt.MapFrom(src => src.TargetId))
                .AfterMap<CommonGlobalAfterMapping<ThesaurusMerge>>();

            CreateMap<ThesaurusEntryView, ThesaurusEntryViewDataOut>();
        }
        
    }
}