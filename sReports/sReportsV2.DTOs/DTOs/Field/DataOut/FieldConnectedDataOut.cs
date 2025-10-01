using Newtonsoft.Json;
using sReportsV2.Common.CustomAttributes;
using System.Collections.Generic;

namespace sReportsV2.DTOs.Field.DataOut
{
    public partial class FieldConnectedDataOut : FieldStringDataOut
    {
        [DataProp]
        public List<string> ConnectedFieldIds { get; set; }

        [JsonIgnore]
        public override string PartialView { get; } = "~/Views/Form/Fields/FieldConnected.cshtml";
        [JsonIgnore]
        public override string NestableView { get; } = "~/Views/Form/DragAndDrop/NestableFields/NestableConnectedField.cshtml";

        public override bool CanBeInDependencyFormula()
        {
            return true;
        }

        public override bool CanBeConnectedField()
        {
            return false;
        }
    }
}
