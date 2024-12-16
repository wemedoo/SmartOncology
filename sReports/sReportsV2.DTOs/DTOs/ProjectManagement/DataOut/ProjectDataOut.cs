using sReportsV2.DTOs.CodeEntry.DataOut;
using sReportsV2.DTOs.Common.DataOut;
using sReportsV2.DTOs.DTOs.TrialManagement;
using System;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.DTOs.DTOs.ProjectManagement.DataOut
{
    public class ProjectDataOut
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public int? ProjectTypeCD { get; set; }
        public List<UserDataOut> Personnels { get; set; } = new List<UserDataOut>();
        public ClinicalTrialDataOut ClinicalTrial { get; set; }
        public DateTimeOffset? ProjectStartDateTime { get; set; }
        public DateTimeOffset? ProjectEndDateTime { get; set; }

        public string GetProjectType(string activeLanguage, List<CodeDataOut> projectType)
        {
            CodeDataOut projectTypeCode = projectType.Find(x => x.Id == ProjectTypeCD);
            return projectTypeCode != null ? projectTypeCode.Thesaurus.GetPreferredTermByTranslationOrDefault(activeLanguage) : string.Empty;
        }
    }
}
