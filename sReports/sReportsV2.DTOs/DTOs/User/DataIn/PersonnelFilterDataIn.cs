using System;

namespace sReportsV2.DTOs.DTOs.User.DataIn
{
    public class PersonnelFilterDataIn : sReportsV2.DTOs.Common.DataIn
    {
        public bool ShowUnassignedUsers { get; set; }

        public DateTime? BirthDate { get; set; }
        public int? IdentifierType { get; set; }
        public string IdentifierValue { get; set; }
        public string BusinessEmail { get; set; }
        public string Username { get; set; }
        public string Family { get; set; }
        public string Given { get; set; }
        public int? OrganizationId { get; set; }
        public int? RoleCD { get; set; }
        public int? PersonnelTypeCD { get; set; }
        public int ActiveOrganization { get; set; }

        public int? CountryCD { get; set; }
        public string CountryName { get; set; }
        public string PostalCode { get; set; }
        public string Street { get; set; }
        public string City { get; set; }

    }
}
