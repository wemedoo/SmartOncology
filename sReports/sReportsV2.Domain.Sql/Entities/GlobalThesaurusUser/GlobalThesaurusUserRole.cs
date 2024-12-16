using sReportsV2.Domain.Sql.EntitiesBase;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace sReportsV2.Domain.Sql.Entities.GlobalThesaurusUser
{
    public class GlobalThesaurusUserRole : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("GlobalThesaurusUserRoleId")]
        public int GlobalThesaurusUserRoleId { get; set; }
        public int GlobalThesaurusUserId { get; set; }
        [ForeignKey("GlobalThesaurusUserId")]
        public GlobalThesaurusUser GlobalThesaurusUser { get; set; }
        public int GlobalThesaurusRoleId { get; set; }
        [ForeignKey("GlobalThesaurusRoleId")]
        public GlobalThesaurusRole GlobalThesaurusRole { get; set; }
    }
}
