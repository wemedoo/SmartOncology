using System.Collections.Generic;

namespace sReportsV2.DTOs.DTOs.QueryManagement.DataIn
{
    public class QueryDataIn
    {
        public int QueryId { get; set; }
        public string FieldId { get; set; }
        public string FormInstanceId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int? LastUpdateById { get; set; }
        public int ReasonCD { get; set; }
        public int StatusCD { get; set; }
        public string Comment { get; set; }
        public List<QueryHistoryDataIn> History { get; set; } = new List<QueryHistoryDataIn>();
    }
}
