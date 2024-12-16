using sReportsV2.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.PatientList
{
    public class PatientListFilter: EntityFilter
    {
        public int PatientListId { get; set; }
        public int? PersonnelId { get; set; }
        public int? OccupationCD { get; set; }
        public int? OrganizationId { get; set; }
        public int? PersonnelTeamId { get; set; }
        public bool? LoadSelectedPersonnel { get; set; }
        public bool? ListWithSelectedPatients { get; set; }
    }
}
