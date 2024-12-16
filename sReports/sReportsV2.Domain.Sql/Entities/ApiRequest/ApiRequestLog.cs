using sReportsV2.Common.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Net;
using sReportsV2.Common.Entities;
using sReportsV2.Common.Extensions;

namespace sReportsV2.Domain.Sql.Entities.ApiRequest
{
    public class ApiRequestLog
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int ApiRequestLogId { get; set; }
        public ApiRequestDirection ApiRequestDirection { get; set; }
        public DateTimeOffset RequestTimestamp { get; set; }
        public string RequestPayload { get; set; }
        public string RequestUriAbsolutePath { get; set; }
        public DateTimeOffset? ResponseTimestamp { get; set; }
        public string ResponsePayload { get; set; }
        public short? HttpStatusCode { get; set; }
        public string ApiName { get; set; }

        public ApiRequestLog()
        {
        }

        public ApiRequestLog(RestRequestData restRequestData, ApiRequestDirection apiRequestDirection, string requestBodySerialized)
        {
            ApiRequestDirection = apiRequestDirection;
            ApiName = restRequestData.ApiName;
            RequestPayload = requestBodySerialized;
            RequestTimestamp = DateTimeOffset.UtcNow.ConvertToOrganizationTimeZone();
            RequestUriAbsolutePath = $"{restRequestData.BaseUrl}/{restRequestData.Endpoint}";
        }

        public void LogResponseData(HttpStatusCode httpStatusCode, string content)
        {
            HttpStatusCode = (short)httpStatusCode;
            ResponsePayload = content;
            ResponseTimestamp = DateTimeOffset.UtcNow.ConvertToOrganizationTimeZone();
        }
    }
}
