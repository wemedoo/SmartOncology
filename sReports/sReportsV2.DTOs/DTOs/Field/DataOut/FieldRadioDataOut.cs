using Newtonsoft.Json;
using sReportsV2.Common.Extensions;
using System.Linq;

namespace sReportsV2.DTOs.Field.DataOut
{
    public class FieldRadioDataOut : FieldSelectableDataOut
    {
        [JsonIgnore]
        public override string PartialView { get; } = "~/Views/Form/Fields/FieldRadio.cshtml";

        [JsonIgnore]
        public override string NestableView { get; } = "~/Views/Form/DragAndDrop/NestableFields/NestableRadioField.cshtml";
    }
}