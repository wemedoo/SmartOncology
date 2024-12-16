using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sReportsV2.Domain.Sql.Entities.OutsideUser
{
    public class OutsideUser : EntitiesBase.Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("OutsideUserId")]
        public int OutsideUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Institution { get; set; }
        public string InstitutionAddress { get; set; }
        public int? OutsideUserAddressId { get; set; }
        [ForeignKey("OutsideUserAddressId")]
        public virtual OutsideUserAddress OutsideUserAddress { get; set; }

        public void Copy(OutsideUser copyFrom)
        {
            if (copyFrom != null)
            {
                this.FirstName = copyFrom.FirstName;
                this.LastName = copyFrom.LastName;
                this.Email = copyFrom.Email;
                this.Institution = copyFrom.Institution;
                this.InstitutionAddress = copyFrom.InstitutionAddress;
                this.CopyAddress(copyFrom.OutsideUserAddress);
                this.SetLastUpdate();
            }
        }


        private void CopyAddress(OutsideUserAddress copyFrom)
        {
            if (copyFrom == null)
            {
                this.OutsideUserAddress = null;
                return;
            }

            if (this.OutsideUserAddress == null)
            {
                this.OutsideUserAddress = new OutsideUserAddress(copyFrom.CreatedById);
            }

            this.OutsideUserAddress.Copy(copyFrom);
        }
    }
}
