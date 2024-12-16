using sReportsV2.Common.Enums;
using sReportsV2.Domain.Sql.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace sReportsV2.Domain.Sql.Entities.Patient
{
    public class PatientContact : EntitiesBase.Entity, IEditChildEntries<PatientContactAddress>, IEditChildEntries<PatientContactTelecom>
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("PatientContactId")]
        public int PatientContactId { get; set; }
        public int? PatientId { get; set; }
        public Patient Patient { get; set; }
        public string NameGiven { get; set; }
        public string NameFamily { get; set; }
        public int? GenderCD { get; set; }

        [ForeignKey("GenderCD")]
        public Code Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public int? ContactRelationshipCD { get; set; }

        [ForeignKey("ContactRelationshipCD")]
        public Code ContactRelationship { get; set; }
        public int? ContactRoleCD { get; set; }

        [ForeignKey("ContactRoleCD")]
        public Code ContactRole { get; set; }
        public List<PatientContactTelecom> PatientContactTelecoms { get; set; }
        public List<PatientContactAddress> PatientContactAddresses { get; set; }
        public DateTime? ContactRoleStartDate { get; set; }
        public DateTime? ContactRoleEndDate { get; set; }

        public PatientContact()
        {
            PatientContactAddresses = new List<PatientContactAddress>();
            PatientContactTelecoms = new List<PatientContactTelecom>();
        }

        public void Copy(PatientContact contact)
        {
            if(contact != null)
            {
                this.GenderCD = contact.GenderCD;
                this.NameGiven = contact.NameGiven;
                this.NameFamily = contact.NameFamily;
                this.BirthDate = contact.BirthDate;
                this.ContactRelationshipCD = contact.ContactRelationshipCD;
                this.ContactRoleCD = contact.ContactRoleCD;

                CopyEntries(contact.PatientContactAddresses);
                CopyEntries(contact.PatientContactTelecoms);
            }
        }

        #region Edit Child entries

        public void CopyEntries(List<PatientContactTelecom> upcomingEntries)
        {
            if (upcomingEntries != null)
            {
                DeleteExistingRemovedEntries(upcomingEntries);
                AddNewOrUpdateOldEntries(upcomingEntries);
            }
        }

        public void DeleteExistingRemovedEntries(List<PatientContactTelecom> upcomingEntries)
        {
            foreach (var telecom in PatientContactTelecoms)
            {
                var remainingTelecom = upcomingEntries.Exists(x => x.PatientContactTelecomId == telecom.PatientContactTelecomId);
                if (!remainingTelecom)
                {
                    telecom.Delete();
                }
            }
        }

        public void AddNewOrUpdateOldEntries(List<PatientContactTelecom> upcomingEntries)
        {
            foreach (var patientTelecom in upcomingEntries)
            {
                if (patientTelecom.PatientContactTelecomId == 0)
                {
                    PatientContactTelecoms.Add(patientTelecom);
                }
                else
                {
                    var dbPatientTelecom = PatientContactTelecoms.Find(x => x.PatientContactTelecomId == patientTelecom.PatientContactTelecomId && x.IsActive());
                    if (dbPatientTelecom != null)
                    {
                        dbPatientTelecom.Copy(patientTelecom);
                    }
                }
            }
        }

        public void CopyEntries(List<PatientContactAddress> upcomingEntries)
        {
            if (upcomingEntries != null)
            {
                DeleteExistingRemovedEntries(upcomingEntries);
                AddNewOrUpdateOldEntries(upcomingEntries);
            }
        }

        public void DeleteExistingRemovedEntries(List<PatientContactAddress> upcomingEntries)
        {
            foreach (var address in PatientContactAddresses)
            {
                var remainingAddress = upcomingEntries.Exists(x => x.PatientContactAddressId == address.PatientContactAddressId);
                if (!remainingAddress)
                {
                    address.Delete();
                }
            }
        }

        public void AddNewOrUpdateOldEntries(List<PatientContactAddress> upcomingEntries)
        {
            foreach (var patientContactAddress in upcomingEntries)
            {
                if (patientContactAddress.PatientContactAddressId == 0)
                {
                    PatientContactAddresses.Add(patientContactAddress);
                }
                else
                {
                    var dbPatientContactAddress = PatientContactAddresses.Find(x => x.PatientContactAddressId == patientContactAddress.PatientContactAddressId && x.IsActive());
                    if (dbPatientContactAddress != null)
                    {
                        dbPatientContactAddress.Copy(patientContactAddress);
                    }
                }
            }
        }

        #endregion /Edit Child entries

        public bool IsHL7ContactMatch(PatientContact contact)
        {
            return this.NameFamily == contact.NameFamily && this.NameGiven == contact.NameGiven && this.BirthDate == contact.BirthDate;
        }
    }
}
