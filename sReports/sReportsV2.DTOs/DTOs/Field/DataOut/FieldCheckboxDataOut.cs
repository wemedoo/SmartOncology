using Newtonsoft.Json;
using sReportsV2.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.DTOs.Field.DataOut
{
    public class FieldCheckboxDataOut : FieldSelectableDataOut
    {
        [JsonIgnore]
        public override string PartialView { get; } = "~/Views/Form/Fields/FieldCheckbox.cshtml";
        [JsonIgnore]
        public override string NestableView { get; } = "~/Views/Form/DragAndDrop/NestableFields/NestableCheckBoxField.cshtml";
    }
}