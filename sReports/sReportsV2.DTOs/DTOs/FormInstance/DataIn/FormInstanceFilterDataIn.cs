using sReportsV2.DTOs.DTOs.Field.DataIn.Custom;
using sReportsV2.DTOs.Field.DataIn;
using System.Collections.Generic;

namespace sReportsV2.DTOs.FormInstance
{
    public class FormInstanceFilterDataIn : Common.DataIn
    {
        public string FormId { get; set; }
        public string VersionId { get; set; }
        public string FormInstanceId { get; set; }
        public string Title { get; set; }
        public int ThesaurusId { get; set; }
        public bool IsSimplifiedLayout { get; set; }
        public string Language { get; set; }
        public string Content { get; set; }
        public int? ProjectId { get; set; }
        public bool ShowUserProjects { get; set; }
        public List<int> UserIds { get; set; } = new List<int> { };
        public List<int> PatientIds { get; set; } = new List<int> { };
        public List<CustomFieldFilterDataIn> CustomFieldFiltersDataIn { get; set; } = new List<CustomFieldFilterDataIn> { };
        public string FieldFiltersOverallOperator { get; set; }
        public List<FieldDataIn> CustomHeaderFields { get; set; }
        public string ActiveChapterId { get; set; }
        public int? ActivePageLeftScroll { get; set; }
        public string ActivePageId { get; set; }
        public int OrganizationId { get; set; }
    }
}