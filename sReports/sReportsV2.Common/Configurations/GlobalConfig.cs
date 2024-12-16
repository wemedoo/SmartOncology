using System;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace sReportsV2.Common.Configurations
{
    public static class GlobalConfig
    {
        private static IHttpContextAccessor _httpContextAccessor;

        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public static string GetUserOffset(string organizationTimeZone = null, bool isOffsetForFormInstance = false)
        {
            var httpContext = _httpContextAccessor?.HttpContext;
            var userData = httpContext?.Session?.GetString("userData");

            if (userData != null)
            {
                dynamic userDataObject = JsonConvert.DeserializeObject(userData);

                if (userDataObject != null && userDataObject.OrganizationTimeZone != null)
                {
                    string timeZoneId = userDataObject.OrganizationTimeZone.ToString();
                    if (!string.IsNullOrEmpty(timeZoneId))
                    {
                        return GetOffsetValue(timeZoneId, isOffsetForFormInstance);
                    }
                }
            }
            else if (!string.IsNullOrEmpty(organizationTimeZone))
            {
                return GetOffsetValue(organizationTimeZone, isOffsetForFormInstance);
            }

            return "00:00";
        }

        private static string GetOffsetValue(string timeZoneId, bool isOffsetForFormInstance)
        {
            TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            TimeSpan offset = timeZone.BaseUtcOffset;
            if (isOffsetForFormInstance)
                return (offset >= TimeSpan.Zero ? "+" : "-") + offset.ToString("hh\\:mm");
            else
            {
                if (offset == TimeSpan.Zero)
                    return offset.ToString("hh\\:mm");
                return (offset > TimeSpan.Zero ? "" : "-") + offset.ToString("hh\\:mm");
            }
        }
    }
}
