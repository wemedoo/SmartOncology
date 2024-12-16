using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace sReportsV2.Common.Enums
{
    #region Entity Enums
    [JsonConverter(typeof(StringEnumConverter))]
    public enum FormDefinitionState
    {
        DesignPending = 0,
        Design = 1,
        ReviewPending = 2,
        Review = 3,
        ReadyForDataCapture = 4,
        Archive = 5
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum FormFieldDependableType
    {
        Toggle,
        Email
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum LayoutType
    {
        Matrix,
        Vertical
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum FormState
    {
        Finished,
        OnGoing,
        Locked,
        InError,
        Unlocked
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ConsensusFindingState
    {
        OnGoing,
        InIteration,
        Finished
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum QuestionOccurenceType
    {
        Any,
        Same,
        Different
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum IterationState
    {
        NotStarted,
        Design,
        InProgress,
        Finished,
        Terminated
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum NodeState
    {
        NotStarted,
        Active,
        Completed,
        Disabled
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum CalculationFormulaType
    {
        Numeric = 0,
        Date = 1
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum CalculationGranularityType
    {
        Year = 0,
        Month,
        Day,
        YearMonth,
        YearMonthDay
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ChapterPageState
    {
        DataEntryOnGoing = 0,
        Locked
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum PropagationType
    {
        Page = 0,
        Chapter,
        FormInstance
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ApiRequestDirection
    {
        Outgoing = 1,
        Incoming = 2
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum FieldAction
    {
        DataCleaning
    }

    #endregion /Entity Enums

    #region Reserved Ids

    [JsonConverter(typeof(StringEnumConverter))]
    public enum EntityStateCode
    {
        Active = 2001,
        Merged = 2002,
        Deleted = 2003
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum CodeSetList
    {
        OrganizationType = 1,
        EpisodeOfCareType = 2,
        EncounterType = 3,
        EncounterStatus = 4,
        EncounterClassification = 5,
        DiagnosisRole = 6,
        PatientIdentifierType = 7,
        OrganizationIdentifierType = 8,
        AddressType = 9,
        Citizenship = 10,
        Role = 11,
        FormDefinitionState = 12,
        Gender = 13,
        EOCStatus = 14,
        TelecomSystemType = 15,
        TelecomUseType = 16,
        IdentifierUseType = 17,
        FormState = 18,
        InstitutionalLegalForm = 19,
        InstitutionalOrganizationalForm = 20,
        UserPrefix = 21,
        AcademicPosition = 22,
        ClinicalTrialRecruitmentsStatus = 23,
        ClinicalTrialRole = 24,
        VersionType = 25,
        PredifinedGlobalUserRole = 26,
        PersonnelType = 27,
        EntityState = 28,
        IdentifierPool = 29,
        SourceSystem = 30,
        TransactionDirection = 31,
        ErrorType = 32,
        TelecommunicationUseType = 33,
        ContactRelationship = 34,
        ContactRole = 35,
        MaritalStatus = 36,
        CommunicationSystem = 37,
        TeamType = 38,
        PersonnelTeamRelationshipType = 39,
        Country = 40,
        Language = 45,
        ReligiousAffiliationType = 46,
        ServiceType = 52,
        TaskType = 53,
        TaskStatus = 54,
        TaskPriority = 55,
        TaskClass = 56,
        TaskDocument = 57,
        Contraception = 58,
        DiseaseContext = 59,
        InstanceState = 60,
        ChemotherapySchemaInstanceActionType = 61,
        ThesaurusState = 62,
        ThesaurusMergeState = 63,
        UserState = 64,
        CommentState = 65,
        GlobalUserSource = 66,
        GlobalUserStatus = 67,
        OrganizationCommunicationEntity = 80,
        ClinicalTrialIdentifiers = 81,
        ClinicalTrialSponsorIdentifierType = 82,
        OccupationCategory = 83,
        OccupationSubCategory = 84,
        Occupation = 85,
        PersonnelSeniority = 86,
        ProjectType = 87,
        CodedField = 88,
        RelationType = 89,
        ResultStatus = 90,
        Document = 91,
        NullFlavor = 92,
        MissingValueDate = 93,
        MissingValueDateTime = 94,
        MissingValueNumber = 95,
        ClinicalDomain = 96,
    }
    #endregion /Reserved Ids

    #region Non-entity enums

    [JsonConverter(typeof(StringEnumConverter))]
    public enum Gender
    {
        Male = 0,
        Female = 1,
        Other = 2,
        Unknown = 3
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum EocStatus
    {
        Planned,
        Waitlist,
        Active,
        Onhold,
        Finished,
        Cancelled,
        EnteredInError
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum TelecomSystemType
    {
        Phone,
        Fax,
        Email,
        Pager,
        Url,
        Sms,
        Other
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum TypeOfIdentifier
    {
        IdentifierName,
        IdentifierValue,
        IdentifierUse
    }

    [JsonConverter(typeof(StringEnumConverter))]

    public enum IdentifierKind
    {
        PatientIdentifierType,
        OrganizationIdentifierType
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum SimilarTermType
    {
        O4MTId,
        UMLS

    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum RegistrationType
    {
        Standard,
        Quick
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum FormTypeField
    {
        FormChapter,
        FormPage,
        FormFieldSet,
        Field,
        FieldRadioOrCheckButton
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum FormItemLevel
    {
        Form,
        Chapter,
        Page,
        FieldSet,
        Field,
        FieldValue
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum PredifinedGlobalUserRole
    {
        SuperAdministrator = 1,
        Viewer,
        Editor,
        Curator
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum PredifinedRole
    {
        SuperAdministrator = 1,
        Administrator,
        Doctor
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum FormInstanceViewMode
    {
        RegularView,
        SynopticView
    }
    #endregion /Non-entity enums
}

