using System.Collections.Generic;

namespace sReportsV2.HL7.Validation
{
    public class OutgoingOruRequiredMessageRule : RequiredValuesMessageRule
    {
        protected override IEnumerable<string> GetRequiredFieldNames()
        {
            return new List<string>()
            {
                "MSH-7",
                "MSH-9",
                "MSH-10",
                "MSH-12",
                "/PATIENT_RESULT/PATIENT/PID-3",
                "/PATIENT_RESULT/PATIENT/PID-5-1",
                "/PATIENT_RESULT/PATIENT/PID-5-2",
                "/PATIENT_RESULT/PATIENT/PID-7",
                "/PATIENT_RESULT/PATIENT/PID-8",
                "/PATIENT_RESULT/PATIENT/VISIT/PV1-4",
                //"/PATIENT_RESULT/PATIENT/VISIT/PV1-19"
            };
        }
    }
}