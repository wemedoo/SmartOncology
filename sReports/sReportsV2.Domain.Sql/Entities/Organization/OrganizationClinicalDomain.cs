using sReportsV2.Domain.Sql.EntitiesBase;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using sReportsV2.Domain.Sql.Entities.Common;

namespace sReportsV2.Domain.Sql.Entities.OrganizationEntities
{
    public class OrganizationClinicalDomain : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("OrganizationClinicalDomainId")]
        public int OrganizationClinicalDomainId { get; set; }
        public int OrganizationId { get; set; }
        [ForeignKey("OrganizationId")]
        public Organization Organization { get; set; }
        public int? ClinicalDomainCD { get; set; }
        [ForeignKey("ClinicalDomainCD")]
        public Code ClinicalDomainCode { get; set; }
    }
}
