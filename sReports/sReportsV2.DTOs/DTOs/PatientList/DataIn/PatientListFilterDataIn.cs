using sReportsV2.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.PatientList.DataIn
{
    public class PatientListFilterDataIn: Common.DataIn
    {
        public int PatientListId { get; set; }
        public string PatientListName { get; set; } 
        public int? PersonnelId { get; set; }
        public int? OccupationCD { get; set; }
        public int? OrganizationId { get; set; }
        public int? PersonnelTeamId { get; set; }
        public bool? LoadSelectedPersonnel { get; set; }
        public bool? ListWithSelectedPatients { get; set; }
    }
}
