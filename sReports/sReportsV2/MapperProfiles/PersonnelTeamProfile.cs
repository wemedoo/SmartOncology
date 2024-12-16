using AutoMapper;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Sql.Entities.PersonnelTeamEntities;
using sReportsV2.DTOs.DTOs.PersonnelTeam.DataIn;
using sReportsV2.DTOs.DTOs.PersonnelTeam.DataOut;

namespace sReportsV2.MapperProfiles
{
    public class PersonnelTeamProfile : Profile
    {
        public PersonnelTeamProfile()
        {
            CreateMap<PersonnelTeamDataIn, PersonnelTeam>()
                .IgnoreAllNonExisting()
                .ForMember(d => d.PersonnelTeamId, opt => opt.MapFrom(src => src.PersonnelTeamId))
                .ForMember(d => d.Name, opt => opt.MapFrom(src => src.TeamName))
                .ForMember(d => d.TypeCD, opt => opt.MapFrom(src => src.TeamType))
                .ForMember(d => d.PersonnelTeamRelations, opt => opt.MapFrom(src => src.PersonnelTeamRelations))
                .ForMember(d => d.PersonnelTeamOrganizationRelations, opt => opt.MapFrom(src => src.PersonnelTeamOrganizationRelations))
                .AfterMap<CommonGlobalAfterMapping<PersonnelTeam>>();

            CreateMap<PersonnelTeam, PersonnelTeamDataOut>()
                .IgnoreAllNonExisting()
                .ForMember(d => d.PersonnelTeamId, opt => opt.MapFrom(src => src.PersonnelTeamId))
                .ForMember(d => d.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(d => d.PersonnelTeamRelations, opt => opt.MapFrom(src => src.PersonnelTeamRelations))
                .ForMember(d => d.Name, opt => opt.MapFrom(src => src.Name));

            CreateMap<PersonnelTeamFilterDataIn, PersonnelTeamFilter>()
                .ForMember(d => d.TeamType, opt => opt.MapFrom(src => src.TeamType))
                .ForMember(d => d.TeamName, opt => opt.MapFrom(src => src.TeamName))
                .ForMember(d => d.OrganizationId, opt => opt.MapFrom(src => src.OrganizationId));

            CreateMap<PersonnelTeamRelationDataIn, PersonnelTeamRelation>()
                .IgnoreAllNonExisting()
                .ForMember(d => d.PersonnelTeamId, opt => opt.MapFrom(src => src.PersonnelTeamId))
                .ForMember(d => d.PersonnelId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(d => d.PersonnelTeamRelationId, opt => opt.MapFrom(src => src.PersonnelTeamRelationId))
                .ForMember(d => d.RelationTypeCD, opt => opt.MapFrom(src => src.RelationTypeCD))
                .AfterMap<CommonGlobalAfterMapping<PersonnelTeamRelation>>(); 

            CreateMap<PersonnelTeamRelation, PersonnelTeamRelationDataOut>()
                .ForMember(d => d.PersonnelTeamRelationId, opt => opt.MapFrom(src => src.PersonnelTeamRelationId))
                .ForMember(d => d.RelationType, opt => opt.MapFrom(src => src.RelationType))
                .ForMember(d => d.PersonnelTeamId, opt => opt.MapFrom(src => src.PersonnelTeamId))
                .ForMember(d => d.UserId, opt => opt.MapFrom(src => src.PersonnelId))
                .ForMember(d => d.User, opt => opt.MapFrom(src => src.Personnel));

            CreateMap<PersonnelTeamRelationFilterDataIn, PersonnelTeamRelationFilter>()
                .ForMember(d => d.PersonnelTeamId, opt => opt.MapFrom(src => src.PersonnelTeamId))
                .ForMember(d => d.PersonnelId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(d => d.RelationTypeCD, opt => opt.MapFrom(src => src.RelationTypeCD));

            CreateMap<PersonnelTeamOrganizationRelationDataIn, PersonnelTeamOrganizationRelation>()
                .IgnoreAllNonExisting()
                .ForMember(d => d.OrganizationId, opt => opt.MapFrom(src => src.OrganizationId))
                .AfterMap<CommonGlobalAfterMapping<PersonnelTeamOrganizationRelation>>();

            CreateMap<PersonnelTeamOrganizationRelation, PersonnelTeamOrganizationRelationDataOut>()
                .ForMember(d => d.PersonnelTeamOrganizationRelationId, opt => opt.MapFrom(src => src.PersonnelTeamOrganizationRelationId))
                .ForMember(d => d.PersonnelTeamId, opt => opt.MapFrom(src => src.PersonnelTeamId))
                .ForMember(d => d.RelationTypeCD, opt => opt.MapFrom(src => src.RelationTypeCD));
        }
    }
}