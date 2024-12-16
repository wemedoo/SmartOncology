using sReportsV2.Domain.Sql.Entities.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sReportsV2.Domain.Sql.Entities.User
{
    [Table("PersonnelPositions")]
    public class PersonnelPosition : EntitiesBase.Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int PersonnelPositionId { get; set; }

        public int? PositionCD { get; set; }
        [ForeignKey("PositionCD")]
        public virtual Code Position { get; set; }

        public int? PersonnelId { get; set; }
        [ForeignKey("PersonnelId")]
        public virtual Personnel Personnel { get; set; }
    }
}
