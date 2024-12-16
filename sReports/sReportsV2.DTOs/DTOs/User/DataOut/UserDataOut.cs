using sReportsV2.Common.Enums;
using sReportsV2.DTOs.CodeEntry.DataOut;
using sReportsV2.DTOs.DTOs.AccessManagment.DataOut;
using sReportsV2.DTOs.DTOs.PersonnelTeam.DataOut;
using sReportsV2.DTOs.DTOs.TrialManagement;
using sReportsV2.DTOs.DTOs.User.DataOut;
using sReportsV2.DTOs.DTOs.User.DTO;
using sReportsV2.DTOs.User.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace sReportsV2.DTOs.Common.DataOut
{
    public class UserDataOut
    {
        public int Id { get; set; }
        public DateTimeOffset? LastUpdate { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public DateTime? DayOfBirth { get; set; }
        public string Email { get; set; }
        public string PersonalEmail { get; set; }
        public string ContactPhone { get; set; }
        public int? PrefixId { get; set; }
        public int? PersonelTypeId { get; set; }
        public List<AddressDTO> Addresses { get; set; } = new List<AddressDTO>();
        public byte[] RowVersion { get; set; }
        public List<RoleDataOut> Roles { get; set; } = new List<RoleDataOut>();
        public List<UserOrganizationDataOut> Organizations { get; set; } = new List<UserOrganizationDataOut> ();
        public List<ClinicalTrialDataOut> ClinicalTrials { get; set; } = new List<ClinicalTrialDataOut> ();
        public List<IdentifierDataOut> Identifiers { get; set; } = new List<IdentifierDataOut>();
        public List<UserAcademicPositionDTO> AcademicPositions { get; set; } = new List<UserAcademicPositionDTO>();
        public List<int> PersonnelPossitions { get; set; } = new List<int>();
        public bool IsDoctor { get; set; }
        public PersonnelOccupationDataOut PersonnelOccupation { get; set; }
        public List<PersonnelTeamDataOut> PersonnelTeams { get; set; } = new List<PersonnelTeamDataOut>();

        public bool HasToChooseActiveOrganization { get; set; }
        public bool HasNoAnyActiveOrganization(int? activeUserStateCD)
        {
            return !GetActiveOrganizations(activeUserStateCD).Any();
        }

        public bool IsBlockedInEveryOrganization(int? blockedUserStateCD)
        {
            return Organizations.Count > 0 && Organizations.TrueForAll(x => x.StateCD == blockedUserStateCD);
        }

        public bool IsCurrentSelectedOrganizationNotActive(int activeOrganizationId, int? activeUserStateCD)
        {
            return GetUserStateInOrganization(activeOrganizationId) != activeUserStateCD;
        }

        public List<int> GetOrganizationRefs()
        {
            return this.Organizations.Select(x => x.OrganizationId).ToList();
        }

        public override string ToString()
        {
            return $"{Username} ({FirstName} {LastName})";
        }

        public string GetorganizationListFormatted()
        {
            return string.Join(", ", Organizations.Select(x => x.Organization.Name));
        }

        public IEnumerable<UserOrganizationDataOut> GetActiveOrganizations(int? activeUserStateCD)
        {
            return Organizations.Where(x => x.StateCD == activeUserStateCD);
        }

        public IEnumerable<UserOrganizationDataOut> GetNonArchivedOrganizations(int? archivedUserStateCD)
        {
            return Organizations.Where(x => x.StateCD != archivedUserStateCD);
        }

        public bool IsUserBlocked(int activeOrganizationId, int? blockedUserStateCD)
        {
            return GetUserStateInOrganization(activeOrganizationId) == blockedUserStateCD;
        }

        public List<Claim> GetClaims() 
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, this.Username)
            };

            if (this.Email != null)
            {
                claims.Add(new Claim(ClaimTypes.Email, this.Email));
                claims.Add(new Claim("preferred_username", this.Email));
            }
            else 
            {
                claims.Add(new Claim("preferred_username", this.Username));
                claims.Add(new Claim("not_email", "true"));
            }

            return claims;
        }

        public bool IsRoleChecked(int roleId)
        {
            return PersonnelPossitions.Contains(roleId);
        }

        public IEnumerable<string> RenderUserRoleNames(string activeLanguage, List<CodeDataOut> allRoles)
        {
            return allRoles.Where(x => PersonnelPossitions.Contains(x.Id)).Select(x => x.Thesaurus.GetPreferredTermByTranslationOrDefault(activeLanguage));
        }

        private int? GetUserStateInOrganization(int organizationId)
        {
            return Organizations?.Find(x => x.OrganizationId == organizationId)?.StateCD;
        }

        public string GetPersonnelTeamNamesFormatted()
        {
            return string.Join(", ", PersonnelTeams.Select(x => x.Name));
        }

        public string GetPersonnelOccupation(string activeLanguage, List<CodeDataOut> occupations)
        {
            CodeDataOut occupationCode = occupations?.Where(x => x.Id == PersonnelOccupation?.OccupationCD)?.FirstOrDefault();
            return occupationCode != null ? occupationCode.Thesaurus.GetPreferredTermByTranslationOrDefault(activeLanguage) : string.Empty;
        }
    }
}