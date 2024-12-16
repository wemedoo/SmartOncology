using sReportsV2.Domain.Sql.EntitiesBase;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using sReportsV2.Domain.Sql.Entities.OrganizationEntities;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Common.Extensions;
using System.Linq;

namespace sReportsV2.Domain.Sql.Entities.PersonnelTeamEntities
{
    public class PersonnelTeam : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("PersonnelTeamId")]
        public int PersonnelTeamId { get; set; }
        public string Name { get; set; }
        public int? TypeCD { get; set; }
        [ForeignKey("TypeCD")]
        public Code Type { get; set; }
        public List<PersonnelTeamRelation> PersonnelTeamRelations { get; set; } = new List<PersonnelTeamRelation>();
        public virtual List<PersonnelTeamOrganizationRelation> PersonnelTeamOrganizationRelations { get; set; } = new List<PersonnelTeamOrganizationRelation>();

        public void CopyData(PersonnelTeam copyFrom)
        {
            this.PersonnelTeamId = copyFrom.PersonnelTeamId;
            this.Name = copyFrom.Name.RemoveDiacritics();
            SetPersonnelTeamRelations(copyFrom.PersonnelTeamRelations);
            SetPersonnelTeamOrganizationRelations(copyFrom.PersonnelTeamOrganizationRelations);
            this.TypeCD = copyFrom.TypeCD;
        }

        private void SetPersonnelTeamRelations(List<PersonnelTeamRelation> personnelTeamRelationsToCopy)
        {
            foreach(PersonnelTeamRelation personnelTeamRelation in personnelTeamRelationsToCopy)
            {
                int replaceIndex = PersonnelTeamRelations.FindIndex(x => x.PersonnelTeamRelationId == personnelTeamRelation.PersonnelTeamRelationId);

                if (replaceIndex != -1)
                    PersonnelTeamRelations[replaceIndex].CopyData(personnelTeamRelation);
                else
                    PersonnelTeamRelations.Add(personnelTeamRelation);
            }
        }

        private void SetPersonnelTeamOrganizationRelations(List<PersonnelTeamOrganizationRelation> personnelTeamOrganizationRelationsToCopy)
        {
            foreach (PersonnelTeamOrganizationRelation personnelTeamOrganizationRelations in personnelTeamOrganizationRelationsToCopy)
            {
                int personnelTeamOrganizationRelationId = PersonnelTeamOrganizationRelations.Find(x => x.OrganizationId == personnelTeamOrganizationRelations.OrganizationId
                    && x.PersonnelTeamId == this.PersonnelTeamId).PersonnelTeamOrganizationRelationId;

                if (personnelTeamOrganizationRelationId != 0)
                {
                    var index = PersonnelTeamOrganizationRelations.FindIndex(x => x.PersonnelTeamOrganizationRelationId == personnelTeamOrganizationRelationId);
                    PersonnelTeamOrganizationRelations[index].CopyData(personnelTeamOrganizationRelations);

                }
                else
                    PersonnelTeamOrganizationRelations.Add(personnelTeamOrganizationRelations);
            }
        }
    }
}
