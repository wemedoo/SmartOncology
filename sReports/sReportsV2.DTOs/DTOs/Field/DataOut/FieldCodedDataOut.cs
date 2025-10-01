using Newtonsoft.Json;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.DTOs.DTOs.FormInstance.DataOut;

namespace sReportsV2.DTOs.Field.DataOut
{
    public partial class FieldCodedDataOut : FieldStringDataOut
    {
        [DataProp]
        public int CodeSetId { get; set; }
        
        [JsonIgnore]
        public override string PartialView { get; } = "~/Views/Form/Fields/FieldCoded.cshtml";
        [JsonIgnore]
        public override string NestableView { get; } = "~/Views/Form/DragAndDrop/NestableFields/NestableCodedField.cshtml";

        public override bool CanBeInDependencyFormula()
        {
            return true;
        }
    }
}
