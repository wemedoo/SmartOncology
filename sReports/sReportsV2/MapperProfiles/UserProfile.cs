using AutoMapper;
using sReportsV2.Common.Entities.User;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Sql.Entities.Base;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Domain.Sql.Entities.User;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Common.DataOut;
using sReportsV2.DTOs.DTOs.AccessManagment.DataOut;
using sReportsV2.DTOs.DTOs.User.DataIn;
using sReportsV2.DTOs.DTOs.User.DataOut;
using sReportsV2.DTOs.DTOs.User.DTO;
using sReportsV2.DTOs.Organization;
using sReportsV2.DTOs.User.DataIn;
using sReportsV2.DTOs.User.DataOut;
using sReportsV2.DTOs.User.DTO;
using System;
using System.Linq;

namespace sReportsV2.MapperProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<Personnel, UserCookieData>()
                .IgnoreAllNonExisting()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.PersonnelId))
                .ForMember(d => d.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(d => d.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(d => d.Username, opt => opt.MapFrom(src => src.Username))
                .ForMember(d => d.LogoUrl, opt => opt.MapFrom(src => src.PersonnelConfig.ActiveOrganization.LogoUrl))
                .ForMember(d => d.ActiveLanguage, opt => opt.MapFrom(src => src.PersonnelConfig.ActiveLanguage))
                .ForMember(d => d.ActiveOrganization, opt => opt.MapFrom(src => src.PersonnelConfig.ActiveOrganizationId))
                .ForMember(d => d.PageSize, opt => opt.MapFrom(src => src.PersonnelConfig.PageSize))
                .ForMember(d => d.Organizations, opt => opt.MapFrom(o => o.GetOrganizations().Select(x => x.Organization)))
                .ForMember(d => d.TimeZoneOffset, opt => opt.MapFrom(o => o.PersonnelConfig.TimeZoneOffset))
                .ForMember(d => d.SuggestedForms, opt => opt.MapFrom(o => o.PersonnelConfig.GetForms()))
                .ForMember(d => d.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(d => d.OrganizationTimeZone, opt => opt.MapFrom(src => MapOrganizationTimeZoneOffset(src.PersonnelConfig.ActiveOrganization.TimeZone)));

            CreateMap<PersonnelPositionPermissionView, PositionPermissionDataOut>()
                .IgnoreAllNonExisting()
                .ForMember(d => d.PositionId, opt => opt.MapFrom(src => src.PositionCD))
                .ForMember(d => d.ModuleName, opt => opt.MapFrom(src => src.ModuleName))
                .ForMember(d => d.PermissionName, opt => opt.MapFrom(src => src.PermissionName));

            CreateMap<PersonnelOrganization, UserOrganizationDataOut>()
                .IgnoreAllNonExisting()
                .ForMember(d => d.IsPracticioner, opt => opt.MapFrom(src => src.IsPracticioner))
                .ForMember(d => d.Organization, opt => opt.MapFrom(src => src.Organization))
                .ForMember(d => d.Qualification, opt => opt.MapFrom(src => src.Qualification))
                .ForMember(d => d.SeniorityLevel, opt => opt.MapFrom(src => src.SeniorityLevel))
                .ForMember(d => d.Speciality, opt => opt.MapFrom(src => src.Speciality))
                .ForMember(d => d.StateCD, opt => opt.MapFrom(src => src.StateCD))
                .ForMember(d => d.SubSpeciality, opt => opt.MapFrom(src => src.SubSpeciality))
                .ForMember(d => d.OrganizationId, opt => opt.MapFrom(src => src.OrganizationId));

            CreateMap<UserOrganizationDataIn, PersonnelOrganization>()
                .IgnoreAllNonExisting();

            CreateMap<UserCookieData, UserData>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(d => d.ActiveOrganization, opt => opt.MapFrom(src => src.ActiveOrganization))
                .ForMember(d => d.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(d => d.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(d => d.Organizations, opt => opt.MapFrom(src => src.Organizations.Select(x => x.Id).ToList()))
                .ForMember(d => d.Username, opt => opt.MapFrom(src => src.Username));

            CreateMap<UserShortInfoDataOut, Personnel>()
                .IgnoreAllNonExisting()
                .ReverseMap();

            CreateMap<UserData, UserDataOut>()
                .IgnoreAllNonExisting()
                .ForMember(dest => dest.Organizations, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<UserDataIn, Personnel>()
                .IgnoreAllNonExisting()
                .ForMember(d => d.PersonnelId, opt => opt.MapFrom(src => src.Id))
                .ForMember(d => d.PersonnelPositions, opt => opt.Ignore())
                .ForMember(d => d.Organizations, opt => opt.MapFrom(src => src.UserOrganizations))
                .ForMember(d => d.PrefixCD, opt => opt.MapFrom(src => src.PrefixCD))
                .ForMember(d => d.PersonnelTypeCD, opt => opt.MapFrom(src => src.PersonnelTypeCD))
                .ForMember(d => d.PersonnelIdentifiers, opt => opt.MapFrom(src => src.Identifiers))
                .ForMember(d => d.PersonnelAcademicPositions, opt => opt.MapFrom(src => src.AcademicPositions))
                .ForMember(d => d.PersonnelAdresses, opt => opt.MapFrom(src => src.Addresses))
                .ForMember(d => d.Username, opt => opt.MapFrom(src => src.Username.TrimInput()))
                .ForMember(d => d.PersonnelOccupation, opt => opt.MapFrom(src => src.PersonnelOccupation))
                .AfterMap<CommonGlobalAfterMapping<Personnel>>();

            CreateMap<UserCookieData, UserDataOut>()
                .IgnoreAllNonExisting()
                .ForMember(d => d.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(d => d.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(d => d.Username, opt => opt.MapFrom(src => src.Username))
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(d => d.Email, opt => opt.MapFrom(src => src.Email));

            CreateMap<Personnel, UserDataOut>()
                .IgnoreAllNonExisting()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.PersonnelId))
                .ForMember(d => d.LastUpdate, opt => opt.MapFrom(src => src.LastUpdate))
                .ForMember(d => d.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(d => d.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(d => d.Username, opt => opt.MapFrom(src => src.Username))
                .ForMember(d => d.ContactPhone, opt => opt.MapFrom(src => src.ContactPhone))
                .ForMember(d => d.PersonnelPossitions, opt => opt.MapFrom(src => src.PersonnelPositions
                    .Where(x => !x.IsDeleted()).Select(x => x.PositionCD.GetValueOrDefault())))
                .ForMember(d => d.PrefixId, opt => opt.MapFrom(src => src.PrefixCD))
                .ForMember(d => d.PersonelTypeId, opt => opt.MapFrom(src => src.PersonnelTypeCD))
                .ForMember(d => d.AcademicPositions, opt => opt.MapFrom(src => src.PersonnelAcademicPositions.Where(x => !x.IsDeleted())))
                .ForMember(d => d.PersonnelOccupation, opt => opt.MapFrom(src => src.PersonnelOccupation))
                .ForMember(d => d.MiddleName, opt => opt.MapFrom(src => src.MiddleName))
                .ForMember(d => d.DayOfBirth, opt => opt.MapFrom(src => src.DayOfBirth))
                .ForMember(d => d.Organizations, opt => opt.MapFrom(src => src.GetOrganizations()))
                .ForMember(d => d.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(d => d.PersonalEmail, opt => opt.MapFrom(src => src.PersonalEmail))
                .ForMember(d => d.Addresses, opt => opt.MapFrom(src => src.PersonnelAdresses.Where(x => !x.IsDeleted())))
                .ForMember(d => d.Identifiers, opt => opt.MapFrom(src => src.PersonnelIdentifiers.Where(x => !x.IsDeleted())))
                .ForMember(d => d.Addresses, opt => opt.MapFrom(src => src.PersonnelAdresses.Where(x => !x.IsDeleted())))
                .ForMember(d => d.IsDoctor, opt => opt.MapFrom(src => src.IsDoctor))
                .ForMember(d => d.PersonnelTeams, opt => opt.MapFrom(src => src.PersonnelTeams
                    .Where(pt => !pt.IsDeleted() && pt.PersonnelTeam != null && !pt.PersonnelTeam.IsDeleted())
                    .Select(pt => pt.PersonnelTeam).ToList()));

            CreateMap<string, OrganizationDataOut>()
                .IgnoreAllNonExisting()
                .ForMember(o => o.Id, opt => opt.MapFrom(src => src));

            CreateMap<Personnel, UserData>()
                .ForMember(dest => dest.Organizations, opt => opt.Ignore())
                .IgnoreAllNonExisting()
                .ForMember(u => u.Id, opt => opt.MapFrom(src => src.PersonnelId))
                .ForMember(u => u.Username, opt => opt.MapFrom(src => src.Username))
                .ForMember(u => u.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(u => u.LastName, opt => opt.MapFrom(src => src.LastName));

            CreateMap<PersonnelFilterDataIn, PersonnelFilter>();

            CreateMap<PersonnelView, UserViewDataOut>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.PersonnelId))
                .ForMember(d => d.StateCD, opt => opt.MapFrom(src => src.StateCD))
                .ForMember(d => d.PersonnelOrganizations, opt => opt.MapFrom(src => src.PersonnelOrganizations))
                .ForMember(d => d.PersonnelPositions, opt => opt.MapFrom(src => src.PersonnelPositions));

            CreateMap<PersonnelIdentifierDataIn, PersonnelIdentifier>()
                .IgnoreAllNonExisting()
                .IncludeBase<IdentifierDataIn, IdentifierBase>()
                .ForMember(o => o.PersonnelIdentifierId, opt => opt.MapFrom(src => src.Id))
                .AfterMap<CommonGlobalAfterMapping<PersonnelIdentifier>>();

            CreateMap<PersonnelIdentifier, IdentifierDataOut>()
                .ForMember(o => o.Id, opt => opt.MapFrom(src => src.PersonnelIdentifierId))
                .IncludeBase<IdentifierBase, IdentifierDataOut>();

            CreateMap<PersonnelAddress, AddressDTO>()
                .ForMember(o => o.Id, opt => opt.MapFrom(src => src.PersonnelAddressId))
                .IncludeBase<AddressBase, AddressDTO>();

            CreateMap<PersonnelAddressDataIn, PersonnelAddress>()
                .IgnoreAllNonExisting()
                .IncludeBase<AddressDTO, AddressBase>()
                .ForMember(o => o.PersonnelAddressId, opt => opt.MapFrom(src => src.Id))
                .AfterMap<CommonGlobalAfterMapping<PersonnelAddress>>();

            CreateMap<UserAcademicPositionDTO, PersonnelAcademicPosition>()
                .IgnoreAllNonExisting()
                .ForMember(o => o.AcademicPositionCD, opt => opt.MapFrom(src => src.AcademicPositionId))
                .ForMember(o => o.PersonnelAcademicPositionId, opt => opt.MapFrom(src => src.Id))
                .AfterMap<CommonGlobalAfterMapping<PersonnelAcademicPosition>>();

            CreateMap<PersonnelAcademicPosition, UserAcademicPositionDTO>()
                .ForMember(o => o.AcademicPositionId, opt => opt.MapFrom(src => src.AcademicPositionCD))
                .ForMember(o => o.Id, opt => opt.MapFrom(src => src.PersonnelAcademicPositionId));

            CreateMap<PersonnelOccupationDataIn, PersonnelOccupation>()
                .IgnoreAllNonExisting()
               .ForMember(o => o.PersonnelId, opt => opt.MapFrom(src => src.PersonnelId))
               .ForMember(o => o.OccupationCategoryCD, opt => opt.MapFrom(src => src.OccupationCategoryCD))
               .ForMember(o => o.OccupationSubCategoryCD, opt => opt.MapFrom(src => src.OccupationSubCategoryCD))
               .ForMember(o => o.OccupationCD, opt => opt.MapFrom(src => src.OccupationCD))
               .ForMember(o => o.PersonnelSeniorityCD, opt => opt.MapFrom(src => src.PersonnelSeniorityCD))
               .AfterMap<CommonGlobalAfterMapping<PersonnelOccupation>>();

            CreateMap<PersonnelOccupation, PersonnelOccupationDataOut>()
                 .ForMember(o => o.PersonnelId, opt => opt.MapFrom(src => src.PersonnelId))
                 .ForMember(o => o.OccupationCategoryCD, opt => opt.MapFrom(src => src.OccupationCategoryCD))
                 .ForMember(o => o.OccupationSubCategoryCD, opt => opt.MapFrom(src => src.OccupationSubCategoryCD))
                 .ForMember(o => o.OccupationCD, opt => opt.MapFrom(src => src.OccupationCD))
                 .ForMember(o => o.PersonnelSeniorityCD, opt => opt.MapFrom(src => src.PersonnelSeniorityCD))
                 .ForMember(o => o.ActiveFrom, opt => opt.MapFrom(src => src.ActiveFrom))
                 .ForMember(o => o.ActiveTo, opt => opt.MapFrom(src => src.ActiveTo));

            CreateMap<PersonnelAutocompleteDataIn, PersonnelAutocompleteFilter>()
                .IgnoreAllNonExisting()
                .ForMember(o => o.FilterByDoctors, opt => opt.MapFrom(src => src.FilterByDoctors))
                .ForMember(o => o.OrganizationId, opt => opt.MapFrom(src => src.OrganizationId))
                .ForMember(o => o.Name, opt => opt.MapFrom(src => src.Term));

            CreateMap<AutoCompleteUserData, UserDataOut>()
                .IgnoreAllNonExisting()
                .ForMember(o => o.Id, opt => opt.MapFrom(src => src.PersonnelId));
        }

        private string MapOrganizationTimeZoneOffset(string timeZoneDisplayName)
        {
            TimeZoneInfo timeZone = FindTimeZoneByDisplayName(timeZoneDisplayName);

            return timeZone.Id;
        }

        private TimeZoneInfo FindTimeZoneByDisplayName(string displayName)
        {
            foreach (TimeZoneInfo timeZone in TimeZoneInfo.GetSystemTimeZones())
            {
                if (timeZone.DisplayName == displayName)
                {
                    return timeZone;
                }
            }
            return null; // Time zone not found
        }
    }
}