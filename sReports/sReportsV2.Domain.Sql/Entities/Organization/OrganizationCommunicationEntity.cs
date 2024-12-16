using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Domain.Sql.EntitiesBase;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sReportsV2.Domain.Sql.Entities.OrganizationEntities
{
    public class OrganizationCommunicationEntity : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("OrgCommunicationEntityId")]
        public int OrgCommunicationEntityId { get; set; }
        public string DisplayName { get; set; }
        [ForeignKey("OrgCommunicationEntityCD")]
        public Code OrgCommunicationEntity { get; set; }
        public int? OrgCommunicationEntityCD { get; set; }
        [ForeignKey("PrimaryCommunicationSystemCD")]
        public Code PrimaryCommunicationSystem { get; set; }
        public int? PrimaryCommunicationSystemCD { get; set; }
        [ForeignKey("SecondaryCommunicationSystemCD")]
        public Code SecondaryCommunicationSystem { get; set; }
        public int? SecondaryCommunicationSystemCD { get; set; }
        [ForeignKey("OrganizationId")]
        public Organization Organization { get; set; }
        public int OrganizationId { get; set; }

        public void Copy(OrganizationCommunicationEntity organizationCommunicationEntity)
        {
            this.DisplayName = organizationCommunicationEntity.DisplayName;
            this.OrgCommunicationEntityCD = organizationCommunicationEntity.OrgCommunicationEntityCD;
            this.PrimaryCommunicationSystemCD = organizationCommunicationEntity.PrimaryCommunicationSystemCD;
            this.SecondaryCommunicationSystemCD = organizationCommunicationEntity.SecondaryCommunicationSystemCD;
            this.OrganizationId = organizationCommunicationEntity.OrganizationId;
            this.ActiveFrom = organizationCommunicationEntity.ActiveFrom;
            this.ActiveTo = organizationCommunicationEntity.ActiveTo;
        }
    }
}
