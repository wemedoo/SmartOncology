using sReportsV2.Common.Enums.DocumentPropertiesEnums;

namespace sReportsV2.Domain.Entities.DocumentProperties
{
    public class DocumentClinicalContext
    {
        public ClinicalContext? ClinicalContext { get; set; }
        public FollowUp? FollowUp { get; set; }
    }
}
