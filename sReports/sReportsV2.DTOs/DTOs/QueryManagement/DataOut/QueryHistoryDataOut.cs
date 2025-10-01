using sReportsV2.DTOs.Common.DataOut;
using System;

namespace sReportsV2.DTOs.DTOs.QueryManagement.DataOut
{
    public class QueryHistoryDataOut
    {
        public int QueryHistoryId { get; set; }
        public int QueryId { get; set; }
        public string Comment { get; set; }
        public UserDataOut LastUpdateBy { get; set; }
        public DateTimeOffset? LastUpdate { get; set; }
        public int StatusCD { get; set; }
    }
}
