using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.PersonnelTeam.DataIn
{
    public class PersonnelTeamRelationDataIn
    {
        public int PersonnelTeamRelationId { get; set; }
        public int? RelationTypeCD { get; set; }
        public int PersonnelTeamId { get; set; }
        public int UserId { get; set; }
    }
}
