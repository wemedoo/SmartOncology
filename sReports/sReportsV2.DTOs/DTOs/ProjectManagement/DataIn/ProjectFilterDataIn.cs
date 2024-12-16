using System;

namespace sReportsV2.DTOs.DTOs.ProjectManagement.DataIn
{
    public class ProjectFilterDataIn : Common.DataIn
    {
        public int? ProjectId { get; set; }
        public string ProjectName { get; set; }
        public int? ProjectType { get; set; }
        public DateTimeOffset? ProjectStartDateTime { get; set; }
        public DateTimeOffset? ProjectEndDateTime { get; set; }
        public int? ActiveOrganizationId { get; set; }
        public bool IsReadOnly { get; set; }

        public bool ShowAddedPersonnels { get; set; } = false;
        public int? PersonnelId { get; set; }
        public int? OccupationCD { get; set; }
        public int? OrganizationId { get; set; }
        public int? PersonnelTeamId { get; set; }
    }
}
