using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace sReportsV2.Common.Enums.DocumentPropertiesEnums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum GeneralPurpose
    {
        InformationCollection,
        InformationPresentation,
        MixedInformationPresentationAndCollection,
        ContextDependent
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ContextDependent
    {
        None,
        UserAccessRight,
        DocumentInformationCollectionOrPresentationState
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum DocumentExplicitPurpose
    {
        ReferralToProcedure,
        ReportingOfProcedure,
        ReportingOfFindings,
        ReportingOfTherapoDiagnosticProcedures,
        CombinedPurposeForReferralAndReporting
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ScopeOfValidity
    {
        International,
        AdministrativeUnit,
        InterInstitutional,
        IntraInstitutional
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ClinicalContext
    {
        Preventive,
        Screening,
        Diagnostic,
        Theragnostic,
        Therapy,
        Rehabilitation,
        FollowUp
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum FollowUp
    {
        None,
        EarlyDetectionOfDiseaseRelapse,
        EvaluationOfTherapeuticEffect,
        DetectionAndEvalutationOfTherapyAssociatedAdverseEvents,
        TreatmentOfTherapyaAssociatedAdverseEvents
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum AdministrativeContext
    {
        InsuranceRelatedDocumentation,
        Billing,
        RegistryEntry
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum Class
    {
        Clinical,
        Research,
        AdministrativeMedical,
        Other
    }
}
