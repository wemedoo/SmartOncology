using AutoMapper;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Entities.DigitalGuidelineInstance;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.DigitalGuidelineInstance.DataIn;
using sReportsV2.DTOs.DigitalGuidelineInstance.DataOut;

namespace sReportsV2.MapperProfiles
{
    public class DigitalGuidelineInstanceProfile : Profile
    {
        public DigitalGuidelineInstanceProfile() 
        {
            CreateMap<Period, PeriodDTO>()
              .ForMember(o => o.StartDate, opt => opt.MapFrom(src => src.Start))
              .ForMember(o => o.EndDate, opt => opt.MapFrom(src => src.End));

            CreateMap<PeriodDTO, Period>()
              .ForMember(o => o.Start, opt => opt.MapFrom(src => src.StartDate))
              .ForMember(o => o.End, opt => opt.MapFrom(src => src.EndDate));

            CreateMap<GuidelineInstanceDataIn, GuidelineInstance>()
               .IgnoreAllNonExisting()
               .ForMember(o => o.Id, opt => opt.MapFrom(src => src.Id))
               .ForMember(o => o.DigitalGuidelineId, opt => opt.MapFrom(src => src.DigitalGuidelineId))
               .ForMember(o => o.EpisodeOfCareId, opt => opt.MapFrom(src => src.EpisodeOfCareId))
               .ForMember(o => o.Period, opt => opt.MapFrom(src => src.Period))
               .ForMember(o => o.Title, opt => opt.MapFrom(src => src.Title))
               .ForMember(o => o.NodeValues, opt => opt.MapFrom(src => src.NodeValues));

            CreateMap<GuidelineInstance, GuidelineInstanceDataOut>()
                .ForMember(o => o.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(o => o.DigitalGuidelineId, opt => opt.MapFrom(src => src.DigitalGuidelineId))
                .ForMember(o => o.EpisodeOfCareId, opt => opt.MapFrom(src => src.EpisodeOfCareId))
                .ForMember(o => o.Period, opt => opt.MapFrom(src => src.Period))
                .ForMember(o => o.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(o => o.NodeValues, opt => opt.MapFrom(src => src.NodeValues))
                .ReverseMap();
        }
    }
}