using sReportsV2.Common.Enums;
using System.Collections.Generic;
using sReportsV2.Common.Extensions;


namespace sReportsV2.DTOs.Form.DataOut
{
    public class FormFieldDependableDataOut
    {
        public string Condition { get; set; }
        public string ConditionFormatted { get; set; }
        public FormFieldDependableType? ActionType { get; set; }
        public string ActionParams { get; set; }
        public List<FormFieldDependableDataOut> Dependables { get; set; } = new List<FormFieldDependableDataOut>();
        public void SetConditionFormatted()
        {
            this.ConditionFormatted = this.Condition.ReplaceNonAlphaCharactersWithDash().RemoveDiacritics();
        }
    }
}