using sReportsV2.Domain.Sql.Entities.Common;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Extensions;

namespace sReportsV2.Domain.Sql.Entities.Encounter
{
    public class Encounter : EntitiesBase.Entity, IEditChildEntries<PersonnelEncounterRelation>
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("EncounterId")]
        public int EncounterId { get; set; }
        public int EpisodeOfCareId { get; set; }
        public EpisodeOfCare.EpisodeOfCare EpisodeOfCare { get; set; }
        public int? PatientId { get; set; }
        [ForeignKey("PatientId")]
        public virtual Patient.Patient Patient { get; set; }
        public int? AdmitSourceCD { get; set; }
        [ForeignKey("AdmitSourceCD")]
        public Code AdmitSource { get; set; }

        public int? StatusCD { get; set; }
        [ForeignKey("StatusCD")]
        public Code EncounterStatus { get; set; }

        public int? ClassCD { get; set; }
        [ForeignKey("ClassCD")]
        public Code EncounterClass { get; set; }

        public int? TypeCD { get; set; }
        [ForeignKey("TypeCD")]
        public Code EncounterType { get; set; }

        public int? ServiceTypeCD { get; set; }
        [ForeignKey("ServiceTypeCD")]
        public Code EncounterServiceType { get; set; }

        public DateTimeOffset? AdmissionDate { get; set; }
        public DateTimeOffset? DischargeDate { get; set; }
        public virtual List<EncounterIdentifier> EncounterIdentifiers { get; set; } = new List<EncounterIdentifier>();
        public virtual List<PersonnelEncounterRelation> PersonnelEncounterRelations { get; set; }
        public virtual List<TaskEntry.Task> Tasks { get; set; } = new List<TaskEntry.Task>();

        public Encounter()
        {

        }

        public override void Delete(DateTimeOffset? activeTo = null, bool setLastUpdateProperty = true, string organizationTimeZone = null)
        {
            var activeToDate = activeTo ?? DateTimeOffset.UtcNow.ConvertToOrganizationTimeZone(organizationTimeZone);

            foreach (var task in Tasks)
                task.Delete(activeToDate);

            this.ActiveTo = activeToDate;
        }

        public bool IsHL7EncounterMatch(EncounterIdentifier upcomingEncounter)
        {
            return this.IsActive()
                && this.EncounterIdentifiers.Exists(eI => eI.IsHL7EncounterMatch(upcomingEncounter));
        }

        public void Copy(Encounter encounter)
        {
            this.ClassCD = encounter.ClassCD;
            this.TypeCD = encounter.TypeCD;
            this.StatusCD = encounter.StatusCD;
            this.ServiceTypeCD = encounter.ServiceTypeCD;
            this.AdmissionDate = encounter.AdmissionDate;
            this.DischargeDate = encounter.DischargeDate;

            CopyEntries(encounter.PersonnelEncounterRelations);
        }

        public void CopyFromHL7(Encounter encounter)
        {
            this.ClassCD = encounter.ClassCD;
            this.TypeCD = encounter.TypeCD;
            this.AdmitSourceCD = encounter.AdmitSourceCD;
            this.AdmissionDate = encounter.AdmissionDate;
            this.DischargeDate = encounter.DischargeDate;

            AddNewEntriesFromHL7(encounter.EncounterIdentifiers);
            CopyEntriesFromHL7(encounter.PersonnelEncounterRelations);
        }

        private void AddNewEntriesFromHL7(List<EncounterIdentifier> upcomingEntries)
        {
            foreach (var encounterIdentifier in upcomingEntries)
            {
                var dbEncounterIdentifier = EncounterIdentifiers.Find(x => x.IsHL7EncounterMatch(encounterIdentifier) 
                    && x.IsActive());
                if (dbEncounterIdentifier == null)
                {
                    EncounterIdentifiers.Add(encounterIdentifier);
                }
            }
        }

        public void CopyEntries(List<PersonnelEncounterRelation> upcomingEntries)
        {
            if (upcomingEntries != null)
            {
                DeleteExistingRemovedEntries(upcomingEntries);
                AddNewOrUpdateOldEntries(upcomingEntries);
            }
        }

        public void DeleteExistingRemovedEntries(List<PersonnelEncounterRelation> upcomingEntries)
        {
            foreach (var personnelEncounterRelation in PersonnelEncounterRelations)
            {
                var remainingPatientEncounterRelation = upcomingEntries.Exists(x => x.PersonnelEncounterRelationId == personnelEncounterRelation.PersonnelEncounterRelationId);
                if (!remainingPatientEncounterRelation)
                {
                    personnelEncounterRelation.Delete();
                }
            }
        }

        public void AddNewOrUpdateOldEntries(List<PersonnelEncounterRelation> upcomingEntries)
        {
            foreach (var personnelEncounterRelation in upcomingEntries)
            {
                if (personnelEncounterRelation.PersonnelEncounterRelationId == 0)
                {
                    PersonnelEncounterRelations.Add(personnelEncounterRelation);
                }
                else
                {
                    var dbPersonnelEncounterRelation = PersonnelEncounterRelations.Find(x => x.PersonnelEncounterRelationId == personnelEncounterRelation.PersonnelEncounterRelationId && x.IsActive());
                    dbPersonnelEncounterRelation?.Copy(personnelEncounterRelation);
                }
            }
        }

        public void CopyEntriesFromHL7(List<PersonnelEncounterRelation> upcomingEntries)
        {
            if (upcomingEntries != null)
            {
                DeleteExistingRemovedEntriesFromHL7(upcomingEntries);
                AddNewEntriesFromHL7(upcomingEntries);
            }
        }

        private void DeleteExistingRemovedEntriesFromHL7(List<PersonnelEncounterRelation> upcomingEntries)
        {
            foreach (var doctor in PersonnelEncounterRelations)
            {
                var remainingDoctor = upcomingEntries.Exists(x => x.IsHL7DoctorMatch(doctor));
                if (!remainingDoctor)
                {
                    doctor.Delete();
                }
            }
        }

        private void AddNewEntriesFromHL7(List<PersonnelEncounterRelation> upcomingEntries)
        {
            foreach (var doctor in upcomingEntries)
            {
                var dbDoctor = PersonnelEncounterRelations.Find(x => x.IsHL7DoctorMatch(doctor)
                    && x.IsActive());
                if (dbDoctor == null)
                {
                    if (doctor.PersonnelId > 0)
                    {
                        PersonnelEncounterRelations.Add(new PersonnelEncounterRelation()
                        {
                            PersonnelId = doctor.PersonnelId,
                            RelationTypeCD = doctor.RelationTypeCD
                        });
                    }
                    else
                    {
                        PersonnelEncounterRelations.Add(doctor);
                    }
                }
            }
        }
    }
}
