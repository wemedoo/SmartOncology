using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.PersonnelTeam.DataOut
{
    public class PersonnelTeamOrganizationRelationDataOut
    {
        public int PersonnelTeamOrganizationRelationId { get; set; }
        public int PersonnelTeamId { get; set; }
        public int? RelationTypeCD { get; set; }
    }
}
