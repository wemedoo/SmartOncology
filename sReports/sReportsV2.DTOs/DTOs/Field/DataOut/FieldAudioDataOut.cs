using Newtonsoft.Json;
using sReportsV2.DTOs.DTOs.Field.DataOut;
using sReportsV2.DTOs.DTOs.FormInstance.DataOut;
using sReportsV2.Common.Extensions;

namespace sReportsV2.DTOs.Field.DataOut
{
    public class FieldAudioDataOut : FieldStringDataOut, IBinaryFieldDataOut
    {
        [JsonIgnore]
        public override string PartialView { get; } = "~/Views/Form/Fields/FieldAudio.cshtml";

        [JsonIgnore]
        public override string NestableView { get; } = "~/Views/Form/DragAndDrop/NestableFields/NestableAudioField.cshtml";

        public bool ExcludeGUIDPartFromName => false;

        public string RemoveClass => "audio-file-remove";

        public string BinaryType => Type;

        public override string GetLabel()
        {
            return this.FullLabel;
        }

        protected override string FormatDisplayValue(FieldInstanceValueDataOut fieldInstanceValue, string valueSeparator)
        {
            return fieldInstanceValue.FirstValue.GetFileNameFromUri();
        }
    }
}