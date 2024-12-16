using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.CodeAssociation.DataIn
{
    public class CodeAssociationFilterDataIn : Common.DataIn
    {
        public int CodeSetId { get; set; }
        public int ParentId { get; set; }
        public int? ChildId { get; set; }
        public string ActiveLanguage { get; set; }
        public bool IsChildToParent { get; set; }
        public bool IsReadOnly { get; set; }
    }
}
