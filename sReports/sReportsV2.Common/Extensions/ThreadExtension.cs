using sReportsV2.Common.Constants;
using System.Globalization;
using System.Threading;

namespace sReportsV2.Common.Extensions
{
    public static class ThreadExtension
    {
        public static void UpdateLanguage(this Thread currentThread, string activeLanguage)
        {
            currentThread.CurrentCulture = new CultureInfo(LanguageConstants.EN);
            currentThread.CurrentUICulture = new CultureInfo(activeLanguage);
        }
    }
}
