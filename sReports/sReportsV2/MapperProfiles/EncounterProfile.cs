using AutoMapper;
using sReportsV2.DTOs.Encounter;
using System.Linq;
using sReportsV2.DTOs.ThesaurusEntry.DataOut;
using sReportsV2.Domain.Sql.Entities.Encounter;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.DTOs.Encounter.DataOut;
using sReportsV2.DTOs.DTOs.Encounter.DataIn;
using sReportsV2.Common.Extensions;

namespace sReportsV2.MapperProfiles
{
    public class EncounterProfile : Profile
    {
        public EncounterProfile()
        {
            CreateMap<int, ThesaurusEntryDataOut>()
                .IgnoreAllNonExisting()
                .ForMember(o => o.Id, opt => opt.MapFrom(src => src));

            CreateMap<EncounterDataIn, Encounter>()
                .IgnoreAllNonExisting()
                .ForMember(o => o.StatusCD, opt => opt.MapFrom(src => src.StatusCD))
                .ForMember(o => o.TypeCD, opt => opt.MapFrom(src => src.TypeCD))
                .ForMember(o => o.ServiceTypeCD, opt => opt.MapFrom(src => src.ServiceTypeCD))
                .ForMember(o => o.ClassCD, opt => opt.MapFrom(src => src.ClassCD))
                .ForMember(o => o.EpisodeOfCareId, opt => opt.MapFrom(src => src.EpisodeOfCareId))
                .ForMember(o => o.EncounterId, opt => opt.MapFrom(src => src.Id))
                .ForMember(o => o.LastUpdate, opt => opt.MapFrom(src => src.LastUpdate))
                .ForMember(o => o.PatientId, opt => opt.MapFrom(src => src.PatientId))
                .ForMember(o => o.AdmissionDate, opt => opt.MapFrom(src => src.Period.StartDate))
                .ForMember(o => o.DischargeDate, opt => opt.MapFrom(src => src.Period.EndDate))
                .ForMember(o => o.PersonnelEncounterRelations, opt => opt.MapFrom(src => src.Doctors))
                .AfterMap<CommonGlobalAfterMapping<Encounter>>();

            CreateMap<Encounter, EncounterDataOut>()
                .IgnoreAllNonExisting()
                .ForMember(o => o.StatusId, opt => opt.MapFrom(src => src.StatusCD))
                .ForMember(o => o.TypeId, opt => opt.MapFrom(src => src.TypeCD))
                .ForMember(o => o.ServiceTypeId, opt => opt.MapFrom(src => src.ServiceTypeCD))
                .ForMember(o => o.ClassId, opt => opt.MapFrom(src => src.ClassCD))
                .ForMember(o => o.EpisodeOfCareId, opt => opt.MapFrom(src => src.EpisodeOfCareId))
                .ForMember(o => o.Id, opt => opt.MapFrom(src => src.EncounterId))
                .ForMember(o => o.LastUpdate, opt => opt.MapFrom(src => src.LastUpdate))
                .ForMember(o => o.EntryDatetime, opt => opt.MapFrom(src => src.EntryDatetime))
                .ForMember(o => o.Period, opt => opt.MapFrom(src => src))
                .ForMember(o => o.PatientId, opt => opt.MapFrom(src => src.PatientId))
                .ForMember(o => o.DischargeDatetime, opt => opt.MapFrom(src => src.DischargeDate))
                .ForMember(o => o.AdmitDatetime, opt => opt.MapFrom(src => src.AdmissionDate))
                .ForMember(o => o.Tasks, opt => opt.MapFrom(src => src.Tasks.Where(x => !x.IsDeleted())))
                .ForMember(o => o.Doctors, opt => opt.MapFrom(src => src.PersonnelEncounterRelations.Where(x => !x.IsDeleted())));

            CreateMap<Encounter, PeriodOffsetDTO>()
                .ForMember(o => o.StartDate, opt => opt.MapFrom(src => src.AdmissionDate))
                .ForMember(o => o.EndDate, opt => opt.MapFrom(src => src.DischargeDate));

            CreateMap<EncounterFilterDataIn, EncounterFilter>();
            CreateMap<EncounterView, EncounterViewDataOut>()
             .ForMember(d => d.EncounterId, opt => opt.MapFrom(src => src.EncounterId));

            CreateMap<EncounterPersonnelRelationDataIn, PersonnelEncounterRelation>()
                .IgnoreAllNonExisting()
                .ForMember(o => o.PersonnelEncounterRelationId, opt => opt.MapFrom(src => src.Id))
                .ForMember(o => o.PersonnelId, opt => opt.MapFrom(src => src.DoctorId))
                .ForMember(o => o.RelationTypeCD, opt => opt.MapFrom(src => src.RelationTypeId))
                .AfterMap<CommonGlobalAfterMapping<PersonnelEncounterRelation>>();

            CreateMap<PersonnelEncounterRelation, EncounterPersonnelRelationDataOut>()
                .ForMember(o => o.Id, opt => opt.MapFrom(src => src.PersonnelEncounterRelationId))
                .ForMember(o => o.RelationTypeId, opt => opt.MapFrom(src => src.RelationTypeCD))
                .ForMember(o => o.DoctorId, opt => opt.MapFrom(src => src.PersonnelId))
                .ForMember(o => o.Doctor, opt => opt.MapFrom(src => src.Personnel));
        }
    }
}