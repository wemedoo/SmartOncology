using AutoMapper;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Sql.Entities.Base;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Domain.Sql.Entities.OrganizationEntities;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Common.DataOut;
using sReportsV2.DTOs.DTOs.Organization.DataIn;
using sReportsV2.DTOs.DTOs.Organization.DataOut;
using sReportsV2.DTOs.Organization;
using sReportsV2.DTOs.Organization.DataIn;
using sReportsV2.DTOs.Organization.DataOut;
using sReportsV2.SqlDomain.Filter;
using System;
using System.Linq;

namespace sReportsV2.MapperProfiles
{
    public class OrganizationProfile : Profile
    {
        public OrganizationProfile()
        {
            CreateMap<OrganizationDataIn, Organization>()
                .IgnoreAllNonExisting()
                .ForMember(d => d.OrganizationId, opt => opt.MapFrom(src => src.Id))
                .ForMember(d => d.OrganizationAddressId, opt => opt.MapFrom(src => src.AddressId))
                .ForMember(d => d.OrganizationAddress, opt => opt.MapFrom(src => src.Address))
                .ForMember(d => d.Alias, opt => opt.MapFrom(src => src.Alias))
                .ForMember(d => d.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(d => d.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(d => d.OrganizationId, opt => opt.MapFrom(src => src.Id))
                .ForMember(d => d.OrganizationIdentifiers, opt => opt.MapFrom(src => src.Identifiers))
                .ForMember(d => d.LogoUrl, opt => opt.MapFrom(src => src.LogoUrl))
                .ForMember(d => d.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(d => d.PrimaryColor, opt => opt.MapFrom(src => src.PrimaryColor))
                .ForMember(d => d.RowVersion, opt => opt.MapFrom(src => Convert.FromBase64String(src.RowVersion)))
                .ForMember(d => d.SecondaryColor, opt => opt.MapFrom(src => src.SecondaryColor))
                .ForMember(d => d.Telecoms, opt => opt.MapFrom(src => src.Telecom))
                .ForMember(d => d.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(d => d.Impressum, opt => opt.MapFrom(src => src.Impressum))
                .ForMember(d => d.PersonnelTeamOrganizationRelations, opt => opt.MapFrom(src => src.PersonnelTeams))
                .ForMember(d => d.OrganizationCommunicationEntities, opt => opt.MapFrom(src => src.OrganizationCommunicationEntities))
                .ForMember(d => d.TimeZone, opt => opt.MapFrom(src => src.TimeZone))
                .ForMember(d => d.TimeZoneOffset, opt => opt.MapFrom(src => src.TimeZoneOffset))
                .ForMember(d => d.ClinicalDomains, opt => opt.MapFrom(src => src.ClinicalDomains))
                .AfterMap<CommonGlobalAfterMapping<Organization>>();

            CreateMap<OrganizationRelationDataIn, OrganizationRelation>()
                .IgnoreAllNonExisting()
                .AfterMap<CommonGlobalAfterMapping<OrganizationRelation>>();

            CreateMap<OrganizationClinicalDomainDataIn, OrganizationClinicalDomain>()
                .IgnoreAllNonExisting()
                .AfterMap<CommonGlobalAfterMapping<OrganizationClinicalDomain>>();

            CreateMap<Organization, OrganizationDataOut>()
                .ForMember(d => d.Address, opt => opt.MapFrom(src => src.OrganizationAddress))
                .ForMember(d => d.Alias, opt => opt.MapFrom(src => src.Alias))
                .ForMember(d => d.ClinicalDomains, opt => opt.MapFrom(src => src.ClinicalDomains.Where(x => !x.IsDeleted())))
                .ForMember(d => d.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(d => d.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.OrganizationId))
                .ForMember(o => o.Identifiers, opt => opt.MapFrom(src => src.OrganizationIdentifiers.Where(c => !c.IsDeleted())))
                .ForMember(d => d.LastUpdate, opt => opt.MapFrom(src => src.LastUpdate))
                .ForMember(d => d.LogoUrl, opt => opt.MapFrom(src => src.LogoUrl))
                .ForMember(d => d.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(d => d.PrimaryColor, opt => opt.MapFrom(src => src.PrimaryColor))
                .ForMember(d => d.RowVersion, opt => opt.MapFrom(src => Convert.ToBase64String(src.RowVersion)))
                .ForMember(d => d.SecondaryColor, opt => opt.MapFrom(src => src.SecondaryColor))
                .ForMember(d => d.Telecoms, opt => opt.MapFrom(src => src.Telecoms.Where(c => !c.IsDeleted())))
                .ForMember(d => d.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(d => d.Parent, opt => opt.MapFrom(src => src.OrganizationRelation.Parent))
                .ForMember(d => d.Impressum, opt => opt.MapFrom(src => src.Impressum))
                .ForMember(d => d.PersonnelTeamOrganizationRelations, opt => opt.MapFrom(src => src.PersonnelTeamOrganizationRelations))
                .ForMember(d => d.OrganizationCommunicationEntities, opt => opt.MapFrom(src => src.OrganizationCommunicationEntities.Where(x => !x.IsDeleted())))
                .ForMember(d => d.TimeZone, opt => opt.MapFrom(src => src.TimeZone))
                .ForMember(d => d.TimeZoneOffset, opt => opt.MapFrom(src => src.TimeZoneOffset));

            CreateMap<OrganizationClinicalDomain, OrganizationClinicalDomainDataOut>()
                .IgnoreAllNonExisting()
                .ForMember(d => d.OrganizationClinicalDomainId, opt => opt.MapFrom(src => src.OrganizationClinicalDomainId))
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.ClinicalDomainCD));

            CreateMap<OrganizationFilterDataIn, OrganizationFilter>();

            CreateMap<OrganizationUsersCountDataOut, OrganizationUsersCount>()
                .IgnoreAllNonExisting()
                .ReverseMap();

            CreateMap<OrganizationTelecomDataIn, OrganizationTelecom>()
                .IgnoreAllNonExisting()
                .IncludeBase<TelecomDTO, TelecomBase>()
                .ForMember(o => o.OrganizationTelecomId, opt => opt.MapFrom(src => src.Id))
                .AfterMap<CommonGlobalAfterMapping<OrganizationTelecom>>();

            CreateMap<OrganizationTelecom, TelecomDTO>()
                .IncludeBase<TelecomBase, TelecomDTO>()
                .ForMember(o => o.Id, opt => opt.MapFrom(src => src.OrganizationTelecomId));

            CreateMap<OrganizationIdentifierDataIn, OrganizationIdentifier>()
                .IgnoreAllNonExisting()
                .IncludeBase<IdentifierDataIn, IdentifierBase>()
                .ForMember(o => o.OrganizationIdentifierId, opt => opt.MapFrom(src => src.Id))
                .AfterMap<CommonGlobalAfterMapping<OrganizationIdentifier>>();

            CreateMap<OrganizationIdentifier, IdentifierDataOut>()
                .ForMember(o => o.Id, opt => opt.MapFrom(src => src.OrganizationIdentifierId))
                .IncludeBase<IdentifierBase, IdentifierDataOut>();

            CreateMap<OrganizationCommunicationEntityDataIn, OrganizationCommunicationEntity>()
                .IgnoreAllNonExisting()
                .ForMember(o => o.OrgCommunicationEntityId, opt => opt.MapFrom(src => src.OrgCommunicationEntityId))
                .ForMember(o => o.DisplayName, opt => opt.MapFrom(src => src.DisplayName))
                .ForMember(o => o.OrgCommunicationEntityCD, opt => opt.MapFrom(src => src.OrgCommunicationEntityCD))
                .ForMember(o => o.PrimaryCommunicationSystemCD, opt => opt.MapFrom(src => src.PrimaryCommunicationSystemCD))
                .ForMember(o => o.SecondaryCommunicationSystemCD, opt => opt.MapFrom(src => src.SecondaryCommunicationSystemCD))
                .ForMember(o => o.OrganizationId, opt => opt.MapFrom(src => src.OrganizationId))
                .ForMember(o => o.ActiveFrom, opt => opt.MapFrom(src => src.ActiveFrom))
                .ForMember(o => o.ActiveTo, opt => opt.MapFrom(src => src.ActiveTo))
                .AfterMap<CommonGlobalAfterMapping<OrganizationCommunicationEntity>>();

            CreateMap<OrganizationCommunicationEntity, OrganizationCommunicationEntityDataOut>()
                 .ForMember(o => o.OrgCommunicationEntityId, opt => opt.MapFrom(src => src.OrgCommunicationEntityId))
                 .ForMember(o => o.DisplayName, opt => opt.MapFrom(src => src.DisplayName))
                 .ForMember(o => o.OrgCommunicationEntityCD, opt => opt.MapFrom(src => src.OrgCommunicationEntityCD))
                 .ForMember(o => o.PrimaryCommunicationSystemCD, opt => opt.MapFrom(src => src.PrimaryCommunicationSystemCD))
                 .ForMember(o => o.SecondaryCommunicationSystemCD, opt => opt.MapFrom(src => src.SecondaryCommunicationSystemCD))
                 .ForMember(o => o.OrganizationId, opt => opt.MapFrom(src => src.OrganizationId))
                 .ForMember(o => o.ActiveFrom, opt => opt.MapFrom(src => src.ActiveFrom))
                 .ForMember(o => o.ActiveTo, opt => opt.MapFrom(src => src.ActiveTo));

            CreateMap<OrganizationAddress, AddressDTO>()
              .ForMember(o => o.Id, opt => opt.MapFrom(src => src.OrganizationAddressId))
              .IncludeBase<AddressBase, AddressDTO>();

            CreateMap<AddressDTO, OrganizationAddress>()
                .IncludeBase<AddressDTO, AddressBase>()
                .ForMember(o => o.OrganizationAddressId, opt => opt.MapFrom(src => src.Id))
                .AfterMap<CommonGlobalAfterMapping<OrganizationAddress>>();
        }
    }
}