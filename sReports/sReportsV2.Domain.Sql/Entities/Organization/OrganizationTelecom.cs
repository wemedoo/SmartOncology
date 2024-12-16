using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using sReportsV2.Domain.Sql.Entities.Common;

namespace sReportsV2.Domain.Sql.Entities.OrganizationEntities
{
    public class OrganizationTelecom : TelecomBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int OrganizationTelecomId { get; set; }
        public int? OrganizationId { get; set; }
        [ForeignKey("OrganizationId")]
        public Organization Organization { get; set; }
    }
}
