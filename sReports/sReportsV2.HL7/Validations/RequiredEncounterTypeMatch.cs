using NHapi.Base.Model;
using NHapi.Base.Util;
using NHapi.Base.Validation;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Helpers;
using sReportsV2.Cache.Singleton;
using System;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.HL7.Validation
{
    public class RequiredEncounterTypeMatch : IMessageRule
    {
        public string Description => $"Encounter type supported in SO is not present in incoming message fields: {SectionReference}";

        public string SectionReference => "PV1-4";

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
                string encounterType = terser.Get(SectionReference);
                
                if (!HasRequiredInboundAlias(encounterType))
                {
                    validationExceptions = new ValidationException[1] { new ValidationException(Description) };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error($"Error while performing RequiredMRNIdentifierMessageRule, message: {ex.Message}");
            }
            return validationExceptions;
        }

        private bool HasRequiredInboundAlias(string encounterType)
        {
            IEnumerable<string> encounterTypeCodesetInboudAliases = SingletonDataContainer.Instance.GetInboundAliasesForCodeSet((int)CodeSetList.EncounterType);
            bool hasencounterTypeInboundAlias = encounterTypeCodesetInboudAliases.Any(inboundAlias => inboundAlias == encounterType);
            return hasencounterTypeInboundAlias;
        }
    }
}