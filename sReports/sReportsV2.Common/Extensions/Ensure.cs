using System;

namespace sReportsV2.Common.Extensions
{
    public static class Ensure
    {
        public static T IsNotNull<T>(T value, string paramName) where T : class
        {
            if (value == null)
            {
                throw new ArgumentNullException(paramName, "Value cannot be null");
            }
            return value;
        }
        
        public static string IsNotNullOrWhiteSpace(string value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(paramName, "Value cannot be null or white space.");
            }
            return value;
        }
    }
}
