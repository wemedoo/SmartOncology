using sReportsV2.Common.Enums;
using sReportsV2.Domain.Entities.FieldEntity;

namespace sReportsV2.Domain.Entities.Dependency
{
    public class DependentOnInfo
    {
        public string Formula { get; set; }    
        public List<DependentOnFieldInfo> DependentOnFieldInfos { get; set; }
        public List<FieldAction> FieldActions { get; set; }

        public string FormatFormula(Dictionary<string, Field> fields)
        {
            string formulaFormated = this.Formula;
            if (fields == null) return formulaFormated;
            foreach (DependentOnFieldInfo dependentOnFieldInfo in this.DependentOnFieldInfos)
            {
                if (fields.TryGetValue(dependentOnFieldInfo.FieldId, out Field field))
                {
                    string variableLabel = field.Label;
                    if (!string.IsNullOrEmpty(dependentOnFieldInfo.FieldValueId) && field is FieldSelectable fieldSelectable)
                    {
                        variableLabel = fieldSelectable.Values.Find(fV => fV.Id == dependentOnFieldInfo.FieldValueId)?.Label ?? string.Empty;
                    }
                    formulaFormated = formulaFormated.Replace($"[{dependentOnFieldInfo.Variable}]", $"[{variableLabel}]");
                }
            }
            return formulaFormated;
        }
    }
}
