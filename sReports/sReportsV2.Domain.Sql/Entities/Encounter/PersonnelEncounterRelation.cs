using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using sReportsV2.Domain.Sql.Entities.User;
using sReportsV2.Domain.Sql.Entities.Common;
using System.Linq;

namespace sReportsV2.Domain.Sql.Entities.Encounter
{
    public class PersonnelEncounterRelation : EntitiesBase.Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int PersonnelEncounterRelationId { get; set; }

        public int EncounterId { get; set; }
        [ForeignKey("EncounterId")]
        public virtual Encounter Encounter { get; set; }

        public int PersonnelId { get; set; }
        [ForeignKey("PersonnelId")]
        public virtual Personnel Personnel { get; set; }

        public int? RelationTypeCD { get; set; }
        [ForeignKey("RelationTypeCD")]
        public Code RelationType { get; set; }

        public PersonnelEncounterRelation() { }

        public void Copy(PersonnelEncounterRelation personnelEncounterRelation)
        {
            this.PersonnelId = personnelEncounterRelation.PersonnelId;
            this.RelationTypeCD = personnelEncounterRelation.RelationTypeCD;
        }

        public bool IsHL7DoctorMatch(PersonnelEncounterRelation incomingDoctor)
        {
            return this.RelationTypeCD == incomingDoctor.RelationTypeCD
                && incomingDoctor.Personnel.PersonnelIdentifiers.Exists(i =>
                    this.Personnel.PersonnelIdentifiers.Exists(pI =>
                        pI.IdentifierValue != null && pI.IdentifierValue == i.IdentifierValue &&
                        pI.IdentifierTypeCD != null && pI.IdentifierTypeCD == i.IdentifierTypeCD &&
                        pI.IdentifierPoolCD != null && pI.IdentifierPoolCD == i.IdentifierPoolCD
                        )
                    );
        }
    }
}
