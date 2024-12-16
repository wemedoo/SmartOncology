using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.PersonnelTeam.DataIn
{
    public class PersonnelTeamDataIn
    {
        public int PersonnelTeamId { get; set; }
        public string TeamName { get; set; }
        public int? TeamType { get; set; }
        public List<PersonnelTeamRelationDataIn> PersonnelTeamRelations { get; set; } = new List<PersonnelTeamRelationDataIn>();
        public List<PersonnelTeamOrganizationRelationDataIn> PersonnelTeamOrganizationRelations{ get; set; } = new List<PersonnelTeamOrganizationRelationDataIn>();
    }
}
