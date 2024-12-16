using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Domain.Sql.Entities.ProjectEntry;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace sReportsV2.Domain.Sql.Entities.Patient
{
    public class Patient : EntitiesBase.Entity, IEditChildEntries<PatientIdentifier>, IEditChildEntries<PatientContact>, IEditChildEntries<PatientAddress>, IEditChildEntries<PatientTelecom>
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("PatientId")]
        public int PatientId { get; set; }
        public int OrganizationId { get; set; }
        public string NameGiven { get; set; }
        public string NameFamily { get; set; }
        public int? GenderCD { get; set; }

        [ForeignKey("GenderCD")]
        public Code Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        [ForeignKey("MultipleBirthId")]
        public MultipleBirth MultipleBirth { get; set; }
        [Column("MultipleBirthId")]
        public int? MultipleBirthId { get; set; }
        public List<Communication> Communications { get; set; }
        public virtual List<EpisodeOfCare.EpisodeOfCare> EpisodeOfCares { get; set; }
        public virtual List<PatientAddress> PatientAddresses { get; set; }
        public virtual List<PatientTelecom> PatientTelecoms { get; set; }

        [Column("CitizenshipCD")]
        public int? CitizenshipCD { get; set; }
        [ForeignKey("CitizenshipCD")]
        public Code Citizenship { get; set; }

        [Column("ReligionCD")]
        public int? ReligionCD { get; set; }
        [ForeignKey("ReligionCD")]
        public Code Religion { get; set; }
        public int? MaritalStatusCD { get; set; }
        [ForeignKey("MaritalStatusCD")]
        public Code MaritalStatus { get; set; }
        public DateTime? DeceasedDateTime { get; set; }
        public bool? Deceased { get; set; }
        public virtual List<PatientContact> PatientContacts { get; set; }
        public virtual List<PatientIdentifier> PatientIdentifiers { get; set; } = new List<PatientIdentifier>();
        [StringLength(129)]
        public string MiddleName { get; set; }
        public virtual List<ProjectPatientRelation> ProjectPatientRelations { get; set; } = new List<ProjectPatientRelation>();
        public virtual List<Encounter.Encounter> Encounters { get; set; } = new List<Encounter.Encounter>();

        public PatientChemotherapyData PatientChemotherapyData { get; set; }

        public Patient() 
        {
            PatientAddresses = new List<PatientAddress>();
            PatientTelecoms = new List<PatientTelecom>();
            PatientIdentifiers = new List<PatientIdentifier>();
            PatientContacts = new List<PatientContact>();
            EpisodeOfCares = new List<EpisodeOfCare.EpisodeOfCare>();
        }

        public Patient(string nameGiven, string nameFamily) 
        {
            this.NameGiven = nameGiven;
            this.NameFamily = nameFamily;
        }

        public override void Delete(DateTimeOffset? activeTo = null, bool setLastUpdateProperty = true, string organizationTimeZone = null)
        {
            var activeToDate = activeTo ?? DateTimeOffset.UtcNow.ConvertToOrganizationTimeZone(organizationTimeZone);

            foreach (var episodeOfCare in EpisodeOfCares)
                episodeOfCare.Delete(activeToDate);

            this.PatientChemotherapyData?.Delete(activeToDate);
            this.ActiveTo = activeToDate;
        }

        #region Patient Change Actions
        public void Copy(Patient patient, bool doHL7CopyContacts = false)
        {
            GenderCD = patient.GenderCD;
            BirthDate = patient.BirthDate;
            NameGiven = patient.NameGiven;
            NameFamily = patient.NameFamily;
            if (MultipleBirth != null)
            {
                MultipleBirth.isMultipleBorn = patient.MultipleBirth.isMultipleBorn;
                MultipleBirth.Number = patient.MultipleBirth.Number;
            }
            SetComunication(patient.Communications);

            OrganizationId = patient.OrganizationId;
            CitizenshipCD = patient.CitizenshipCD;
            ReligionCD = patient.ReligionCD;
            MaritalStatusCD = patient.MaritalStatusCD;
            Deceased = patient.Deceased;
            DeceasedDateTime = patient.DeceasedDateTime;

            CopyEntries(patient.PatientIdentifiers);
            CopyEntries(patient.PatientAddresses);
            CopyEntries(patient.PatientTelecoms);
            PatientChemotherapyData?.Copy(patient.PatientChemotherapyData);
            if (doHL7CopyContacts)
            {
                CopyEntriesFromHL7(patient.PatientContacts);
            }
            else
            {
                CopyEntries(patient.PatientContacts);
            }
        }

        public Encounter.Encounter CopyEocAndEncountersFromHL7(EpisodeOfCare.EpisodeOfCare incomingEpisodeOfCare)
        {
            Encounter.Encounter procedeedEncounter = null;
            Encounter.Encounter incomingEncounter = incomingEpisodeOfCare.Encounters.FirstOrDefault();
            incomingEpisodeOfCare?.Encounters?.ForEach(x => x.PatientId = this.PatientId);
            if (EpisodeOfCares == null)
            {
                EpisodeOfCares = new List<EpisodeOfCare.EpisodeOfCare>();
            }
            if (EpisodeOfCares.Count == 0)
            {
                EpisodeOfCares = new List<EpisodeOfCare.EpisodeOfCare>() { incomingEpisodeOfCare };
                procedeedEncounter = incomingEncounter;
            }
            else
            {
                EpisodeOfCare.EpisodeOfCare latestEpisodeOfCare = EpisodeOfCares.OrderBy(x => x.EntryDatetime).Last();
                latestEpisodeOfCare.StatusCD = incomingEpisodeOfCare.StatusCD;
                procedeedEncounter = latestEpisodeOfCare.AddNewOrUpdateOldEntriesFromHL7(incomingEncounter);
            }
            return procedeedEncounter;
        }
        #endregion /Patient Change Actions


        #region Edit Child entries

        public void CopyEntries(List<PatientIdentifier> upcomingEntries)
        {
            if (upcomingEntries != null)
            {
                DeleteExistingRemovedEntries(upcomingEntries);
                AddNewOrUpdateOldEntries(upcomingEntries);
            }
        }

        public void DeleteExistingRemovedEntries(List<PatientIdentifier> upcomingEntries)
        {
            foreach (var identifier in PatientIdentifiers)
            {
                var remainingIdentifier = upcomingEntries.Exists(x => x.PatientIdentifierId == identifier.PatientIdentifierId);
                if (!remainingIdentifier)
                {
                    identifier.Delete();
                }
            }
        }

        public void AddNewOrUpdateOldEntries(List<PatientIdentifier> upcomingEntries)
        {
            foreach (var identifier in upcomingEntries)
            {
                if (identifier.PatientIdentifierId == 0)
                {
                    PatientIdentifiers.Add(identifier);
                }
                else
                {
                    var dbIdentifier = PatientIdentifiers.Find(x => x.PatientIdentifierId == identifier.PatientIdentifierId 
                        && x.IsActive());
                    dbIdentifier?.Copy(identifier);
                }
            }
        }

        public void CopyEntries(List<PatientContact> upcomingEntries)
        {
            if (upcomingEntries != null)
            {
                DeleteExistingRemovedEntries(upcomingEntries);
                AddNewOrUpdateOldEntries(upcomingEntries);
            }
        }

        public void DeleteExistingRemovedEntries(List<PatientContact> upcomingEntries)
        {
            foreach (var contact in PatientContacts)
            {
                var remainingContact = upcomingEntries.Exists(x => x.PatientContactId == contact.PatientContactId);
                if (!remainingContact)
                {
                    contact.Delete();
                }
            }
        }

        public void AddNewOrUpdateOldEntries(List<PatientContact> upcomingEntries)
        {
            foreach (var patientContact in upcomingEntries)
            {
                if (patientContact.PatientContactId == 0)
                {
                    PatientContacts.Add(patientContact);
                }
                else
                {
                    var dbPatientContact = PatientContacts.Find(x => x.PatientContactId == patientContact.PatientContactId 
                        && x.IsActive());
                    dbPatientContact?.Copy(patientContact);
                }
            }
        }

        public void CopyEntries(List<PatientTelecom> upcomingEntries)
        {
            if (upcomingEntries != null)
            {
                DeleteExistingRemovedEntries(upcomingEntries);
                AddNewOrUpdateOldEntries(upcomingEntries);
            }
        }

        public void DeleteExistingRemovedEntries(List<PatientTelecom> upcomingEntries)
        {
            foreach (var telecom in PatientTelecoms)
            {
                var remainingTelecom = upcomingEntries.Exists(x => x.PatientTelecomId == telecom.PatientTelecomId);
                if (!remainingTelecom)
                {
                    telecom.Delete();
                }
            }
        }

        public void AddNewOrUpdateOldEntries(List<PatientTelecom> upcomingEntries)
        {
            foreach (var patientTelecom in upcomingEntries)
            {
                if (patientTelecom.PatientTelecomId == 0)
                {
                    PatientTelecoms.Add(patientTelecom);
                }
                else
                {
                    var dbPatientTelecom = PatientTelecoms.Find(x => x.PatientTelecomId == patientTelecom.PatientTelecomId 
                        && x.IsActive());
                    dbPatientTelecom?.Copy(patientTelecom);
                }
            }
        }

        public void CopyEntries(List<PatientAddress> upcomingEntries)
        {
            if (upcomingEntries != null)
            {
                DeleteExistingRemovedEntries(upcomingEntries);
                AddNewOrUpdateOldEntries(upcomingEntries);
            }
        }

        public void DeleteExistingRemovedEntries(List<PatientAddress> upcomingEntries)
        {
            foreach (var address in PatientAddresses)
            {
                var remainingAddress = upcomingEntries.Exists(x => x.PatientAddressId == address.PatientAddressId);
                if (!remainingAddress)
                {
                    address.Delete();
                }
            }
        }

        public void AddNewOrUpdateOldEntries(List<PatientAddress> upcomingEntries)
        {
            foreach (var patientAddress in upcomingEntries)
            {
                if (patientAddress.PatientAddressId == 0)
                {
                    PatientAddresses.Add(patientAddress);
                }
                else
                {
                    var dbPatientAddress = PatientAddresses.Find(x => x.PatientAddressId == patientAddress.PatientAddressId 
                        && x.IsActive());
                    dbPatientAddress?.Copy(patientAddress);
                }
            }
        }

        #endregion /Edit Child entries

        public void CopyEntriesFromHL7(List<PatientContact> upcomingEntries)
        {
            if (upcomingEntries != null)
            {
                DeleteExistingRemovedEntriesFromHL7(upcomingEntries);
                AddNewOrUpdateOldEntriesFromHL7(upcomingEntries);
            }
        }

        private void DeleteExistingRemovedEntriesFromHL7(List<PatientContact> upcomingEntries)
        {
            foreach (var contact in PatientContacts)
            {
                var remainingContact = upcomingEntries.Exists(x => x.IsHL7ContactMatch(contact));
                if (!remainingContact)
                {
                    contact.Delete();
                }
            }
        }

        private void AddNewOrUpdateOldEntriesFromHL7(List<PatientContact> upcomingEntries)
        {
            foreach (var patientContact in upcomingEntries)
            {
                var dbPatientContact = PatientContacts.Find(x => x.IsHL7ContactMatch(patientContact) 
                    && x.IsActive());
                if (dbPatientContact != null)
                {
                    dbPatientContact?.Copy(patientContact);
                }
                else
                {
                    PatientContacts.Add(patientContact);
                }
            }
        }

        private void SetComunication(List<Communication> communications)
        {
            if (communications == null) return;
            if (this.Communications == null)
            {
                this.Communications = new List<Communication>();
            }

            foreach (var communication in communications.Where(x => x.CommunicationId != 0))
            {
                var communicationDb = this.Communications.Find(c => c.CommunicationId == communication.CommunicationId);
                communicationDb?.Copy(communication);
            }

            foreach (var communication in communications.Where(x => x.CommunicationId == 0))
            {
                this.Communications.Add(communication);
            }
        }
    }
}
