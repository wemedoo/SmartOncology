using sReportsV2.Common.Enums;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.DTOs.Field.DataOut
{
    public class DependentOnInfoDataOut
    {
        public string Formula { get; set; }
        public List<DependentOnFieldInfoDataOut> DependentOnFieldInfos { get; set; }
        public List<FieldAction> FieldActions { get; set; }

        public DependentOnInfoDataOut()
        {
        }

        public DependentOnInfoDataOut(DependentOnInfoDataOut dependentOnInfoDataOut)
        {
            this.Formula = dependentOnInfoDataOut.Formula;
            this.DependentOnFieldInfos = dependentOnInfoDataOut.DependentOnFieldInfos;
            this.FieldActions = dependentOnInfoDataOut.FieldActions;
        }

        public bool HasDependentField(string fieldId)
        {
            return this.DependentOnFieldInfos != null && this.DependentOnFieldInfos.Exists(f => f.FieldId == fieldId && string.IsNullOrEmpty(f.FieldValueId));
        }

        public bool HasDependentFieldValue(string fieldValueId)
        {
            return this.DependentOnFieldInfos != null && this.DependentOnFieldInfos.Exists(f => f.FieldValueId == fieldValueId);
        }

        public DependentOnFieldInfoDataOut GetDependentOnFieldInfoByVariable(string variableName)
        {
            return this.DependentOnFieldInfos.Find(f => f.Variable == variableName);
        }
    }
}
