using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Domain.Sql.EntitiesBase;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sReportsV2.Domain.Sql.Entities.OrganizationEntities;

namespace sReportsV2.Domain.Sql.Entities.PersonnelTeamEntities
{
    public class PersonnelTeamOrganizationRelation : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("PersonnelTeamOrganizationRelationId")]
        public int PersonnelTeamOrganizationRelationId { get; set; }
        public int PersonnelTeamId { get; set; }
        [ForeignKey("PersonnelTeamId")]
        public PersonnelTeam PersonnelTeam { get; set; }
        public int OrganizationId { get; set; }
        [ForeignKey("OrganizationId")]
        public Organization Organization { get; set; }
        public int? RelationTypeCD { get; set; }
        [ForeignKey("RelationTypeCD")]
        public Code RelationType { get; set; }

        public void CopyData(PersonnelTeamOrganizationRelation copyForm)
        {
            this.RelationTypeCD = copyForm.RelationTypeCD;
        }

    }
}
