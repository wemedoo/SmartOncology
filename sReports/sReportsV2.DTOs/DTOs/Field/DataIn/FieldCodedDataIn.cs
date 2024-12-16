using sReportsV2.DTOs.Field.DataIn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.Field.DataIn
{
    public class FieldCodedDataIn : FieldStringDataIn
    {
        public int CodeSetId { get; set; }
    }
}
