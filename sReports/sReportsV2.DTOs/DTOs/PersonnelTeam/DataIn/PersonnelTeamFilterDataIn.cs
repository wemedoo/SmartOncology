using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.PersonnelTeam.DataIn
{
    public class PersonnelTeamFilterDataIn : Common.DataIn
    {
        public int OrganizationId { get; set; }
        public string TeamName { get; set; }
        public int? TeamType { get; set; }
    }
}
