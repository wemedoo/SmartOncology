using sReportsV2.Common.Extensions;
using System.Resources;

namespace sReportsV2.Cache.Extensions
{
    public static class LocalizationExtension
    {
        public static string GetStringOrDefault(this ResourceManager resourceManager, string name)
        {
            resourceManager = Ensure.IsNotNull(resourceManager, nameof(resourceManager));
            return string.IsNullOrEmpty(name) ? string.Empty : resourceManager.GetString(name) ?? name;
        }
    }
}
