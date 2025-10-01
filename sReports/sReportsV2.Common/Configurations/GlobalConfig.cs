using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using sReportsV2.Common.Constants;

namespace sReportsV2.Common.Configurations
{
    public static class GlobalConfig
    {
        private static IHttpContextAccessor _httpContextAccessor;

        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public static string GetTimeZoneId(string organizationTimeZoneId = null)
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
                        return timeZoneId;
                    }
                }
            }
            else if (!string.IsNullOrEmpty(organizationTimeZoneId))
            {
                return organizationTimeZoneId;
            }

            return DateTimeConstants.UTCTimeZone;
        }
    }
}
