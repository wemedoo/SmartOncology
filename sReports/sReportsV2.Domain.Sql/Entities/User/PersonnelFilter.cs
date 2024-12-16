using sReportsV2.Common.Entities;
using System;

namespace sReportsV2.Domain.Sql.Entities.User
{
    public class PersonnelFilter : EntityFilter
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
        public string City { get; set; }
        public int? CountryCD { get; set; }
        public string PostalCode { get; set; }
        public string Street { get; set; }
    }
}
