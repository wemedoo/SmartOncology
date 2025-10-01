using sReportsV2.Common.CustomAttributes;
using sReportsV2.DTOs.CustomAttributes;
using System.Collections.Generic;

namespace sReportsV2.DTOs.Form.DataOut
{
    public partial class FormChapterDataOut
    {
        [DataProp]
        public string Id { get; set; }
        [DataProp]
        public string Title { get; set; }
        [DataProp]
        public string Description { get; set; }
        [DataProp]
        public int ThesaurusId { get; set; }
        [DataProp]
        public bool IsReadonly { get; set; }
        [DataList]
        public List<FormPageDataOut> Pages { get; set; } = new List<FormPageDataOut>();

        public string GetHtmlId()
        {
            return $"chapter-{Id}";
        }
    }
}