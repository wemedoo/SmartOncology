using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.Form.DataOut;
using System.Collections.Generic;

namespace sReportsV2.DTOs.DTOs.Prompt.DataOut
{
    public class PromptDataOut
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string CurrentVersionId { get; set; }
        public bool LatestVersion { get; set; }
        public FormDataOut Form { get; set; }
        public List<VersionDTO> Versions { get; set; }
    }
}
