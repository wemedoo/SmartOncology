using Newtonsoft.Json;
using sReportsV2.Common.Extensions;
using sReportsV2.DTOs.DTOs.Field.DataOut;
using sReportsV2.DTOs.DTOs.FormInstance.DataOut;

namespace sReportsV2.DTOs.Field.DataOut
{
    public class FieldFileDataOut : FieldStringDataOut, IBinaryFieldDataOut
    {
        [JsonIgnore]
        public override string PartialView { get; } = "~/Views/Form/Fields/FieldFile.cshtml";

        [JsonIgnore]
        public override string NestableView { get; } = "~/Views/Form/DragAndDrop/NestableFields/NestableFileField.cshtml";

        public bool ExcludeGUIDPartFromName => true;

        public string RemoveClass => "file-remove";

        public string BinaryType => Type;

        protected override string FormatDisplayValue(FieldInstanceValueDataOut fieldInstanceValue, string valueSeparator)
        {
            return fieldInstanceValue.FirstValue.GetFileNameFromUri();
        }
    }
}