using sReportsV2.Common.Entities;
using sReportsV2.Domain.Entities.CustomFieldFilters;
using sReportsV2.Domain.Entities.FieldEntity;
using System.Collections.Generic;

namespace sReportsV2.Domain.Entities.FormInstance
{
    public class FormInstanceFilterData : EntityFilter
    {
        public string FormId { get; set; }
        public string FormInstanceId { get; set; }
        public string Title { get; set; }
        public int ThesaurusId { get; set; }
        public string VersionId { get; set; }
        public string Content { get; set; }
        public int? ProjectId { get; set; }
        public List<int> UserIds { get; set; } = new List<int> { }; 
        public List<int> PatientIds { get; set; } = new List<int> { };
        public List<CustomFieldFilterData> CustomFieldFiltersData { get; set; } = new List<CustomFieldFilterData> { };
        public string FieldFiltersOverallOperator { get; set; }
        public List<Field> CustomHeaderFields { get; set; }
        public int OrganizationId { get; set; }
        public Dictionary<string, string> Languages { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> SpecialValues { get; set; } = new Dictionary<string, string>();
        public List<int> PersonnelProjectsIds { get; set; }
    }
}
