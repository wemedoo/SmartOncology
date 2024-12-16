using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Common.Constants
{
    public static class FhirResourcesConstant
    {
        public const string QuestionnaireDefinitionUrl = "http://h17.org/fhir/StructureDefinition/SDC-questionnaire-url";
        public const string QuestionnaireResponseDefinitionUrl = "http://h17.org/fhir/StructureDefinition/SDC-response-endpoint";
    }

    public static class FhirDocumentReferenceConstants
    {
        public const string StatusCurrent = "current";
        public const string StatusSuperseded = "superseded";
        public const string StatusEnteredInError = "entered-in-error";
    }
}
