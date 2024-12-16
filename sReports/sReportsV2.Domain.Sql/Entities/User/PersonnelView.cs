using sReportsV2.Common.Enums;
using sReportsV2.Domain.Sql.EntitiesBase;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sReportsV2.Domain.Sql.Entities.User
{
    public class PersonnelView : Entity
    {
        [Key]
        [Column("PersonnelId")]
        public int PersonnelId { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int? OrganizationId { get; set; }
        public int? StateCD { get; set; }
        public string PersonnelPositions { get; set; }
        public string PersonnelPositionIds { get; set; }
        public string PersonnelOrganizations { get; set; }
        public string PersonnelOrganizationIds { get; set; }
        public DateTime? DayOfBirth { get; set; }
        public int? PersonnelTypeCD { get; set; }
        public string PersonnelIdentifiers { get; set; }
        public string PersonnelAddresses { get; set; }
    }
}
