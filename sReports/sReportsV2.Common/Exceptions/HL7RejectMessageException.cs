using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Common.Exceptions
{
    [Serializable()]
    public class HL7RejectMessageException : Exception
    {
        public HL7RejectMessageException(string message) : base(message)
        {
        }
    }
}
