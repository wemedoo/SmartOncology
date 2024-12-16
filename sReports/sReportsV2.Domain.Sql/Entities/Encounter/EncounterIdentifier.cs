using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace sReportsV2.Domain.Sql.Entities.Encounter
{
    [Table("EncounterIdentifiers")]
    public class EncounterIdentifier : Base.IdentifierBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int EncounterIdentifierId { get; set; }

        public int? EncounterId { get; set; }
        [ForeignKey("EncounterId")]
        public Encounter Encounter { get; set; }

        public EncounterIdentifier()
        {
        }

        public EncounterIdentifier(int? identifierTypeCD, string value, int? identifierUseCD = null) : base(identifierTypeCD, value, identifierUseCD)
        {
        }

        public bool IsHL7EncounterMatch(EncounterIdentifier upcomingEncounter)
        {
            return this.IdentifierTypeCD != null && this.IdentifierTypeCD == upcomingEncounter.IdentifierTypeCD
                && this.IdentifierPoolCD != null && this.IdentifierPoolCD == upcomingEncounter.IdentifierPoolCD
                && this.IdentifierValue == upcomingEncounter.IdentifierValue
                ;
        }
    }
}
