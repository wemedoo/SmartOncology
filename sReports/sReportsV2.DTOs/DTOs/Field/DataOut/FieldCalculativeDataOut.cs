using Newtonsoft.Json;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Common.Enums;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.DTOs.Field.DataOut
{
    public class FieldCalculativeDataOut : FieldStringDataOut
    {
        [JsonIgnore]
        public override string PartialView { get; } = "~/Views/Form/Fields/FieldCalculative.cshtml";

        [JsonIgnore]
        public override string NestableView { get; } = "~/Views/Form/DragAndDrop/NestableFields/NestableCalculativeField.cshtml";

        [DataProp]
        public string Formula { get; set; }

        [DataProp]
        public Dictionary<string, string> IdentifiersAndVariables { get; set; }
        [DataProp]
        public CalculationFormulaType FormulaType { get; set; }
        [DataProp]
        public CalculationGranularityType? GranularityType { get; set; }

        public string GetFormulaNormilized()
        {
            return this.Formula
                    .Replace("[", "")
                    .Replace("]", "")
                    .Trim();
        }

        public List<string> GetFormulaFields()
        {
            List<string> result = new List<string>();

            string[] spliited = Formula.Split('[');
            foreach (string split in spliited.Where(x => x.Contains("]")))
            {
                string fieldData = split.Trim();
                int indexOfBracket = fieldData.IndexOf("]");
                string fieldId = fieldData.Substring(0, indexOfBracket);
                result.Add(fieldId);
            }
            return result;
        }

        public override bool CanBeInDependencyFormula()
        {
            return true;
        }
    }
}