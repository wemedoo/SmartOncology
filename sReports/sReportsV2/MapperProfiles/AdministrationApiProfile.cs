using AutoMapper;
using sReportsV2.Domain.Sql.Entities.ApiRequest;
using sReportsV2.DTOs.AdministrationApi.DataIn;
using sReportsV2.DTOs.AdministrationApi.DataOut;

namespace sReportsV2.MapperProfiles
{
    public class AdministrationApiProfile : Profile
    {
        public AdministrationApiProfile()
        {
            CreateMap<ApiRequestLog, AdministrationApiDataOut>()
                .ForMember(d => d.ApiName, opt => opt.MapFrom(src => src.ApiName))
                .ForMember(d => d.ApiRequestLogId, opt => opt.MapFrom(src => src.ApiRequestLogId))
                .ForMember(d => d.ApiRequestDirection, opt => opt.MapFrom(src => src.ApiRequestDirection))
                .ForMember(d => d.HttpStatusCode, opt => opt.MapFrom(src => src.HttpStatusCode))
                .ForMember(d => d.RequestTimestamp, opt => opt.MapFrom(src => src.RequestTimestamp))
                .ForMember(d => d.RequestPayload, opt => opt.MapFrom(src => src.RequestPayload))
                .ForMember(d => d.ResponseTimestamp, opt => opt.MapFrom(src => src.ResponseTimestamp))
                .ForMember(d => d.ResponsePayload, opt => opt.MapFrom(src => src.ResponsePayload))
                .ForMember(d => d.RequestUriAbsolutePath, opt => opt.MapFrom(src => src.RequestUriAbsolutePath));

            CreateMap<AdministrationApiFilterDataIn, AdministrationApiFilter>()
                .ForMember(d => d.ApiName, opt => opt.MapFrom(src => src.ApiName))
                .ForMember(d => d.ApiRequestDirection, opt => opt.MapFrom(src => src.ApiRequestDirection))
                .ForMember(d => d.HttpStatusCode, opt => opt.MapFrom(src => src.HttpStatusCode))
                .ForMember(d => d.RequestTimestampFrom, opt => opt.MapFrom(src => src.RequestTimestampFrom))
                .ForMember(d => d.RequestTimestampTo, opt => opt.MapFrom(src => src.RequestTimestampTo))
                .ForMember(d => d.RequestContains, opt => opt.MapFrom(src => src.RequestContains));
        }
    }
}