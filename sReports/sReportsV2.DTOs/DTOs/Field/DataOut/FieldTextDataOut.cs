using Newtonsoft.Json;
using sReportsV2.Common.CustomAttributes;
using System.Collections.Generic;

namespace sReportsV2.DTOs.Field.DataOut
{
    public class FieldTextDataOut : FieldStringDataOut
    {
        [JsonIgnore]
        public override string PartialView { get; } = "~/Views/Form/Fields/FieldText.cshtml";

        [JsonIgnore]
        public override string NestableView { get; } = "~/Views/Form/DragAndDrop/NestableFields/NestableTextField.cshtml";

        [DataProp]
        public int? MinLength { get; set; }
        [DataProp]
        public int? MaxLength { get; set; }

        [JsonIgnore]
        public override string ValidationAttr
        {
            get
            {
                var attributes = new List<string>();

                AddAttributeIfNotNull(attributes, "minlength", MinLength);
                AddAttributeIfNotNull(attributes, "data-minlength", MinLength);
                AddAttributeIfNotNull(attributes, "maxLength", MaxLength);
                AddAttributeIfNotNull(attributes, "data-maxlength", MaxLength);

                return string.Join(" ", attributes);
            }
        }

        [JsonIgnore]
        public override string PopulateAdditionalAttr
        {
            get
            {
                var attributes = new List<string>();

                AddAttributeIfNotNull(attributes, "data-minlength", MinLength);
                AddAttributeIfNotNull(attributes, "data-maxlength", MaxLength);

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
    }
}