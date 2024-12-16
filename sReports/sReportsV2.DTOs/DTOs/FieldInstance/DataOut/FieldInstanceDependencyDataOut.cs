using sReportsV2.Common.Enums;
using System.Collections.Generic;

namespace sReportsV2.DTOs.DTOs.FieldInstance.DataOut
{
    public class FieldInstanceDependencyDataOut
    {
        public LogicalExpresionResult ExpressionResult { get; set; }
        public List<FieldAction> FieldActions { get; set; }

        public FieldInstanceDependencyDataOut(LogicalExpresionResult result, List<FieldAction> fieldActions)
        {
            ExpressionResult = result;
            FieldActions = fieldActions;
        }
    }

    public enum LogicalExpresionResult
    {
        TRUE,
        FALSE,
        UNDEFINED
    }
}
