using sReportsV2.DTOs.Field.DataOut;

namespace sReportsV2.DTOs.DTOs.Prompt.DataOut
{
    public class PromptFieldDataOut
    {
        public string FieldId { get; set; }
        public FieldDataOut Field { get; set; }
        public string Prompt { get; set; }
    }
}
