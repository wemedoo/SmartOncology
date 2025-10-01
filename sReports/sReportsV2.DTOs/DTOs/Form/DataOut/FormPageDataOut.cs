using sReportsV2.Common.CustomAttributes;
using sReportsV2.DTOs.CustomAttributes;
using sReportsV2.DTOs.Field.DataOut;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.DTOs.Form.DataOut
{
    public partial class FormPageDataOut
    {
        [DataProp]
        public string Id { get; set; }
        [DataProp]
        public string Title { get; set; }
        [DataProp]
        public bool IsVisible { get; set; }
        [DataProp]
        public string Description { get; set; }
        [DataProp]
        public int ThesaurusId { get; set; }
        [DataProp]
        public FormPageImageMapDataOut ImageMap { get; set; }
        [DataList]
        public List<List<FormFieldSetDataOut>> ListOfFieldSets { get; set; } = new List<List<FormFieldSetDataOut>>();
        [DataProp]
        public FormLayoutStyleDataOut LayoutStyle { get; set; }
    }
}