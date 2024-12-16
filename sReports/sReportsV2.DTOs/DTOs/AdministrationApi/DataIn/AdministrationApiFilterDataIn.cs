using sReportsV2.Common.Enums;
using System;

namespace sReportsV2.DTOs.AdministrationApi.DataIn
{
    public class AdministrationApiFilterDataIn : Common.DataIn
    {
        public ApiRequestDirection? ApiRequestDirection { get; set; }
        public DateTimeOffset? RequestTimestampFrom { get; set; }
        public DateTimeOffset? RequestTimestampTo { get; set; }
        public string RequestContains { get; set; }
        public short? HttpStatusCode { get; set; }
        public string ApiName { get; set; }
        public string ScreeningNumber { get; set; }
        public bool ShowOnlyUnsuccessful { get; set; }
    }
}
