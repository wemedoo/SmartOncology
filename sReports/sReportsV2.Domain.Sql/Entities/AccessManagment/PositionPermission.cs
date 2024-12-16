using sReportsV2.Domain.Sql.Entities.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using sReportsV2.Common.Extensions;

namespace sReportsV2.Domain.Sql.Entities.AccessManagment
{
    public class PositionPermission : EntitiesBase.Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int PositionPermissionId { get; set; }

        public int? PositionCD { get; set; }
        [ForeignKey("PositionCD")]
        public virtual Code Position { get; set; }

        public int? PermissionModuleId { get; set; }
        [ForeignKey("PermissionModuleId")]
        public virtual PermissionModule PermissionModule { get; set; }

        public void UpdatePermission(bool isDeletedNewValue)
        {
            var activeTo = isDeletedNewValue ? DateTimeOffset.UtcNow.ConvertToOrganizationTimeZone() : DateTimeOffset.MaxValue;
            if (this.ActiveTo != activeTo)
            {
                this.Delete(activeTo);
            }
        }
    }
}
