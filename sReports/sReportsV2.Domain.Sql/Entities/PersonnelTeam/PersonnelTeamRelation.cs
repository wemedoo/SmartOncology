using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Domain.Sql.Entities.User;
using sReportsV2.Domain.Sql.EntitiesBase;
using System;

namespace sReportsV2.Domain.Sql.Entities.PersonnelTeamEntities
{
    public class PersonnelTeamRelation : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("PersonnelTeamRelationId")]
        public int PersonnelTeamRelationId { get; set; }
        public int? RelationTypeCD { get; set; }
        [ForeignKey("RelationTypeCD")]
        public Code RelationType { get; set; }
        public int? PersonnelTeamId { get; set; }
        [ForeignKey("PersonnelTeamId")]
        public PersonnelTeam PersonnelTeam { get; set; }
        public int? PersonnelId { get; set; }
        [ForeignKey("PersonnelId")]
        public Personnel Personnel { get; set; }

        public void CopyData(PersonnelTeamRelation copyForm)
        {
            this.RelationTypeCD = copyForm.RelationTypeCD;
        }
    }
}