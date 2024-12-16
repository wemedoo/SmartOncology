using sReportsV2.Common.Enums;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Domain.Sql.Entities.OrganizationEntities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sReportsV2.Domain.Sql.Entities.User
{
    public class PersonnelOccupation : EntitiesBase.Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("PersonnelOccupationId")]
        public int PersonnelOccupationId { get; set; }
        public int PersonnelId { get; set; }
        [ForeignKey("PersonnelId")]
        public Personnel Personnel { get; set; }
        public int OccupationCategoryCD { get; set; }
        [ForeignKey("OccupationCategoryCD")]
        public Code OccupationCategory { get; set; }
        public int OccupationSubCategoryCD { get; set; }
        [ForeignKey("OccupationSubCategoryCD")]
        public Code OccupationSubCategory { get; set; }
        public int OccupationCD { get; set; }
        [ForeignKey("OccupationCD")]
        public Code Occupation { get; set; }
        public int? PersonnelSeniorityCD { get; set; }
        [ForeignKey("PersonnelSeniorityCD")]
        public Code PersonnelSeniority { get; set; }

        public void Copy(PersonnelOccupation personnelOccupation, int personnelId)
        {
            this.PersonnelId = personnelId;
            this.OccupationCategoryCD = personnelOccupation.OccupationCategoryCD;
            this.OccupationSubCategoryCD = personnelOccupation.OccupationSubCategoryCD;
            this.OccupationCD = personnelOccupation.OccupationCD;
            this.PersonnelSeniorityCD = personnelOccupation.PersonnelSeniorityCD;
        }
    }
}
