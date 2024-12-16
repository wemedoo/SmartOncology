using Newtonsoft.Json;

namespace sReportsV2.DTOs.Field.DataOut
{
    public class FieldEmailDataOut : FieldStringDataOut
    {
        [JsonIgnore]
        public override string PartialView { get; } = "~/Views/Form/Fields/FieldEmail.cshtml";

        [JsonIgnore]
        public override string NestableView { get; } = "~/Views/Form/DragAndDrop/NestableFields/NestableEmailField.cshtml";

        public override bool CanBeInDependencyFormula()
        {
            return true;
        }
    }
}