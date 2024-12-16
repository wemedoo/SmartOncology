using sReportsV2.Common.Entities;

namespace sReportsV2.Domain.Sql.Entities.PersonnelTeamEntities
{
    public class PersonnelTeamFilter : EntityFilter
    {
        public int OrganizationId { get; set; }
        public string TeamName { get; set; }
        public int TeamType { get; set; }
    }
}
