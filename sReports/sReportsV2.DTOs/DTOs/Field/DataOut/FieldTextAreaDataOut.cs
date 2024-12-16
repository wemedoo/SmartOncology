using Newtonsoft.Json;
using sReportsV2.Common.CustomAttributes;

namespace sReportsV2.DTOs.Field.DataOut
{
    public class FieldTextAreaDataOut : FieldStringDataOut
    {
        [JsonIgnore]
        public override string PartialView { get; } = "~/Views/Form/Fields/FieldTextarea.cshtml";

        [JsonIgnore]
        public override string NestableView { get; } = "~/Views/Form/DragAndDrop/NestableFields/NestableTextAreaField.cshtml";
        
        [DataProp]
        public bool DataExtractionEnabled { get; set; }

        public override bool CanBeInDependencyFormula()
        {
            return true;
        }

    }
}