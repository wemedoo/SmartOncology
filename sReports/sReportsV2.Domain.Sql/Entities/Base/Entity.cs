using sReportsV2.Common.Enums;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Domain.Sql.Entities.User;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sReportsV2.Domain.Sql.EntitiesBase
{
    public class Entity
    {
        [Timestamp]
        public byte[] RowVersion { get; set; }
        public DateTimeOffset EntryDatetime { get; set; }
        public DateTimeOffset? LastUpdate { get; set; }
        [NotMapped]
        public DateTimeOffset StartDateTime { get; set; }
        [NotMapped]
        public DateTimeOffset EndDateTime { get; set; }
        public int? CreatedById { get; set; }  // Cannot rename it to UserId -> it would conflict with User Entity
        [ForeignKey("CreatedById")]
        public Personnel CreatedBy { get; set; }
        public DateTimeOffset ActiveFrom { get; set; }
        public DateTimeOffset ActiveTo { get; set; }
        public int? EntityStateCD { get; set; }
        [ForeignKey("EntityStateCD")]
        public virtual Code EntityState { get; set; }

        public void CopyRowVersion(Entity entity)
        {
            this.RowVersion = entity.RowVersion;
        }

        public void SetLastUpdate(string organizationTimeZone = null)
        {
            this.LastUpdate = DateTimeOffset.UtcNow.ConvertToOrganizationTimeZone(organizationTimeZone);
        }

        public void SetEntryDatetime(string organizationTimeZone = null)
        {
            this.EntryDatetime = DateTimeOffset.UtcNow.ConvertToOrganizationTimeZone(organizationTimeZone);
            this.ActiveFrom = DateTimeOffset.UtcNow.ConvertToOrganizationTimeZone(organizationTimeZone);
            this.ActiveTo = DateTimeOffset.MaxValue;
        }

        public void SetActiveFromAndToDatetime(string organizationTimeZone = null)
        {
            this.ActiveFrom = (this.ActiveFrom.Equals(DateTimeOffset.MinValue))
                ? DateTimeOffset.UtcNow.ConvertToOrganizationTimeZone(organizationTimeZone)
                : this.ActiveFrom;

            this.ActiveTo = (this.ActiveTo.Equals(DateTimeOffset.MinValue))
                ? DateTimeOffset.MaxValue
                : this.ActiveTo;

        }

        public void SetActiveFromAndTo(string organizationTimeZone = null)
        {
            this.SetLastUpdate();
            this.ActiveFrom = DateTimeOffset.UtcNow.ConvertToOrganizationTimeZone(organizationTimeZone);
            this.ActiveTo = DateTimeOffset.MaxValue;
        }

        public virtual void Delete(DateTimeOffset? activeTo = null, bool setLastUpdateProperty = true, string organizationTimeZone = null)
        {
            SetActiveTo(activeTo, organizationTimeZone);
            if (setLastUpdateProperty)
            {
                SetLastUpdate(organizationTimeZone);
            }
        }

        public Entity()
        {
            //this.EntityStateCD = (int)EntityStateCode.Active;
            SetEntryDatetime();
        }

        protected Entity(bool isEntityAddedByMigrations)
        {
            SetEntryDatetime();
        }

        protected Entity(int? createdById, string organizationTimeZone = null)
        {
            this.CreatedById = createdById;
            this.SetEntryDatetime(organizationTimeZone);
        }

        public void OverrideDates(DateTimeOffset dateTime, bool overrideEntryDatetime = false)
        {
            if (overrideEntryDatetime)
            {
                EntryDatetime = dateTime;
            }
            LastUpdate = dateTime;
        }

        public bool IsDeleted()
        {
            return EntityStateCD == (int)EntityStateCode.Deleted || ActiveFrom > DateTimeOffset.UtcNow.ConvertToOrganizationTimeZone() || ActiveTo < DateTimeOffset.UtcNow.ConvertToOrganizationTimeZone();
        }

        public bool IsActive()
        {
            return !IsDeleted();
        }

        private void SetActiveTo(DateTimeOffset? activeTo = null, string organizationTimeZone = null)
        {
            this.ActiveTo = activeTo ?? DateTimeOffset.UtcNow.ConvertToOrganizationTimeZone(organizationTimeZone);
        }
    }
}
