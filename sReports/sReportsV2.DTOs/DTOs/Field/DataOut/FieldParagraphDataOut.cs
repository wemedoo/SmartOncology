using Newtonsoft.Json;
using sReportsV2.Common.CustomAttributes;

namespace sReportsV2.DTOs.Field.DataOut
{
    public class FieldParagraphDataOut : FieldStringDataOut
    {
        [JsonIgnore]
        public override string PartialView { get; } = "~/Views/Form/Fields/FieldParagraph.cshtml";

        [JsonIgnore]
        public override string NestableView { get; } = "~/Views/Form/DragAndDrop/NestableFields/NestableParagraphField.cshtml";

        [DataProp]
        public string Paragraph { get; set; }

        public override string GetLabel()
        {
            return this.Paragraph;
        }

        public override string GetChildFieldInstanceCssSelector(string fieldInstanceRepetitionId)
        {
            return $"#{fieldInstanceRepetitionId}";
        }
    }
}