using sReportsV2.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.ClinicalTrial
{
    public class TrialManagementFilter : EntityFilter
    {
        public int? ClinicalTrialId { get; set; }
        public string ClinicalTrialTitle { get; set; }
        public string ClinicalTrialAcronym { get; set; }
        public string ClinicalTrialSponsorName { get; set; }
        public int? ClinicalTrialRecruitmentStatusCD { get; set; }
        public bool ShowArchived { get; set; }
        public bool ShowUnarchived { get; set; }

        public bool ShowAddedPersonnels { get; set; } = false;
        public int? PersonnelId { get; set; }
        public int? OccupationCD { get; set; }
        public int? OrganizationId { get; set; }
        public int? PersonnelTeamId { get; set; }
    }
}
