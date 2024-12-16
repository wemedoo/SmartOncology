using NHapi.Base.Model;
using NHapi.Base.Util;
using NHapi.Base.Validation;
using NHapi.Model.V231.Datatype;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Helpers;
using sReportsV2.Cache.Singleton;
using System;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.HL7.Validation
{
    public class RequiredMRNIdentifierMessageRule : IMessageRule
    {
        public string Description => $"MRN Identifier is not present in incoming message fields: {SectionReference}";

        public string SectionReference => "PID-2 or PID-3";

        public ValidationException[] test(IMessage msg)
        {
            return TestIfMRNIdentifierIsPresent(new Terser(msg));
        }

        public ValidationException[] Test(IMessage msg)
        {
            return test(msg);
        }

        private ValidationException[] TestIfMRNIdentifierIsPresent(Terser terser)
        {
            ValidationException[] validationExceptions = Array.Empty<ValidationException>();
            try
            {
                var pidSegment = terser.GetSegment("PID");
                if (pidSegment != null)
                {
                    var pid_2 = pidSegment.GetField(2);
                    var pid_3 = pidSegment.GetField(3);
                    if (!HasRequiredInboundAlias(pid_2.Concat(pid_3)))
                    {
                        validationExceptions = new ValidationException[1] { new ValidationException(Description) };
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error($"Error while performing RequiredMRNIdentifierMessageRule, message: {ex.Message}");
            }
            return validationExceptions;
        }

        private bool HasRequiredInboundAlias(IEnumerable<IType> identifierFields)
        {
            IEnumerable<string> inboundAliasesForIdentifierType = identifierFields.Select(f => ((CX)f).IdentifierTypeCode.Value);
            IEnumerable<string> inboundAliasesForIdentifierTypeInSO = SingletonDataContainer.Instance.GetInboundAliasesForCode((int)CodeSetList.PatientIdentifierType, ResourceTypes.MedicalRecordNumber);
            bool hasMRNIdentifierInboundAlias = inboundAliasesForIdentifierType.Intersect(inboundAliasesForIdentifierTypeInSO).Any();
            return hasMRNIdentifierInboundAlias;
        }
    }
}