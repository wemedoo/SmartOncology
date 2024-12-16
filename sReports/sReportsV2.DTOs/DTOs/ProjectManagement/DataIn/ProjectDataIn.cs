using sReportsV2.DTOs.DTOs.TrialManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.ProjectManagement.DataIn
{
    public class ProjectDataIn
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public int? ProjectTypeCD { get; set; }
        public DateTimeOffset? ProjectStartDateTime { get; set; }
        public DateTimeOffset? ProjectEndDateTime { get; set; }
        public ClinicalTrialDataIn ClinicalTrial { get; set; }
    }
}
