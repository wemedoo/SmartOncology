using sReportsV2.Common.Enums;
using System;
using System.Collections.Generic;
using System.Net;

namespace sReportsV2.DTOs.AdministrationApi.DataOut
{
    public class AdministrationApiDataOut
    {
        public int ApiRequestLogId { get; set; }
        public ApiRequestDirection ApiRequestDirection { get; set; }
        public DateTimeOffset RequestTimestamp { get; set; }
        public DateTimeOffset? ResponseTimestamp { get; set; }
        public string RequestPayload { get; set; }
        public string RequestUriAbsolutePath { get; set; }
        public string ResponsePayload { get; set; }
        public short? HttpStatusCode { get; set; }
        public string ApiName { get; set; }

        public string GetResponseCode(Dictionary<short, string> statusCodes)
        {
            if (statusCodes.TryGetValue(HttpStatusCode.Value, out string statusCode))
            {
                return statusCode;
            }
            else if (Enum.TryParse(HttpStatusCode.ToString(), out HttpStatusCode parsedHttpStatusCode))
            {
                return parsedHttpStatusCode.ToString();
            }
            else
            {
                return HttpStatusCode.ToString();
            }
        }
    }
}
