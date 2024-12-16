using sReportsV2.Domain.Sql.Entities.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sReportsV2.Domain.Sql.Entities.OrganizationEntities
{
    [Table("OrganizationAddresses")]
    public class OrganizationAddress : AddressBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int OrganizationAddressId { get; set; }

        public OrganizationAddress() { }
        public OrganizationAddress(int? createdById) : base(createdById) { }
    }
}
