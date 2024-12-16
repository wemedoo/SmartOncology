using sReportsV2.Domain.Sql.Entities.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sReportsV2.Domain.Sql.Entities.User
{
    [Table("PersonnelAcademicPositions")]
    public class PersonnelAcademicPosition : EntitiesBase.Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int PersonnelAcademicPositionId { get; set; }
        public int PersonnelId { get; set; }
        [ForeignKey("PersonnelId")]
        public Personnel Personnel { get; set; }
        public int? AcademicPositionCD { get; set; }
        [ForeignKey("AcademicPositionCD")]
        public virtual Code AcademicPosition { get; set; }
        public int? AcademicPositionTypeCD { get; set; }
        [ForeignKey("AcademicPositionTypeCD")]
        public virtual Code AcademicPositionType { get; set; }
    }
}
