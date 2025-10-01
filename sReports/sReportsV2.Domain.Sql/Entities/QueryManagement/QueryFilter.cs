using sReportsV2.Common.Entities;

namespace sReportsV2.Domain.Sql.Entities.QueryManagement
{
    public class QueryFilter : EntityFilter
    {
        public string FieldId { get; set; }
        public string FieldLabel { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int? ReasonCD { get; set; }
        public int? StatusCD { get; set; }
        public string ActiveLanguage { get; set; }
    }
}
