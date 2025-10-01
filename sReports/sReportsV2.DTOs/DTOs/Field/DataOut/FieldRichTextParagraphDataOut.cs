using Newtonsoft.Json;

namespace sReportsV2.DTOs.Field.DataOut
{
    public class FieldRichTextParagraphDataOut : FieldStringDataOut
    {
        [JsonIgnore]
        public override string PartialView { get; } = "~/Views/Form/Fields/FieldRichTextParagraph.cshtml";

        [JsonIgnore]
        public override string NestableView { get; } = "~/Views/Form/DragAndDrop/NestableFields/NestableRichTextParagraphField.cshtml";

        public override bool CanBeInDependencyFormula()
        {
            return false;
        }

        public override bool CanBeConnectedField()
        {
            return false;
        }
    }
}