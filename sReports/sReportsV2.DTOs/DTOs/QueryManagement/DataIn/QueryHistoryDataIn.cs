using System;

namespace sReportsV2.DTOs.DTOs.QueryManagement.DataIn
{
    public class QueryHistoryDataIn
    {
        public int QueryHistoryId { get; set; }
        public int QueryId { get; set; }
        public string Comment { get; set; }
        public DateTimeOffset? LastUpdate { get; set; }
        public int StatusCD { get; set; }
    }
}
