using sReportsV2.Domain.Sql.Entities.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sReportsV2.Domain.Sql.Entities.OutsideUser
{
    [Table("OutsideUserAddresses")]
    public class OutsideUserAddress : AddressBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int OutsideUserAddressId { get; set; }

        public OutsideUserAddress() { }
        public OutsideUserAddress(int? createdById) : base(createdById) { }
    }
}
