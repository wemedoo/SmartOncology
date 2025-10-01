using sReportsV2.DTOs.Field.DataOut;

namespace sReportsV2.DTOs.DTOs.Prompt.DataOut
{
    public class PromptDetailDataOut
    {
        public string Prompt { get; set; }
        public FieldDataOut Field { get; set; }
        public string FormName { get; set; }
        public string GetPromptItemName()
        {
            return Field != null ? Field.Label : FormName;
        }
    }
}
