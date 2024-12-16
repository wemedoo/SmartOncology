using sReportsV2.Common.Enums;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.DTOs.Field.DataIn
{
    public class DependentOnInfoDataIn
    {
        public string Formula { get; set; }
        public List<DependentOnFieldInfoDataIn> DependentOnFieldInfos { get; set; } = new List<DependentOnFieldInfoDataIn>();
        public List<FieldAction> FieldActions { get; set; }

        public List<string> GetFormulaVariables()
        {
            return DependentOnFieldInfos.Select(x => x.Variable).ToList();
        }

        public DependentOnFieldInfoDataIn GetDependentOnFieldInfoByVariable(string variableName)
        {
            return this.DependentOnFieldInfos.Find(f => f.Variable == variableName);
        }
    }
}
