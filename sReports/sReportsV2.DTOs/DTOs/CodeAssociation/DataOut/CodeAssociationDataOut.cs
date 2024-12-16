using sReportsV2.DTOs.CodeEntry.DataOut;
using sReportsV2.DTOs.ThesaurusEntry.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.CodeAssociation.DataOut
{
    public class CodeAssociationDataOut
    {
        public int CodeAssociationId { get; set; }
        public int ParentId { get; set; }
        public int? ChildId { get; set; }
        public CodeDataOut Parent { get; set; }
        public CodeDataOut Child { get; set; }
    }
}
