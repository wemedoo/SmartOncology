using System;
using System.Runtime.Serialization;

namespace sReportsV2.Common.Exceptions
{
    [Serializable()]
    public class InsertAliasException : Exception
    {
        public InsertAliasException()
        {
        }

        public InsertAliasException(string message)
            : base(message)
        {
        }

        public InsertAliasException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected InsertAliasException(SerializationInfo serializationInfo, StreamingContext streamingContext)
        {
            throw new NotImplementedException();
        }
    }
}
