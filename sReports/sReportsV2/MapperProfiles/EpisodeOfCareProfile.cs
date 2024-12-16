using AutoMapper;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.EpisodeOfCare;
using sReportsV2.Domain.Sql.Entities.EpisodeOfCare;
using System.Linq;
using sReportsV2.Common.Enums;
using System;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Common.Extensions;

namespace sReportsV2.MapperProfiles
{
    public class EpisodeOfCareProfile : Profile
    {
        public EpisodeOfCareProfile()
        {
            CreateMap<EpisodeOfCareFilterDataIn, EpisodeOfCareFilter>()
                .IgnoreAllNonExisting();

            CreateMap<EpisodeOfCareDataIn, EpisodeOfCareFilter>()
                .IgnoreAllNonExisting()
                .ForMember(o => o.PatientId, opt => opt.MapFrom(src => src.PatientId))
                .ForMember(o => o.TypeCD, opt => opt.MapFrom(src => src.TypeCD))
                .ForMember(o => o.StatusCD, opt => opt.MapFrom(src => src.StatusCD));

            CreateMap<Domain.Sql.Entities.Common.PeriodDatetime, PeriodDTO>()
              .ForMember(o => o.StartDate, opt => opt.MapFrom(src => src.Start))
              .ForMember(o => o.EndDate, opt => opt.MapFrom(src => src.End));

            CreateMap<PeriodDTO, Domain.Sql.Entities.Common.PeriodDatetime>()
              .ForMember(o => o.Start, opt => opt.MapFrom(src => src.StartDate))
              .ForMember(o => o.End, opt => opt.MapFrom(src => src.EndDate));

            CreateMap<FormEpisodeOfCare, EpisodeOfCare>()
                .IgnoreAllNonExisting()
                .ForMember(dest => dest.Type, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(o => o.TypeCD, opt => opt.MapFrom(src => src.Type))
                .ForMember(o => o.DiagnosisCondition, opt => opt.MapFrom(src => src.DiagnosisCondition))
                .ForMember(o => o.DiagnosisRole, opt => opt.MapFrom(src => src.DiagnosisRole))
                .ForMember(o => o.DiagnosisRank, opt => opt.MapFrom(src => src.DiagnosisRank))
                .AfterMap<CommonGlobalAfterMapping<EpisodeOfCare>>();

            CreateMap<EpisodeOfCareDataIn, EpisodeOfCare>()
                  .IgnoreAllNonExisting()
                  .ForMember(o => o.EpisodeOfCareId, opt => opt.MapFrom(src => src.Id))
                  .ForMember(o => o.PatientId, opt => opt.MapFrom(src => src.PatientId))
                  .ForMember(o => o.StatusCD, opt => opt.MapFrom(src => src.StatusCD))
                  .ForMember(o => o.TypeCD, opt => opt.MapFrom(src => src.TypeCD))
                  .ForMember(o => o.DiagnosisCondition, opt => opt.MapFrom(src => src.DiagnosisCondition))
                  .ForMember(o => o.DiagnosisRole, opt => opt.MapFrom(src => src.DiagnosisRole))
                  .ForMember(o => o.DiagnosisRank, opt => opt.MapFrom(src => src.DiagnosisRank))
                  .ForMember(o => o.Period, opt => opt.MapFrom(src => src.Period))
                  .ForMember(o => o.LastUpdate, opt => opt.MapFrom(src => src.LastUpdate))
                  .ForMember(o => o.Description, opt => opt.MapFrom(src => src.Description))
                  .ForMember(o => o.PersonnelTeamId, opt => opt.MapFrom(src => src.PersonnelTeamId))
                  .AfterMap<CommonGlobalAfterMapping<EpisodeOfCare>>()
                  .ForMember(o => o.Status, opt => opt.Ignore())
                  .ForMember(o => o.Type, opt => opt.Ignore());

            CreateMap<EpisodeOfCare, EpisodeOfCareDataOut>()
                  .IgnoreAllNonExisting()
                  .ForMember(o => o.Id, opt => opt.MapFrom(src => src.EpisodeOfCareId))
                  .ForMember(o => o.PatientId, opt => opt.MapFrom(src => src.PatientId))
                  .ForMember(o => o.Status, opt => opt.MapFrom(src => src.StatusCD))
                  .ForMember(o => o.Type, opt => opt.MapFrom(src => src.TypeCD))
                  .ForMember(o => o.DiagnosisCondition, opt => opt.MapFrom(src => src.DiagnosisCondition))
                  .ForMember(o => o.DiagnosisRank, opt => opt.MapFrom(src => src.DiagnosisRank))
                  .ForMember(o => o.Period, opt => opt.MapFrom(src => src.Period))
                  .ForMember(o => o.LastUpdate, opt => opt.MapFrom(src => src.LastUpdate))
                  .ForMember(o => o.Description, opt => opt.MapFrom(src => src.Description))
                  .ForMember(o => o.PersonnelTeam, opt => opt.MapFrom(src => src.PersonnelTeam))
                  .ForMember(o => o.Encounters, opt => opt.MapFrom(src => src.Encounters.Where(x => !x.IsDeleted())));
        }
    }
}