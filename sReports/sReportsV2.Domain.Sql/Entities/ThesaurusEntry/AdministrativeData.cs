using sReportsV2.Common.Entities.User;
using sReportsV2.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using sReportsV2.Common.Extensions;

namespace sReportsV2.Domain.Sql.Entities.ThesaurusEntry
{
    public class AdministrativeData
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("AdministrativeDataId")]
        public int AdministrativeDataId { get; set; }
        public List<Version> VersionHistory { get; set; }
        public int ThesaurusEntryId { get; set; }
        public AdministrativeData() { }
        public AdministrativeData(UserData userData, int? stateCD, int? typeCD)
        {
            VersionHistory = new List<Version>
            {
                new Version()
                {
                    CreatedOn = DateTimeOffset.UtcNow.ConvertToOrganizationTimeZone(),
                    TypeCD = typeCD,
                    PersonnelId = userData.Id,
                    OrganizationId = userData.ActiveOrganization.GetValueOrDefault(),
                    StateCD = stateCD
                }
            };
        }

        public void UpdateVersionHistory(UserData userData, int? stateCD, int? typeCD)
        {
            VersionHistory = VersionHistory ?? new List<Version>();
            SetRevokedDateOfLastVersion();

            VersionHistory.Add(new Version()
            {
                CreatedOn = DateTimeOffset.UtcNow.ConvertToOrganizationTimeZone(),
                TypeCD = typeCD,
                PersonnelId = userData.Id,
                OrganizationId = userData.ActiveOrganization.GetValueOrDefault(),
                StateCD = stateCD
            });
        }

        private void SetRevokedDateOfLastVersion()
        {
            Version version = VersionHistory.LastOrDefault();
            if (version != null)
            {
                version.RevokedOn = DateTimeOffset.UtcNow.ConvertToOrganizationTimeZone();
            }
        }
    }
}
