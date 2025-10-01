using Newtonsoft.Json;

namespace sReportsV2.DTOs.Field.DataOut
{
    public partial class FieldFileDataOut : FieldStringDataOut
    {
        [JsonIgnore]
        public override string PartialView { get; } = "~/Views/Form/Fields/FieldFile.cshtml";

        [JsonIgnore]
        public override string NestableView { get; } = "~/Views/Form/DragAndDrop/NestableFields/NestableFileField.cshtml";
        public bool DataExtractionEnabled { get; set; }

        public override bool CanBeConnectedField()
        {
            return false;
        }
    }
}