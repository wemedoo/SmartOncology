using AutoMapper;
using sReportsV2.Cache.Singleton;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Entities.DigitalGuideline;
using sReportsV2.Domain.Entities.DigitalGuideline.EvidenceProperties;
using sReportsV2.DTOs.DigitalGuideline.DataIn;
using sReportsV2.DTOs.DigitalGuideline.DataIn.EvidenceProperties;
using sReportsV2.DTOs.DigitalGuideline.DataOut;
using sReportsV2.DTOs.DigitalGuideline.DataOut.EvidenceProperties;
using sReportsV2.DTOs.DigitalGuideline.DTO;
using System.Linq;

namespace sReportsV2.MapperProfiles
{
    public class DigitalGuidelineProfile : Profile
    {
        public DigitalGuidelineProfile()
        {
            CreateMap<GuidelineFilter, GuidelineFilterDataIn>().ReverseMap();

            CreateMap<GuidelineDataIn, Guideline>()
                .IgnoreAllNonExisting()
                .ReverseMap();

            CreateMap<Guideline, GuidelineDataOut>()
                .ReverseMap();

            CreateMap<GuidelineElementsDataIn, GuidelineElements>().ReverseMap();

            CreateMap<GuidelineElements, GuidelineElementsDataOut>().ReverseMap();

            CreateMap<GuidelineElementDataIn, GuidelineElement>().ReverseMap();

            CreateMap<GuidelineElement, GuidelineElementDataOut>().ReverseMap();

            CreateMap<GuidelineElementDataDataIn, GuidelineElementData>()
                .ReverseMap();

            CreateMap<GuidelineElementData, GuidelineElementDataDataOut>()
                .IgnoreAllNonExisting()
                .ReverseMap();

            CreateMap<GuidelineStatementElementDataDataIn, GuidelineStatementElementData>()
                .IncludeBase<GuidelineElementDataDataIn, GuidelineElementData>();

            CreateMap<GuidelineStatementElementData, GuidelineStatementElementDataDataOut>()
                .IgnoreAllNonExisting()
                .IncludeBase<GuidelineElementData, GuidelineElementDataDataOut>();

            CreateMap<GuidelineEdgeElementDataDataIn, GuidelineEdgeElementData>()
                .IncludeBase<GuidelineElementDataDataIn, GuidelineElementData>();

            CreateMap<GuidelineEdgeElementData, GuidelineEdgeElementDataDataOut>()
                .IgnoreAllNonExisting()
                .IncludeBase<GuidelineElementData, GuidelineElementDataDataOut>();

            CreateMap<GuidelineDecisionElementDataDataIn, GuidelineDecisionElementData>()
                .IncludeBase<GuidelineElementDataDataIn, GuidelineElementData>();

            CreateMap<GuidelineDecisionElementData, GuidelineDecisionElementDataDataOut>()
                .IgnoreAllNonExisting()
                .IncludeBase<GuidelineElementData, GuidelineElementDataDataOut>();

            CreateMap<GuidelineEventElementDataDataIn, GuidelineEventElementData>()
                .IncludeBase<GuidelineElementDataDataIn, GuidelineElementData>();

            CreateMap<GuidelineEventElementData, GuidelineEventElementDataDataOut>()
                .IgnoreAllNonExisting()
                .IncludeBase<GuidelineElementData, GuidelineElementDataDataOut>();

            CreateMap<Position, PositionDTO>().ReverseMap();

            CreateMap<EvidencePropertiesDataIn, EvidenceProperties>().ReverseMap();

            CreateMap<EvidenceProperties, EvidencePropertiesDataOut>().ReverseMap();

            CreateMap<EvidenceCategoryDataIn, EvidenceCategory>()
                .ForMember(o => o.StrengthOfRecommendation, opt => opt.MapFrom(src => src.StrengthOfRecommendation.Id))
                .ForMember(o => o.OxfordLevelOfEvidenceSystem, opt => opt.MapFrom(src => src.OxfordLevelOfEvidenceSystem.Id))
                ;
            CreateMap<NCCNEvidenceCategoryDataIn, NCCNEvidenceCategory>()
                 .ForMember(o => o.CategoryOfEvidenceAndConsensus, opt => opt.MapFrom(src => src.CategoryOfEvidenceAndConsensus.Id))
                .ForMember(o => o.CategoryOfPreference, opt => opt.MapFrom(src => src.CategoryOfPreference.Id))
                ;


            CreateMap<EvidenceCategory, EvidenceCategoryDataOut>()
            .ForMember(o => o.OxfordLevelOfEvidenceSystem, opt => opt.MapFrom(src => SingletonDataContainer.Instance.GetOxfordLevelOfEvidenceSystem().FirstOrDefault(x => x.Thesaurus.Id.ToString().Equals(src.OxfordLevelOfEvidenceSystem)).Thesaurus))
            .ForMember(o => o.StrengthOfRecommendation, opt => opt.MapFrom(src => SingletonDataContainer.Instance.GetStrengthOfRecommendation().FirstOrDefault(x => x.Thesaurus.Id.ToString().Equals(src.StrengthOfRecommendation)).Thesaurus));

            CreateMap<NCCNEvidenceCategory, NCCNEvidenceCategoryDataOut>()
                .ForMember(o => 
                o.CategoryOfEvidenceAndConsensus, 
                opt => opt.MapFrom(src => SingletonDataContainer.Instance.GetNCCNCategoriesOfEvidenceAndConsensus().FirstOrDefault(x => x.Thesaurus.Id.ToString().Equals(src.CategoryOfEvidenceAndConsensus)).Thesaurus))
                .ForMember(o => 
                o.CategoryOfPreference, 
                opt => opt.MapFrom(src => SingletonDataContainer.Instance.GetNCCNCategoriesOfPreference().FirstOrDefault(x => x.Thesaurus.Id.ToString().Equals(src.CategoryOfPreference)).Thesaurus));

            
            CreateMap<Publication, PublicationDTO>().ReverseMap();

            CreateMap<GuidelineElementDataIn, GuidelineElementDataOut>().ReverseMap();

            CreateMap<GuidelineElementDataDataIn, GuidelineElementDataDataOut>()
                .IgnoreAllNonExisting()
                .ReverseMap();

            CreateMap<GuidelineStatementElementDataDataIn, GuidelineStatementElementDataDataOut>()
                .IgnoreAllNonExisting()
                .IncludeBase<GuidelineElementDataDataIn, GuidelineElementDataDataOut>();

            CreateMap<GuidelineEdgeElementDataDataIn, GuidelineEdgeElementDataDataOut>()
                .IgnoreAllNonExisting()
                .IncludeBase<GuidelineElementDataDataIn, GuidelineElementDataDataOut>();

            CreateMap<GuidelineDecisionElementDataDataIn, GuidelineDecisionElementDataDataOut>()
                .IgnoreAllNonExisting()
                .IncludeBase<GuidelineElementDataDataIn, GuidelineElementDataDataOut>();


            CreateMap<GuidelineEventElementDataDataIn, GuidelineEventElementDataDataOut>()
                .IgnoreAllNonExisting()
                .IncludeBase<GuidelineElementDataDataIn, GuidelineElementDataDataOut>();

            CreateMap<EvidencePropertiesDataIn, EvidencePropertiesDataOut>();

            CreateMap<EvidenceCategoryDataIn, EvidenceCategoryDataOut>()
            .ForMember(o => o.OxfordLevelOfEvidenceSystem, opt => opt.MapFrom(src => SingletonDataContainer.Instance.GetOxfordLevelOfEvidenceSystem().FirstOrDefault(x => x.Thesaurus.Id.ToString().Equals(src.OxfordLevelOfEvidenceSystem.Id)).Thesaurus))
            .ForMember(o => o.StrengthOfRecommendation, opt => opt.MapFrom(src => SingletonDataContainer.Instance.GetStrengthOfRecommendation().FirstOrDefault(x => x.Thesaurus.Id.ToString().Equals(src.StrengthOfRecommendation.Id)).Thesaurus));

            CreateMap<NCCNEvidenceCategoryDataIn, NCCNEvidenceCategoryDataOut>()
                .ForMember(o =>
                o.CategoryOfEvidenceAndConsensus,
                opt => opt.MapFrom(src => SingletonDataContainer.Instance.GetOxfordLevelOfEvidenceSystem().FirstOrDefault(x => x.Thesaurus.Id.ToString().Equals(src.CategoryOfEvidenceAndConsensus.Id)).Thesaurus))
                .ForMember(o =>
                o.CategoryOfPreference,
                opt => opt.MapFrom(src => SingletonDataContainer.Instance.GetStrengthOfRecommendation().FirstOrDefault(x => x.Thesaurus.Id.ToString().Equals(src.CategoryOfPreference.Id)).Thesaurus));
        }
    }
}