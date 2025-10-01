using System;

namespace sReportsV2.Common.Exceptions
{
    [Serializable()]
    public class UserAdministrationException : Exception
    {
        public int HttpStatusCode { get; set; }
        public UserAdministrationException()
        {
        }

        public UserAdministrationException(int httpStatusCode, string message) : base(message)
        {
            this.HttpStatusCode = httpStatusCode;
        }

        public UserAdministrationException(int httpStatusCode, string message, Exception innerException) : base(message, innerException)
        {
            this.HttpStatusCode = httpStatusCode;
        }
    }
}
