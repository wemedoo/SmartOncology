using System;
using System.Text;

namespace sReportsV2.Common.Helpers
{
    public static class ExceptionHelper
    {
        public static string GetExceptionStackMessages(Exception exception)
        {
            StringBuilder stringBuilder = new StringBuilder(exception.Message ?? string.Empty);

            Exception currentException = exception?.InnerException;
            while (currentException != null)
            {
                stringBuilder.Append($" --> {currentException.Message}");
                currentException = currentException.InnerException;
            }

            return stringBuilder.ToString();
        }
    }
}
