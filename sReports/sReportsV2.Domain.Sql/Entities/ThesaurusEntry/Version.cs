using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Common.Entities.User;
using sReportsV2.Common.Extensions;

namespace sReportsV2.Domain.Sql.Entities.ThesaurusEntry
{
    public class Version
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("VersionId")]
        public int VersionId { get; set; }

        [ForeignKey("TypeCD")]
        public virtual Code TypeCode { get; set; }
        public int? TypeCD { get; set; }

        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset? RevokedOn { get; set; }

        [ForeignKey("StateCD")]
        public virtual Code StateCode { get; set; }
        public int? StateCD { get; set; }

        public AdministrativeData AdministrativeData {get;set;}
        public int AdministrativeDataId { get; set; }
        public int PersonnelId { get; set; }
        public int OrganizationId { get; set; }

        public Version()
        {
        }

        public Version(UserData userData, int? stateCD, int? typeCD)
        {
            CreatedOn = DateTimeOffset.UtcNow.ConvertToOrganizationTimeZone();
            TypeCD = typeCD;
            PersonnelId = userData.Id;
            OrganizationId = userData.ActiveOrganization.GetValueOrDefault();
            StateCD = stateCD;
        }
    }
}
