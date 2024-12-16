using sReportsV2.DTOs.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace sReportsV2.DTOs.Form.DataIn
{
    public class FormChapterDataIn : IViewModeDataIn
    {
        public string Id { get; set; }
        public string Title { get; set; }
        [DataType(DataType.Html)]
        public string Description { get; set; }
        public string ThesaurusId { get; set; }
        public bool IsReadonly { get; set; }
        public List<FormPageDataIn> Pages { get; set; } = new List<FormPageDataIn>();
        public bool IsReadOnlyViewMode { get; set; }
    }
}