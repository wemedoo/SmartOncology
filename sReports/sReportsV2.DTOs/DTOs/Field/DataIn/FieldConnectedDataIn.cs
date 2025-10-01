using sReportsV2.DTOs.Field.DataIn;
using System.Collections.Generic;

namespace sReportsV2.DTOs.DTOs.Field.DataIn
{
    public class FieldConnectedDataIn : FieldStringDataIn
    {
        public List<string> ConnectedFieldIds { get; set; }
    }
}
