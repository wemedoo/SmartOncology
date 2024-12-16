using System;

namespace sReportsV2.Common.Exceptions
{
    public class TabularExportException : Exception
    {
        public TabularExportException(string message = null) : base(message)
        {
        }

        public TabularExportException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
