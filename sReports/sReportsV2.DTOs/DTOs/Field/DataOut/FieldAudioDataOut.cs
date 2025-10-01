using Newtonsoft.Json;
using sReportsV2.DTOs.DTOs.Field.DataOut;
using sReportsV2.DTOs.DTOs.FormInstance.DataOut;
using sReportsV2.Common.Extensions;

namespace sReportsV2.DTOs.Field.DataOut
{
    public partial class FieldAudioDataOut : FieldStringDataOut
    {
        [JsonIgnore]
        public override string PartialView { get; } = "~/Views/Form/Fields/FieldAudio.cshtml";

        [JsonIgnore]
        public override string NestableView { get; } = "~/Views/Form/DragAndDrop/NestableFields/NestableAudioField.cshtml";

        public override string GetLabel()
        {
            return this.FullLabel;
        }

        public override bool CanBeConnectedField()
        {
            return false;
        }
    }
}