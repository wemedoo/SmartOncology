using Newtonsoft.Json;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.DTOs.Field.DataOut.Custom.Action;

namespace sReportsV2.DTOs.Field.DataOut.Custom
{
    public class CustomFieldButtonDataOut : FieldDataOut
    {
        [JsonIgnore]
        public override string PartialView { get; } = "~/Views/Form/Fields/Custom/FieldCustomButton.cshtml";
        public override string NestableView { get; } = "~/Views/Form/DragAndDrop/NestableFields/NestableCustomButton.cshtml";


        [DataProp]
        public CustomActionDataOut CustomAction { get; set; }
    }
}