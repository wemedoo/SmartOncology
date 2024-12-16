using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Common.Exceptions
{
    [Serializable()]
    public class DuplicateAliasException : Exception
    {
        public DuplicateAliasException()
        {
        }

        public DuplicateAliasException(string message)
            : base(message)
        {
        }

        public DuplicateAliasException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected DuplicateAliasException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
        {
            throw new NotImplementedException();
        }
    }
}
