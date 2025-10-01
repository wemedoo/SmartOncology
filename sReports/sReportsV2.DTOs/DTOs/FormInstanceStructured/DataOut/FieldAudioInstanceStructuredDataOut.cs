using sReportsV2.Common.Extensions;
using sReportsV2.DTOs.DTOs.Field.DataOut;
using sReportsV2.DTOs.DTOs.FormInstance.DataOut;

namespace sReportsV2.DTOs.Field.DataOut
{
    public partial class FieldAudioDataOut : IBinaryFieldDataOut
    {
        public bool ExcludeGUIDPartFromName => false;

        public string RemoveClass => "audio-file-remove";

        public string BinaryType => Type;
        protected override string FormatDisplayValue(FieldInstanceValueDataOut fieldInstanceValue, string valueSeparator)
        {
            return fieldInstanceValue.FirstValue.GetFileNameFromUri();
        }
    }
}
