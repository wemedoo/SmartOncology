using sReportsV2.Common.Enums;
using sReportsV2.Common.Enums.DocumentPropertiesEnums;
using sReportsV2.Common.Entities;

namespace sReportsV2.Domain.Entities.Form
{
    public class FormFilterData : EntityFilter
    {
        public string Content { get; set; }
        public string Title { get; set; }
        public int ThesaurusId { get; set; }
        public FormDefinitionState? State { get; set; }
        public int OrganizationId { get; set; }
        public string ActiveLanguage { get; set; }
        public Class? Classes { get; set; }
        public string ClassesOtherValue { get; set; }
        public GeneralPurpose? GeneralPurpose { get; set; }
        public ContextDependent? ContextDependent { get; set; }
        public DocumentExplicitPurpose? ExplicitPurpose { get; set; }
        public ScopeOfValidity? ScopeOfValidity { get; set; }
        public int? ClinicalDomain { get; set; }
        public ClinicalContext? ClinicalContext { get; set; }
        public FollowUp? FollowUp { get; set; }
        public AdministrativeContext? AdministrativeContext { get; set; }
        public DateTime? DateTimeTo { get; set; }
        public DateTime? DateTimeFrom { get; set; }
        public List<string> FormStates { get; set; } = new List<string>();
        public List<string> Ids { get; set; } = new List<string>();
        public bool HideInvalidDocuments { get; set; }
    }
}
