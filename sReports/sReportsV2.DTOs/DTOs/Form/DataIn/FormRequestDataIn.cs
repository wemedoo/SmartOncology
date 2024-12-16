using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.Form.DataIn
{
    public class FormRequestDataIn
    {
        public int ThesaurusId { get; set; }
        public string VersionId { get; set; }
        public string ActiveTab { get; set; } 
        public string TaggedCommentId { get; set; }
    }
}
