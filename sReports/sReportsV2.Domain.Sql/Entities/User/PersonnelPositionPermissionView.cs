using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sReportsV2.Domain.Sql.Entities.User
{
    public class PersonnelPositionPermissionView
    {
        [Key]
        public Guid Id { get; set; }
        public int PersonnelPositionId { get; set; }
        public int PersonnelId { get; set; }
        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
        public int PermissionId { get; set; }
        public string PermissionName { get; set; }
        public int PositionCD { get; set; }
    }
}
