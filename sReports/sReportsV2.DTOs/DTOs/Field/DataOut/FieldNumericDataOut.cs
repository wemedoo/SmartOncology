using Newtonsoft.Json;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Common.Enums;
using System.Collections.Generic;

namespace sReportsV2.DTOs.Field.DataOut
{
    public class FieldNumericDataOut : FieldStringDataOut
    {
        [DataProp]
        public double? Min { get; set; }

        [DataProp]
        public double? Max { get; set; }

        [DataProp]
        public double? Step { get; set; }

        [JsonIgnore]
        public override string PartialView { get; } = "~/Views/Form/Fields/FieldNumber.cshtml";

        [JsonIgnore]
        public override string NestableView { get; } = "~/Views/Form/DragAndDrop/NestableFields/NestableNumberField.cshtml";

        [JsonIgnore]
        public override string ValidationAttr
        {
            get
            {
                var attributes = new List<string>();

                AddAttributeIfNotNull(attributes, "min", Min);
                AddAttributeIfNotNull(attributes, "data-min", Min);
                AddAttributeIfNotNull(attributes, "max", Max);
                AddAttributeIfNotNull(attributes, "data-max", Max);
                AddAttributeIfNotNull(attributes, "Step", Step);

                return string.Join(" ", attributes);
            }
        }

        [JsonIgnore]
        public override string PopulateAdditionalAttr
        {
            get
            {
                var attributes = new List<string>();

                AddAttributeIfNotNull(attributes, "data-min", Min);
                AddAttributeIfNotNull(attributes, "data-max", Max);
                AddAttributeIfNotNull(attributes, "Step", Step);

                return string.Join(" ", attributes);
            }
        }

        public override bool CanBeInDependencyFormula()
        {
            return true;
        }

        private void AddAttributeIfNotNull(List<string> attributes, string attributeName, object value)
        {
            if (value != null)
            {
                attributes.Add($"{attributeName}={value}");
            }
        }

        protected override int GetMissingValueCodeSetId()
        {
            return (int)CodeSetList.MissingValueNumber;
        }
    }
}