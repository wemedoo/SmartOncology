
namespace sReportsV2.DTOs.DTOs.QueryManagement.DataIn
{
    public class QueryFilterDataIn : Common.DataIn
    {
        public int QueryId { get; set; }
        public string FieldId { get; set; }
        public string FieldLabel { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Comment { get; set; }
        public int? ReasonCD { get; set; }
        public int? StatusCD { get; set; }
        public string ActiveLanguage { get; set; }
    }
}
