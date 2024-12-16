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

        public UserAdministrationException(string message) : base(message)
        {
        }

        public UserAdministrationException(int httpStatusCode, string message) : this(message)
        {
            this.HttpStatusCode = httpStatusCode;
        }
    }
}
