using System.ComponentModel.DataAnnotations;

namespace sReportsV2.DTOs.Form.DataIn
{
    public class FormHelpDataIn
    {
        public string Title { get; set; }

        [DataType(DataType.Html)]
        public string Content { get; set; }
    }
}