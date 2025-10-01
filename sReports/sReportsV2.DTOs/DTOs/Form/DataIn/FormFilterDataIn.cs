using sReportsV2.Common.Enums;
using sReportsV2.Common.Enums.DocumentPropertiesEnums;
using sReportsV2.DTOs.Common;
using System;
using System.Collections.Generic;

namespace sReportsV2.DTOs.Form
{
    public class FormFilterDataIn : Common.DataIn
    {
        public string Content { get; set; }
        public string Title { get; set; }
        public int ThesaurusId { get; set; }
        public bool ShowUserProjects { get; set; }
        public bool IsReadOnly { get; set; }
        public FormDefinitionState? State { get; set; }
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

        public void WorkOnlyWithValidForms()
        {
            this.HideInvalidDocuments = true;
        }
    }
}