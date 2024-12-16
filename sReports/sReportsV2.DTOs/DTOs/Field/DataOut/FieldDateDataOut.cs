using Newtonsoft.Json;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Common.Extensions;
using sReportsV2.DTOs.DTOs.FormInstance.DataOut;
using sReportsV2.Common.Enums;

namespace sReportsV2.DTOs.Field.DataOut
{
    public class FieldDateDataOut : FieldStringDataOut
    {
        [JsonIgnore]
        public override string PartialView { get; } = "~/Views/Form/Fields/FieldDate.cshtml";

        [JsonIgnore]
        public override string NestableView { get; } = "~/Views/Form/DragAndDrop/NestableFields/NestableDateField.cshtml";

        [DataProp]
        public bool PreventFutureDates { get; set; }

        public override bool CanBeInDependencyFormula()
        {
            return true;
        }

        protected override string FormatDisplayValue(FieldInstanceValueDataOut fieldInstanceValue, string valueSeparator)
        {
            return fieldInstanceValue.FirstValue.RenderDate();
        }

        protected override int GetMissingValueCodeSetId()
        {
            return (int)CodeSetList.MissingValueDate;
        }
    }
}