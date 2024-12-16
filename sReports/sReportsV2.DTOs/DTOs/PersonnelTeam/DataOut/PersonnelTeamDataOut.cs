using sReportsV2.DTOs.Common.DataOut;
using sReportsV2.DTOs.CodeEntry.DataOut;
using System.Collections.Generic;

namespace sReportsV2.DTOs.DTOs.PersonnelTeam.DataOut
{
    public class PersonnelTeamDataOut
    {
        public int PersonnelTeamId { get; set; }
        public int OrganizationId { get; set; }
        public string Name { get; set; }
        public CodeDataOut Type { get; set; }
        public bool Active { get; set; }
        public List<PersonnelTeamRelationDataOut> PersonnelTeamRelations { get; set; }
    }
}
