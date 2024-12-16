using NHapi.Base.Model;
using NHapi.Base.Util;
using NHapi.Base.Validation;
using System;
using System.Collections.Generic;

namespace sReportsV2.HL7.Validation
{
    public class RequiredValuesMessageRule : IMessageRule
    {
        public string Description => string.Empty;

        public string SectionReference => string.Empty;

        public ValidationException[] test(IMessage msg)
        {
            return TestIfRequiredFieldsHaveValue(new Terser(msg));
        }

        public ValidationException[] Test(IMessage msg)
        {
            
            return test(msg);
        }

        private ValidationException[] TestIfRequiredFieldsHaveValue(Terser terser)
        {
            List<string> missingFields = new List<string>();
            foreach (string fieldName in GetRequiredFieldNames())
            {
                if (string.IsNullOrEmpty(terser.Get(fieldName)))
                {
                    missingFields.Add(fieldName);
                }
            }
            
            return missingFields.Count > 0 ? new ValidationException[1] { new ValidationException(FormatValidationExceptionMessage(missingFields)) } : Array.Empty<ValidationException>();
        }

        private string FormatValidationExceptionMessage(List<string> missingFields)
        {
            return $"Fields [{string.Join(",", GetRequiredFieldNames())}] have to be entered, but [{string.Join(",", missingFields)}] are missing";
        }

        protected virtual IEnumerable<string> GetRequiredFieldNames()
        {
            return new List<string>()
            {
                "MSH-7",
                "MSH-9",
                "MSH-10",
                "MSH-12",
                "PID-3",
                "PID-5-1",
                "PID-5-2",
                "PID-7",
                "PID-8",
                "PV1-4",
                "PV1-19"
            };
        }
    }
}