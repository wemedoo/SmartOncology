using Microsoft.Extensions.Configuration;
using sReportsV2.Common.Constants;

namespace sReportsV2.Common.Extensions
{
    public static class ConfigurationExtension
    {
        public static bool IsSReportsRunning(this IConfiguration configuration)
        {
            return configuration["Instance"] == InstanceNames.SReports;
        }

        public static bool IsGlobalThesaurusRunning(this IConfiguration configuration)
        {
            return configuration["Instance"] == InstanceNames.ThesaurusGlobal;
        }
    }
}
