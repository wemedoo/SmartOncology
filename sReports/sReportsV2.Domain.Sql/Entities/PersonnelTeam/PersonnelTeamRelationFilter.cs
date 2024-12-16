using sReportsV2.Common.Entities;

namespace sReportsV2.Domain.Sql.Entities.PersonnelTeamEntities
{
    public class PersonnelTeamRelationFilter : EntityFilter
    {
        public int PersonnelTeamId { get; set; }
        public int? RelationTypeCD { get; set; }
        public int? PersonnelId { get; set; }
    }
}
