using Newtonsoft.Json;
using sReportsV2.Common.CustomAttributes;

namespace sReportsV2.DTOs.Field.DataOut
{
    public class FieldLinkDataOut : FieldStringDataOut
    {
        [JsonIgnore]
        public override string PartialView { get; } = "~/Views/Form/Fields/FieldLink.cshtml";

        [JsonIgnore]
        public override string NestableView { get; } = "~/Views/Form/DragAndDrop/NestableFields/NestableLinkField.cshtml";

        [DataProp]
        public string Link { get; set; }

        public override string GetLabel()
        {
            return this.Link;
        }

        public override string GetChildFieldInstanceCssSelector(string fieldInstanceRepetitionId)
        {
            return $"#{fieldInstanceRepetitionId}";
        }
    }
}