using sReportsV2.Domain.Sql.Entities.PersonnelTeamEntities;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Domain.Sql.EntitiesBase;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using sReportsV2.Common.Constants;
using sReportsV2.Domain.Sql.Entities.PatientList;
using sReportsV2.Common.Extensions;

namespace sReportsV2.Domain.Sql.Entities.User
{
    [Table("Personnel")]
    public class Personnel : Entity, IEditChildEntries<PersonnelIdentifier>, IEditChildEntries<PersonnelAddress>, IEditChildEntries<PersonnelAcademicPosition>, IEditChildEntries<PersonnelPosition>
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int PersonnelId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [StringLength(128)]
        public string SystemName { get; set; }
        public DateTime? DayOfBirth { get; set; }
        public string MiddleName { get; set; }
        public string Email { get; set; }
        public string PersonalEmail { get; set; }
        public string ContactPhone { get; set; }
        public bool IsDoctor { get; set; }
        [NotMapped]
        public List<string> PasswordHistory { get; set; } = new List<string>();

        [Column("PrefixCD")]
        public int? PrefixCD { get; set; }
        [ForeignKey("PrefixCD")]
        public virtual Code Prefix { get; set; }
        public int? PersonnelTypeCD { get; set; }
        [ForeignKey("PersonnelTypeCD")]
        public virtual Code PersonnelType { get; set; }
        public int PersonnelConfigId { get; set; }
        [ForeignKey("PersonnelConfigId")]
        public virtual PersonnelConfig PersonnelConfig { get; set; }
        public int? PersonnelOccupationId { get; set; }
        [ForeignKey("PersonnelOccupationId")]
        public virtual PersonnelOccupation PersonnelOccupation { get; set; }
        public virtual List<PersonnelOrganization> Organizations { get; set; } = new List<PersonnelOrganization>();
        public virtual List<PersonnelTeamRelation> PersonnelTeams { get; set; } = new List<PersonnelTeamRelation>();
        public virtual List<PersonnelIdentifier> PersonnelIdentifiers { get; set; } = new List<PersonnelIdentifier>();
        public virtual List<PersonnelAddress> PersonnelAdresses { get; set; } = new List<PersonnelAddress>();
        public virtual List<PersonnelAcademicPosition> PersonnelAcademicPositions { get; set; } = new List<PersonnelAcademicPosition>();
        public virtual List<PersonnelPosition> PersonnelPositions { get; set; } = new List<PersonnelPosition>();
        public virtual List<PatientListPersonnelRelation> PatientListPersonnelRelations { get; set; } = new List<PatientListPersonnelRelation>();

        public Personnel()
        {
        }

        public Personnel(string firstName, string lastName) : this()
        {
            FirstName = firstName;
            LastName = lastName;
        }

        public Personnel(string username, string password, string salt, string email, string firstName, string lastName, DateTime? dayOfBirth, int? activeOrganizationId = null, int? superAdministratorPositionId = null) : this(firstName, lastName)
        {
            Salt = salt;
            DayOfBirth = dayOfBirth;
            Email = email;
            Username = username;
            Password = password;
            PersonnelConfig = new PersonnelConfig()
            {
                ActiveOrganizationId = activeOrganizationId,
                ActiveLanguage = LanguageConstants.EN,
                TimeZoneOffset = DateTimeExtension.DefaultTimezone
            };
            if (superAdministratorPositionId.HasValue)
            {
                PersonnelPositions = new List<PersonnelPosition>
                {
                    new PersonnelPosition
                    {
                        PositionCD = superAdministratorPositionId,
                        CreatedBy = this
                    }
                };
            }
        }

        public List<PersonnelOrganization> GetNonArchivedOrganizations(int? archivedUserStateCD)
        {
            return Organizations.Where(x => x.StateCD != archivedUserStateCD && x.Organization.IsActive()).ToList();
        }

        public IEnumerable<PersonnelOrganization> GetOrganizations()
        {
            return Organizations.Where(x => x.Organization.IsActive());
        }

        public List<int> GetOrganizationRefs()
        {
            if (this.Organizations == null)
                this.Organizations = new List<PersonnelOrganization>();

            return this.Organizations.Select(x => x.OrganizationId).ToList();
        }

        public void AddSuggestedForm(string formId)
        {
            this.PersonnelConfig.AddSuggestedForm(formId);
        }

        public void RemoveSuggestedForm(string formId)
        {
            this.PersonnelConfig.RemoveSuggestedForm(formId);
        }

        public void Copy(Personnel user)
        {
            this.Username = user.Username;
            this.PrefixCD = user.PrefixCD;
            this.PersonnelTypeCD = user.PersonnelTypeCD;
            this.FirstName = user.FirstName;
            this.MiddleName = user.MiddleName;
            this.LastName = user.LastName;
            this.DayOfBirth = user.DayOfBirth;
            this.Email = user.Email;
            this.PersonalEmail = user.PersonalEmail;
            this.IsDoctor = user.IsDoctor;

            CopyEntries(user.PersonnelIdentifiers);
            CopyEntries(user.PersonnelAdresses);
            CopyEntries(user.PersonnelAcademicPositions);
            CopyEntries(user.PersonnelPositions);
        }

        public void CopyPersonnelOccupationId(int personnelOccupationId)
        {
            this.PersonnelOccupationId = personnelOccupationId;
        }

        public string GetFirstAndLastName()
        {
            return this.FirstName + " " + this.LastName;
        }

        public string GetActiveOrganizationTimeZoneId()
        {
            return PersonnelConfig?.ActiveOrganization?.GetOrganizationTimeZoneId();
        }

        #region Edit Child entries


        public void CopyEntries(List<PersonnelIdentifier> upcomingEntries)
        {
            if (upcomingEntries != null)
            {
                DeleteExistingRemovedEntries(upcomingEntries);
                AddNewOrUpdateOldEntries(upcomingEntries);
            }
        }

        public void DeleteExistingRemovedEntries(List<PersonnelIdentifier> upcomingEntries)
        {
            foreach (var identifier in PersonnelIdentifiers)
            {
                var remainingIdentifier = upcomingEntries.Exists(x => x.PersonnelIdentifierId == identifier.PersonnelIdentifierId);
                if (!remainingIdentifier)
                {
                    identifier.Delete();
                }
            }
        }

        public void AddNewOrUpdateOldEntries(List<PersonnelIdentifier> upcomingEntries)
        {
            foreach (var identifier in upcomingEntries)
            {
                if (identifier.PersonnelIdentifierId == 0)
                {
                    PersonnelIdentifiers.Add(identifier);
                }
                else
                {
                    var dbIdentifier = PersonnelIdentifiers.Find(x => x.PersonnelIdentifierId == identifier.PersonnelIdentifierId && x.IsActive());
                    if (dbIdentifier != null)
                    {
                        dbIdentifier.Copy(identifier);
                    }
                }
            }
        }

        public void CopyEntries(List<PersonnelAddress> upcomingEntries)
        {
            if (upcomingEntries != null)
            {
                DeleteExistingRemovedEntries(upcomingEntries);
                AddNewOrUpdateOldEntries(upcomingEntries);
            }
        }

        public void DeleteExistingRemovedEntries(List<PersonnelAddress> upcomingEntries)
        {
            foreach (var address in PersonnelAdresses)
            {
                var remainingAddress = upcomingEntries.Exists(x => x.PersonnelAddressId == address.PersonnelAddressId);
                if (!remainingAddress)
                {
                    address.Delete();
                }
            }
        }

        public void AddNewOrUpdateOldEntries(List<PersonnelAddress> upcomingEntries)
        {
            foreach (var personnelAddress in upcomingEntries)
            {
                if (personnelAddress.PersonnelAddressId == 0)
                {
                    PersonnelAdresses.Add(personnelAddress);
                }
                else
                {
                    var dbPersonnelAddress = PersonnelAdresses.Find(x => x.PersonnelAddressId == personnelAddress.PersonnelAddressId && x.IsActive());
                    if (dbPersonnelAddress != null)
                    {
                        dbPersonnelAddress.Copy(personnelAddress);
                    }
                }
            }
        }

        public void CopyEntries(List<PersonnelAcademicPosition> upcomingEntries)
        {
            if (upcomingEntries != null)
            {
                DeleteExistingRemovedEntries(upcomingEntries);
                AddNewOrUpdateOldEntries(upcomingEntries);
            }
        }

        public void DeleteExistingRemovedEntries(List<PersonnelAcademicPosition> upcomingEntries)
        {
            foreach (var academicPosition in PersonnelAcademicPositions)
            {
                var remainingIdentifier = upcomingEntries.Exists(x => x.PersonnelAcademicPositionId == academicPosition.PersonnelAcademicPositionId);
                if (!remainingIdentifier)
                {
                    academicPosition.Delete();
                }
            }
        }

        public void AddNewOrUpdateOldEntries(List<PersonnelAcademicPosition> upcomingEntries)
        {
            foreach (var personelAcademicPosition in upcomingEntries)
            {
                if (personelAcademicPosition.PersonnelAcademicPositionId == 0)
                {
                    PersonnelAcademicPositions.Add(personelAcademicPosition);
                }
            }
        }

        public void CopyEntries(List<PersonnelPosition> upcomingEntries)
        {
            if (upcomingEntries != null)
            {
                DeleteExistingRemovedEntries(upcomingEntries);
                AddNewOrUpdateOldEntries(upcomingEntries);
            }
        }

        public void DeleteExistingRemovedEntries(List<PersonnelPosition> upcomingEntries)
        {
            foreach (PersonnelPosition personnelPosition in PersonnelPositions.Where(x => x.IsActive()))
            {
                bool remainingIdentifier = upcomingEntries.Exists(x => x.PositionCD == personnelPosition.PositionCD);
                if (!remainingIdentifier)
                {
                    personnelPosition.Delete();
                }
            }
        }

        public void AddNewOrUpdateOldEntries(List<PersonnelPosition> upcomingEntries)
        {
            foreach (PersonnelPosition upcomingPersonnelPosition in upcomingEntries)
            {
                bool exists = PersonnelPositions.Exists(x => x.PositionCD == upcomingPersonnelPosition.PositionCD && x.IsActive());
                if (!exists)
                {
                    PersonnelPositions.Add(upcomingPersonnelPosition);
                }
            }
        }

        #endregion /Edit Child entries

    }
}
