using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.HL7.Validation
{
    public class OnlyPatientDataRequiredMessageRule : RequiredValuesMessageRule
    {
        protected override IEnumerable<string> GetRequiredFieldNames()
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
                "PID-8"
            };
        }
    }
}