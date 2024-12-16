using NHapi.Base.Validation.Implementation;
using System.Collections.Generic;

namespace sReportsV2.HL7.Validation
{
    public class CustomValidations : DefaultValidation
    {
        public CustomValidations(IList<NHapi.Base.Validation.IMessageRule> validationRules)
        {
            foreach (var validationRule in validationRules)
            {
                MessageRuleBindings.Add(new RuleBinding("*", "*", validationRule));
            }
        }
    }
}