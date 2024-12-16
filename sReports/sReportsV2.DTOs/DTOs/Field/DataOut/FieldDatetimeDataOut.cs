using Newtonsoft.Json;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Extensions;
using sReportsV2.DTOs.DTOs.FormInstance.DataOut;

namespace sReportsV2.DTOs.Field.DataOut
{
    public class FieldDatetimeDataOut : FieldStringDataOut
    {
        [JsonIgnore]
        public override string PartialView { get; } = "~/Views/Form/Fields/FieldDatetime.cshtml";

        [JsonIgnore]
        public override string NestableView { get; } = "~/Views/Form/DragAndDrop/NestableFields/NestableDatetimeField.cshtml";

        [DataProp]
        public bool PreventFutureDates { get; set; }

        public override bool CanBeInDependencyFormula()
        {
            return true;
        }

        protected override string FormatDisplayValue(FieldInstanceValueDataOut fieldInstanceValue, string valueSeparator)
        {
            string value = fieldInstanceValue.FirstValue;
            return $"{value.RenderDate()} {value.RenderTime()}";
        }

        protected override int GetMissingValueCodeSetId()
        {
            return (int)CodeSetList.MissingValueDateTime;
        }
    }
}