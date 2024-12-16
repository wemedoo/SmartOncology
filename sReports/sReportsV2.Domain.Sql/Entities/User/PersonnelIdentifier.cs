using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sReportsV2.Domain.Sql.Entities.User
{
    public class PersonnelIdentifier : Base.IdentifierBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int PersonnelIdentifierId { get; set; }

        public int? PersonnelId { get; set; }
        [ForeignKey("PersonnelId")]
        public Personnel Personnel { get; set; }

        public PersonnelIdentifier()
        {
        }

        public PersonnelIdentifier(int? identifierTypeCD, string value, int? identifierUseCD = null) : base(identifierTypeCD, value, identifierUseCD)
        {
        }
    }
}
