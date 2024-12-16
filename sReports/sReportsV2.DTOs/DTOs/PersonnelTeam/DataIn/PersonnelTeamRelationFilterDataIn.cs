using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.PersonnelTeam.DataIn
{
    public class PersonnelTeamRelationFilterDataIn : Common.DataIn
    {
        public int PersonnelTeamId { get; set; }
        public int? RelationTypeCD { get; set; }
        public int? UserId { get; set; }
    }
}
