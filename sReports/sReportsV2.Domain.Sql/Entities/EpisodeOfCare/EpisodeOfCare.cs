using sReportsV2.Common.Entities.User;
using sReportsV2.Common.Enums;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Domain.Sql.Entities.PersonnelTeamEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;

namespace sReportsV2.Domain.Sql.Entities.EpisodeOfCare
{
    public class EpisodeOfCare : EntitiesBase.Entity, IReplaceThesaurusEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("EpisodeOfCareId")]
        public int EpisodeOfCareId { get; set; }
        public int PatientId { get; set; }
        public int OrganizationId { get; set; }
        [Column("StatusCD")]
        public int StatusCD { get; set; }
        [ForeignKey("StatusCD")]
        public Code Status { get; set; }
        [Column("TypeCD")]
        public int TypeCD { get; set; }
        [ForeignKey("TypeCD")]
        public Code Type { get; set; }
        [ForeignKey("DiagnosisConditionId")]
        public ThesaurusEntry.ThesaurusEntry DiagnosisCondition { get; set; }
        public int? DiagnosisConditionId { get; set; }
        public PeriodDatetime Period { get; set; }
        public string Description { get; set; }
        public int? PersonnelTeamId { get; set; }
        [ForeignKey("PersonnelTeamId")]
        public PersonnelTeam PersonnelTeam { get; set; }
        public virtual List<Encounter.Encounter> Encounters { get; set; } = new List<Encounter.Encounter>();
        public virtual Patient.Patient Patient { get; set; }

        public void Copy(EpisodeOfCare episodeOfCare)
        {
            this.StatusCD = episodeOfCare.StatusCD;
            this.TypeCD = episodeOfCare.TypeCD;
            this.Description = episodeOfCare.Description;
            this.DiagnosisConditionId = episodeOfCare.DiagnosisConditionId;
            this.Period = new PeriodDatetime()
            {
                Start = episodeOfCare.Period.Start,
                End = episodeOfCare.Period.End
            };
            this.PersonnelTeamId = episodeOfCare.PersonnelTeamId;
        }

        public void ReplaceThesauruses(ThesaurusMerge thesaurusMerge)
        {
            this.DiagnosisConditionId = this.DiagnosisConditionId.ReplaceThesaurus(thesaurusMerge);
        }

        public override void Delete(DateTimeOffset? activeTo = null, bool setLastUpdateProperty = true, string organizationTimeZone = null)
        {
            var activeToDate = activeTo ?? DateTimeOffset.UtcNow.ConvertToOrganizationTimeZone(organizationTimeZone);

            foreach (var encounter in Encounters)
                encounter.Delete(activeToDate);

            this.ActiveTo = activeTo ?? DateTimeOffset.UtcNow.ConvertToOrganizationTimeZone(organizationTimeZone);
        }

        public Encounter.Encounter AddNewOrUpdateOldEntriesFromHL7(Encounter.Encounter upcomingEncounter)
        {
            Encounter.Encounter procedeedEncounter = null;
            if (this.Encounters == null)
            {
                this.Encounters = new List<Encounter.Encounter>();
            }
            var dbEncounter = Encounters.Find(x =>
                x.IsHL7EncounterMatch(upcomingEncounter.EncounterIdentifiers.FirstOrDefault())
                && x.IsActive()
            );
            if (dbEncounter != null)
            {
                dbEncounter?.CopyFromHL7(upcomingEncounter);
                procedeedEncounter = dbEncounter;
            }
            else
            {
                Encounters.Add(upcomingEncounter);
                procedeedEncounter = upcomingEncounter;
            }

            return procedeedEncounter;
        }
    }
}
