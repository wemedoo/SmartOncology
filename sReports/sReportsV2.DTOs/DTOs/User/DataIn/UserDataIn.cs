using sReportsV2.DTOs.DTOs.TrialManagement;
using sReportsV2.DTOs.DTOs.User.DataIn;
using sReportsV2.DTOs.DTOs.User.DTO;
using System;
using System.Collections.Generic;

namespace sReportsV2.DTOs.User.DataIn
{
    public class UserDataIn
    {
        public int Id { get; set; }
        public DateTimeOffset? LastUpdate { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PersonalEmail { get; set; }
        public string ContactPhone { get; set; }
        public List<int> Roles { get; set; } = new List<int>();
        public int? PrefixCD { get; set; }
        public int? PersonnelTypeCD { get; set; }
        public string MiddleName { get; set; }
        public List<PersonnelAddressDataIn> Addresses { get; set; }
        public DateTime? DayOfBirth { get; set; }
        public List<UserOrganizationDataIn> UserOrganizations { get; set; }
        public List<ClinicalTrialDataIn> ClinicalTrials { get; set; }
        public List<PersonnelIdentifierDataIn> Identifiers { get; set; } = new List<PersonnelIdentifierDataIn>();
        public List<UserAcademicPositionDTO> AcademicPositions { get; set; } = new List<UserAcademicPositionDTO>();
        public PersonnelOccupationDataIn PersonnelOccupation { get; set; } = new PersonnelOccupationDataIn();
    }
}