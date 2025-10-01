using sReportsV2.DTOs.CodeEntry.DataOut;
using sReportsV2.DTOs.Common.DataOut;
using sReportsV2.DTOs.Field.DataOut;
using System;
using System.Collections.Generic;

namespace sReportsV2.DTOs.DTOs.QueryManagement.DataOut
{
    public class QueryDataOut
    {
        public int QueryId { get; set; }
        public string FieldId { get; set; }
        public string FormInstanceId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Comment { get; set; }
        public int? LastUpdateById { get; set; }
        public int ReasonCD { get; set; }
        public int StatusCD { get; set; }
        public DateTimeOffset EntryDatetime { get; set; }
        public DateTimeOffset? LastUpdate { get; set; }
        public UserDataOut CreatedBy { get; set; }
        public UserDataOut LastUpdateBy { get; set; }
        public FieldDataOut Field { get; set; }
        public List<QueryHistoryDataOut> History { get; set; } = new List<QueryHistoryDataOut>();

        public string GetQueryReason(string activeLanguage, List<CodeDataOut> types)
        {
            CodeDataOut queryReasonCode = types.Find(x => x.Id == ReasonCD);
            return types != null ? queryReasonCode.Thesaurus.GetPreferredTermByTranslationOrDefault(activeLanguage) : string.Empty;
        }

        public string GetQueryStatus(string activeLanguage, List<CodeDataOut> statuses)
        {
            CodeDataOut queryStatusCode = statuses.Find(x => x.Id == StatusCD);
            return statuses != null ? queryStatusCode.Thesaurus.GetPreferredTermByTranslationOrDefault(activeLanguage) : string.Empty;
        }
    }
}
