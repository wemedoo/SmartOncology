using Newtonsoft.Json;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.DTOs.DTOs.FormInstance.DataOut;

namespace sReportsV2.DTOs.Field.DataOut
{
    public class FieldCodedDataOut : FieldStringDataOut
    {
        [DataProp]
        public int CodeSetId { get; set; }
        
        [JsonIgnore]
        public override string PartialView { get; } = "~/Views/Form/Fields/FieldCoded.cshtml";
        [JsonIgnore]
        public override string NestableView { get; } = "~/Views/Form/DragAndDrop/NestableFields/NestableCodedField.cshtml";

        protected override string FormatDisplayValue(FieldInstanceValueDataOut fieldInstanceValue, string valueSeparator)
        {
            string valueLabel = GetValueLabel();
            string valueLabelOrValue = !string.IsNullOrWhiteSpace(valueLabel) ? valueLabel : GetValue();
            return valueLabelOrValue ?? string.Empty;
        }

        public override bool CanBeInDependencyFormula()
        {
            return true;
        }
    }
}
