using sReportsV2.Common.Constants;
using sReportsV2.Common.Enums;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using System.Collections.Generic;
using System.ComponentModel;

namespace sReportsV2.Initializer.CodeSets
{
    public static class CodeSetValues
    {
        [DisplayName(CodeSetAttributeNames.OrganizationType)]
        public static List<string> OrganizationType { get; set; } = new List<string>()
        {
            "Healthcare Provider",
            "Hospital Department",
            "Organizational Team",
            "Government",
            "Payer",
            "Educational Institute",
            "Religious Institution",
            "Clinical Research Sponsor",
            "Community Group",
            "Non Healthcare Business Or Corporation",
            "University Hospital",
            "Cantonal Hospital",
            "Group Practice",
            "Family Doctor Office",
            "Research Institution",
            "Spitex",
            "Governmental Body",
            "Non Profit Organization",
            "Patient Group",
            "Department",
            "Team",
            "Other"
        };

        [DisplayName(CodeSetAttributeNames.EpisodeOfCareType)]
        public static List<string> EpisodeOfCareType { get; set; } = new List<string>()
        {
            "Home and Community Care",
            "Post Acute Care",
            "Post coordinated diabetes program",
            "Drug and alcohol rehabilitation",
            "Community-based aged care",
            "General Prevention Program of Oncological Diseases",
            "Prevention of Oncological Diseases - Municipality Program for Smoking Cesation",
            "Screening for Oncological Diseases - Colorectal Cancer Screening",
            "Screening for Oncological Diseases - Cantonal Program for Breast Cancer Screening Bern",
            "Diagnosis of Oncological Diseases and Staging Procedures - Head and Neck Cancer Clinical Pathway of Inselspital Bern",
            "Treatment of Oncological Diseases - Head and Neck Cancer Clinical Pathway of Inselspital Bern",
            "Oncological Acute Post-Treatment Care - Head and Neck Cancer Clinical Pathway of Inselspital Bern",
            "Oncological Long-Term Care and Follow-Up - Head and Neck Cancer Clinical Pathway of Inselspital Bern",
            "Radiation oncology treatment",
        };

        [DisplayName(CodeSetAttributeNames.EncounterType)]
        public static List<string> EncounterType { get; set; } = new List<string>()
        {
            "Not Applicable",
            "Annual Diabetes Mellitus Screening",
            "Bone Drilling Bone Marrow Punction In Clinic",
            "Infant Colon Screening 60 minutes",
            "Outpatient Kenacort Injection",
            "Emergency room admission (procedure)",
            "General examination of patient (procedure)",
            "Emergency Room Admission",
            "Emergency hospital admission (procedure)",
            "Encounter for check up (procedure)",
            "Encounter for symptom",
            "Encounter for problem",
            "Periodic reevaluation and management of healthy individual (procedure)",
            "Postoperative follow-up visit (procedure)",
            "Screening surveillance (regime/therapy)",
            "Gynecology service (qualifier value)",
            "Well child visit (procedure)",
            "Encounter for symptom (procedure)",
            "Discussion about treatment (procedure)",
            "Follow-up encounter",
            "Encounter for problem (procedure)",
            "Drug rehabilitation and detoxification",
            "Outpatient procedure",
            "Prenatal initial visit",
            "Prenatal visit",
            "Obstetric emergency hospital admission",
            "Postnatal visit",
            "Consultation for treatment",
            "Patient encounter procedure",
            "Urgent care clinic (environment)",
            "Admission to surgical department",
            "Follow-up visit (procedure)",
            "Non-urgent orthopedic admission",
            "Follow-up encounter (procedure)",
            "Patient-initiated encounter",
            "Asthma follow-up",
            "Emergency hospital admission for asthma",
            "Assessment of dementia (procedure)",
            "Domiciliary or rest home patient evaluation and management",
            "Hospital admission",
            "posttraumatic stress disorder",
            "Allergic disorder initial assessment",
            "Allergic disorder follow-up assessment",
            "Admission to thoracic surgery department",
            "Initial psychiatric interview with mental status and evaluation (procedure)",
            "Telephone encounter (procedure)",
            "Inpatient stay 3 days",
            "Inpatient stay (finding)",
            "Telemedicine consultation with patient",
            "Cardiac arrest",
            "Myocardial infarction",
            "Stroke"
        };

        [DisplayName(CodeSetAttributeNames.EncounterStatus)]
        public static List<string> EncounterStatus { get; set; } = new List<string>()
        {
            "Planned",
            "Arrived",
            "Triaged",
            "In progress",
            "Onleave",
            "Finished",
            "Cancelled"
        };

        [DisplayName(CodeSetAttributeNames.EncounterClassification)]
        public static List<string> EncounterClassification { get; set; } = new List<string>()
        {
            "Ambulatory",
            "Emergency",
            "Field",
            "Home Health",
            "Inpatient Encounter",
            "Inpatient Non Acute",
            "Observation Encounter",
            "Pre Admission",
            "Short Stay",
            "Virtual",
            "Not Applicable",
            "Inpatient Acute",
            "DATE OF ASSESSMENT (DD/MM/YYYY):",
            "Leukocyte esterase [Presence] in Urine by Test strip"
        };

        [DisplayName(CodeSetAttributeNames.DiagnosisRole)]
        public static List<string> DiagnosisRole { get; set; } = new List<string>()
        {
            "Billing",
            "Post Op Diagnosis",
            "Pre Op Diagnosis",
            "Comorbidity Diagnosis",
            "Chief Complaint",
            "Discharge Diagnosis",
            "Admission Diagnosis",
            "Not Applicable",
        };

        [DisplayName(CodeSetAttributeNames.PatientIdentifierType)]
        public static List<string> PatientIdentifierType { get; set; } = new List<string>()
        {
            "Account number",
            "Account number Creditor",
            "Account number debitor",
            "Accreditation/Certification Identifier",
            "Advanced Practice Registered Nurse number",
            "American Express",
            "American Medical Association Number",
            "Ancestor Specimen ID",
            "Anonymous identifier",
            "Bank Account Number",
            "Bank Card Number",
            "Birth Certificate",
            "Birth Certificate File Number",
            "Birth registry number",
            "Breed Registry Number",
            "Change of Name Document",
            "Citizenship Card",
            "Cost Center number",
            "Country number",
            "Death Certificate File Number",
            "Death Certificate ID",
            "Dentist license number",
            "Diner's Club card",
            "Diplomatic Passport",
            "Discover Card",
            "Doctor number",
            "Donor Registration Number",
            "Driver's Licence",
            "Drug Enforcement Administration registration number",
            "Drug Furnishing or prescriptive authority Number",
            "Employee number",
            "Facility ID",
            "Fetal Death Report File Number",
            "Fetal Death Report ID",
            "Filler Identifier",
            "General ledger number",
            "Guarantor external identifier",
            "Guarantor internal identifier",
            "Health Card Number",
            "Health Plan Identifier",
            "Indigenous/Aboriginal",
            "Insel PID",
            "Jurisdictional health number (Canada)",
            "Labor and industries number",
            "Laboratory Accession ID",
            "License number",
            "Lifelong physician number",
            "Living Subject Enterprise Number",
            "Local Registry ID",
            "Marriage Certificate",
            "Master Card",
            "Medical License number",
            "Medical Record Number",
            "Medicare/CMS (formerly HCFA)'s Universal Physician Identification numbers",
            "Medicare/CMS Performing Provider Identification Number",
            "Member Number",
            "Microchip Number",
            "Military ID number",
            "National Health Plan Identifier",
            "National Insurance Organization Identifier",
            "National Insurance Payor Identifier (Payor)",
            "National Person Identifier where the xxx is the ISO table 3166 3-character (alphabetic) country code",
            "National employer identifier",
            "National provider identifier",
            "National unique individual identifier",
            "Naturalization Certificate",
            "Nurse practitioner number",
            "Observation Instance",
            "Optometrist license number",
            "Osteopathic License number",
            "Parole Card",
            "Passport Number",
            "Patient Medicaid number",
            "Patient external identifier",
            "Patient internal identifier",
            "Patient's Medicare number",
            "Pension Number",
            "Permanent Resident Card Number",
            "Person number",
            "Pharmacist license number",
            "Physician Assistant number",
            "Placer Identifier",
            "Podiatrist license number",
            "Practitioner Medicaid number",
            "Practitioner Medicare number",
            "Primary physician office number",
            "Public Health Case Identifier",
            "Public Health Event Identifier",
            "Public Health Official ID",
            "QA number",
            "Railroad Retirement Provider",
            "Railroad Retirement number",
            "Regional registry ID",
            "Registered Nurse Number",
            "Resource identifier",
            "Secondary physician office number",
            "Shipment Tracking Number",
            "Social Security Number",
            "Social number",
            "Specimen ID",
            "Staff Enterprise Number",
            "State assigned NDBS card Identifier",
            "State license",
            "State registry ID",
            "Study Permit",
            "Subscriber Number",
            "Temporary Account Number",
            "Temporary Living Subject Number",
            "Temporary Medical Record Number",
            "Temporary Permanent Resident (Canada)",
            "Training License Number",
            "Treaty Number/ (Canada)",
            "Unique Specimen ID",
            "Unique master citizen number",
            "Universal Device Identifier",
            "Unspecified identifier",
            "VISA",
            "Visit number",
            "Visitor Permit",
            "WIC identifier",
            "Work Permit",
            "Workers' Comp Number",
            "Enitentiary/correctional institution Number",
            "Synthea identifier type",
            "Driver's License",
            ResourceTypes.OomniaExternalId,
            ResourceTypes.OomniaScreeningNumber
        };

        [DisplayName(CodeSetAttributeNames.OrganizationIdentifierType)]
        public static List<string> OrganizationIdentifierType { get; set; } = new List<string>()
        {
            "Accession ID",
            "Organization identifier",
            "Provider number",
            "Social Beneficiary Identifier",
            "Tax ID number",
            ResourceTypes.OomniaExternalId
        };

        [DisplayName(CodeSetAttributeNames.AddressType)]
        public static List<string> AddressType { get; set; } = new List<string>()
        {
            "Business",
            "Home",
            "Mailing",
            "Previous"
        };

        [DisplayName(CodeSetAttributeNames.Citizenship)]
        public static List<string> Citizenship { get; set; } = new List<string>()
        {
            "British",
            "Serbian",
            "Swiss"
        };

        [DisplayName(CodeSetAttributeNames.ReligiousAffiliationType)]
        public static List<string> ReligiousAffiliationType { get; set; } = new List<string>()
        {
            "Adventist",
            "African Religions",
            "Afro-Caribbean Religions",
            "Agnosticism",
            "Anglican",
            "Animism",
            "Assembly of God",
            "Atheism",
            "Babi & Baha'I faiths",
            "Baptist",
            "Bon",
            "Brethren",
            "Cao Dai",
            "Celticism",
            "Christian (non-Catholic, non-specific)",
            "Christian Scientist",
            "Church of Christ",
            "Church of God",
            "Confucianism",
            "Congregational",
            "Cyberculture Religions",
            "Disciples of Christ",
            "Divination",
            "Eastern Orthodox",
            "Episcopalian",
            "Evangelical Covenant",
            "Fourth Way",
            "Free Daism",
            "Friends",
            "Full Gospel",
            "Gnosis",
            "Hinduism",
            "Humanism",
            "Independent",
            "Islam",
            "Jainism",
            "Jehovah's Witnesses",
            "Judaism",
            "Latter Day Saints",
            "Lutheran",
            "Mahayana",
            "Meditation",
            "Messianic Judaism",
            "Methodist",
            "Mitraism",
            "Native American",
            "Nazarene",
            "New Age",
            "non-Roman Catholic",
            "Occult",
            "Orthodox",
            "Paganism",
            "Pentecostal",
            "Presbyterian",
            "Process, The",
            "Protestant",
            "Protestant, No Denomination",
            "Reformed",
            "Reformed/Presbyterian",
            "Roman Catholic Church",
            "Salvation Army",
            "Satanism",
            "Scientology",
            "Shamanism",
            "Shiite (Islam)",
            "Shinto",
            "Sikism",
            "Spiritualism",
            "Sunni (Islam)",
            "Taoism",
            "Theravada",
            "Unitarian Universalist",
            "Unitarian-Universalism",
            "United Church of Christ",
            "Universal Life Church",
            "Vajrayana (Tibetan)",
            "Veda",
            "Voodoo",
            "Wicca",
            "Yaohushua",
            "Zen Buddhism",
            "Zoroastrianism"
        };

        [DisplayName(CodeSetAttributeNames.FormDefinitionState)]
        public static List<string> FormDefinitionState { get; set; } = new List<string>()
        {
            "Design pending",
            "Design",
            "Review pending",
            "Review",
            "Ready for data capture",
            "Archive"
        };

        [DisplayName(CodeSetAttributeNames.Gender)]
        public static List<string> Gender { get; set; } = new List<string>()
        {
            "Male",
            "Female",
            "Other",
            "Unknown"
        };

        [DisplayName(CodeSetAttributeNames.EOCStatus)]
        public static List<string> EOCStatus { get; set; } = new List<string>()
        {
            "Planned",
            "Waitlist",
            "On hold",
            "Finished",
            "Cancelled",
            "Entered in error"
        };

        [DisplayName(CodeSetAttributeNames.TelecomSystemType)]
        public static List<string> TelecomSystemType { get; set; } = new List<string>()
        {
            "Phone",
            "Fax",
            "Email",
            "Pager",
            "Url",
            "Sms",
            "Other"
        };

        [DisplayName(CodeSetAttributeNames.TelecomUseType)]
        public static List<string> TelecomUseType { get; set; } = new List<string>()
        {
            "Home",
            "Work",
            "Temp",
            "Old",
            "Mobile"
        };

        [DisplayName(CodeSetAttributeNames.IdentifierUseType)]
        public static List<string> IdentifierUseType { get; set; } = new List<string>()
        {
            "Usual",
            "Official",
            "Temp",
            "Secondary",
            "Old"
        };

        [DisplayName(CodeSetAttributeNames.FormState)]
        public static List<string> FormState { get; set; } = new List<string>()
        {
            "Finished",
            "On going",
            "Signed"
        };

        [DisplayName(CodeSetAttributeNames.InstitutionalLegalForm)]
        public static List<string> InstitutionalLegalForm { get; set; } = new List<string>()
        {
            "Private practice",
            "State University Hospital",
            "Company with limited liabilities",
            "Private company",
            "Public company"
        };

        [DisplayName(CodeSetAttributeNames.InstitutionalOrganizationalForm)]
        public static List<string> InstitutionalOrganizationalForm { get; set; } = new List<string>()
        {
            "Institute",
            "Center",
            "Clinic"
        };

        [DisplayName(CodeSetAttributeNames.UserPrefix)]
        public static List<string> UserPrefix { get; set; } = new List<string>()
        {
            "Mr",
            "Mrs",
            "Ms"
        };

        [DisplayName(CodeSetAttributeNames.AcademicPosition)]
        public static List<string> AcademicPosition { get; set; } = new List<string>()
        {
            "Professor",
            "Assistant professsor",
            "Doctor of philosophy",
            "Privatdozent"
        };

        [DisplayName(CodeSetAttributeNames.ClinicalTrialRecruitmentsStatus)]
        public static List<string> ClinicalTrialRecruitmentsStatus { get; set; } = new List<string>()
        {
            "Not yet recruiting",
            "Recruiting",
            "Enrolling by invitation",
            "Not recruiting",
            "Suspended",
            "Terminated",
            "Completed",
            "Withdrawn",
            "Unknown",
            "Active"
        };

        [DisplayName(CodeSetAttributeNames.ClinicalTrialRole)]
        public static List<string> ClinicalTrialRole { get; set; } = new List<string>()
        {
            "Principal investigator",
            "Co-Investigator",
            "Study nurse",
            "Clinical research administrator",
            "Medical monitor",
            "Clinical trial pharmacist"
        };

        [DisplayName(CodeSetAttributeNames.VersionType)]
        public static List<string> VersionType { get; set; } = new List<string>()
        {
            "MAJOR",
            "MINOR",
            "PATCH"
        };

        [DisplayName(CodeSetAttributeNames.PredifinedGlobalUserRole)]
        public static List<string> PredifinedGlobalUserRole { get; set; } = new List<string>()
        {
            "Super administrator",
            "Viewer",
            "Editor",
            "Curator"
        };

        [DisplayName(CodeSetAttributeNames.PersonnelType)]
        public static List<string> PersonnelType { get; set; } = new List<string>()
        {
            "External system",
            "Internal personnel",
            "External personnel"
        };

        [DisplayName(CodeSetAttributeNames.EntityState)]
        public static List<string> EntityState { get; set; } = new List<string>()
        {
            "Active",
            "Merged",
            "Deleted"
        };

        [DisplayName(CodeSetAttributeNames.IdentifierPool)]
        public static List<string> IdentifierPool { get; set; } = new List<string>()
        { 
            
        };

        [DisplayName(CodeSetAttributeNames.SourceSystem)]
        public static List<string> SourceSystem { get; set; } = new List<string>()
        {
        };

        [DisplayName(CodeSetAttributeNames.TransactionDirection)]
        public static List<string> TransactionDirection { get; set; } = new List<string>()
        {
            "Inbound",
            "Outbound"
        };

        [DisplayName(CodeSetAttributeNames.ErrorType)]
        public static List<string> ErrorType { get; set; } = new List<string>()
        {
            "AR",
            "AE"
        };

        [DisplayName(CodeSetAttributeNames.TelecommunicationUseType)]
        public static List<ThesaurusEntry> TelecommunicationUseType { get; set; } = new List<ThesaurusEntry>()
        {
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Primary Residence Number",
                        Definition = "Primary Residence Number"
                    },
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.DE,
                        PreferredTerm = "Hauptwohnsitznummer",
                        Definition = "Hauptwohnsitznummer"
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Other Residence Number",
                        Definition = "Other Residence Number"
                    },
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.DE,
                        PreferredTerm = "Nebenwohnsitznummer",
                        Definition = "Nebenwohnsitznummer"
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Work Number",
                        Definition = "Work Number"
                    },
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.DE,
                        PreferredTerm = "Dienstnummer",
                        Definition = "Dienstnummer"
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Vacation Home Number",
                        Definition = "Vacation Home Number"
                    },
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.DE,
                        PreferredTerm = "Ferienwohnsitznummer",
                        Definition = "Ferienwohnsitznummer"
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Answering Service Number",
                        Definition = "Answering Service Number"
                    },
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.DE,
                        PreferredTerm = "Auftragsdienst",
                        Definition = "Auftragsdienst"
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Emergency Number",
                        Definition = "Emergency Number"
                    },
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.DE,
                        PreferredTerm = "Notfallnummer",
                        Definition = "Notfallnummer"
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Network (email) Address",
                        Definition = "Network (email) Address"
                    },
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.DE,
                        PreferredTerm = "Netzwerkadresse (E-Mail)",
                        Definition = "Netzwerkadresse (E-Mail)"
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Beeper Number",
                        Definition = "Beeper Number"
                    },
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.DE,
                        PreferredTerm = "Pagernummer (Cityruf o.ä.)",
                        Definition = "Pagernummer (Cityruf o.ä.)"
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Personal",
                        Definition = "Personal"
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Direct Secure Messaging",
                        Definition = "Direct Secure Messaging using the Direct Protocol"
                    }
                }
            }
        };

        [DisplayName(CodeSetAttributeNames.ContactRelationship)]
        public static List<ThesaurusEntry> ContactRelationship { get; set; } = new List<ThesaurusEntry>()
        {
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Billing contact person",
                        Definition = "Billing contact person"
                    },
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.DE,
                        PreferredTerm = "Kontaktperson für Abrechnung",
                        Definition = "Kontaktperson für Abrechnung"
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Contact person",
                        Definition = "Contact person"
                    },
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.DE,
                        PreferredTerm = "Kontaktperson",
                        Definition = "Kontaktperson"
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Emergency contact person",
                        Definition = "Emergency contact person"
                    },
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.DE,
                        PreferredTerm = "Kontaktperson für Notfälle",
                        Definition = "Kontaktperson für Notfälle"
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Person preparing referral",
                        Definition = "Person preparing referral"
                    },
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.DE,
                        PreferredTerm = "Person, die die Überweisung vorbereitet",
                        Definition = "Person, die die Überweisung vorbereitet"
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Employer",
                        Definition = "Employer"
                    },
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.DE,
                        PreferredTerm = "Arbeitgeber",
                        Definition = "Arbeitgeber"
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Emergency Contact",
                        Definition = "Emergency Contact"
                    },
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.DE,
                        PreferredTerm = "Ansprechpartner in Notfällen",
                        Definition = "Ansprechpartner in Notfällen"
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Federal Agency",
                        Definition = "Federal Agency"
                    },
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.DE,
                        PreferredTerm = "Bundesbehörde",
                        Definition = "Bundesbehörde"
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Insurance Company",
                        Definition = "Insurance Company"
                    },
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.DE,
                        PreferredTerm = "Versicherung",
                        Definition = "Versicherung"
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Next-of-Kin",
                        Definition = "Next-of-Kin"
                    },
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.DE,
                        PreferredTerm = "Kontaktperson",
                        Definition = "Kontaktperson"
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "State Agency",
                        Definition = "State Agency"
                    },
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.DE,
                        PreferredTerm = "Landesbehörde",
                        Definition = "Landesbehörde"
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Unknown",
                        Definition = "Unknown"
                    },
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.DE,
                        PreferredTerm = "unbekannt",
                        Definition = "unbekannt"
                    }
                }
            }
        };

        [DisplayName(CodeSetAttributeNames.ContactRole)]
        public static List<ThesaurusEntry> ContactRole { get; set; } = new List<ThesaurusEntry>()
        {
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Associate",
                        Definition = "Associate"
                    },
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.DE,
                        PreferredTerm = "Collègue",
                        Definition = "Collègue"
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Brother",
                        Definition = "Brother"
                    },
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.DE,
                        PreferredTerm = "Bruder",
                        Definition = "Bruder"
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Care giver",
                        Definition = "Care giver"
                    },
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.DE,
                        PreferredTerm = "Pflegeeltern",
                        Definition = "Pflegeeltern"
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Child",
                        Definition = "Child"
                    },
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.DE,
                        PreferredTerm = "Kind",
                        Definition = "Kind"
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Handicapped dependent",
                        Definition = "Handicapped dependent"
                    },
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.DE,
                        PreferredTerm = "Begleitperson für Hilfsbedürftige",
                        Definition = "Begleitperson für Hilfsbedürftige"
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Life partner",
                        Definition = "Life partner"
                    },
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.DE,
                        PreferredTerm = "Lebenspartner",
                        Definition = "Lebenspartner"
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Emergency contact",
                        Definition = "Emergency contact"
                    },
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.DE,
                        PreferredTerm = "Ansprechpartner in Notfällen",
                        Definition = "Ansprechpartner in Notfällen"
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Employee",
                        Definition = "Employee"
                    },
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.DE,
                        PreferredTerm = "Angestellter",
                        Definition = "Angestellter"
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Employer",
                        Definition = "Employer"
                    },
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.DE,
                        PreferredTerm = "Arbeitgeber",
                        Definition = "Arbeitgeber"
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Extended family",
                        Definition = "Extended family"
                    },
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.DE,
                        PreferredTerm = "erweiterter Familienkreis",
                        Definition = "erweiterter Familienkreis"
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Foster child",
                        Definition = "Foster child"
                    },
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.DE,
                        PreferredTerm = "Pflegekind",
                        Definition = "Pflegekind"
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Friend",
                        Definition = "Friend"
                    },
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.DE,
                        PreferredTerm = "Freund",
                        Definition = "Freund"
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Father",
                        Definition = "Father"
                    },
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.DE,
                        PreferredTerm = "Vater",
                        Definition = "Vater"
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Grandchild",
                        Definition = "Grandchild"
                    },
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.DE,
                        PreferredTerm = "Enkel",
                        Definition = "Enkel"
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Guardian",
                        Definition = "Guardian"
                    },
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.DE,
                        PreferredTerm = "Pate",
                        Definition = "Pate"
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Grandparent",
                        Definition = "Grandparent"
                    },
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.DE,
                        PreferredTerm = "Großeltern",
                        Definition = "Großeltern"
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Manager",
                        Definition = "Manager"
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Mother",
                        Definition = "Mother"
                    },
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.DE,
                        PreferredTerm = "Mutter",
                        Definition = "Mutter"
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Natural child",
                        Definition = "Natural child"
                    },
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.DE,
                        PreferredTerm = "eigenes Kind",
                        Definition = "eigenes Kind"
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "None",
                        Definition = "None"
                    },
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.DE,
                        PreferredTerm = "keine",
                        Definition = "keine"
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Other adult",
                        Definition = "Other adult"
                    },
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.DE,
                        PreferredTerm = "anderer Erwachsener",
                        Definition = "anderer Erwachsener"
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Other",
                        Definition = "Other"
                    },
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.DE,
                        PreferredTerm = "andere",
                        Definition = "andere"
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Owner",
                        Definition = "Owner"
                    },
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.DE,
                        PreferredTerm = "Eigentümer",
                        Definition = "Eigentümer"
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Parent",
                        Definition = "Parent"
                    },
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.DE,
                        PreferredTerm = "Eltern",
                        Definition = "Eltern"
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Stepchild",
                        Definition = "Stepchild"
                    },
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.DE,
                        PreferredTerm = "Stiefkind",
                        Definition = "Stiefkind"
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Self",
                        Definition = "Self"
                    },
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.DE,
                        PreferredTerm = "selbst",
                        Definition = "selbst"
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Sibling",
                        Definition = "Sibling"
                    },
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.DE,
                        PreferredTerm = "Geschwister",
                        Definition = "Geschwister"
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Sister",
                        Definition = "Sister"
                    },
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.DE,
                        PreferredTerm = "Schwester",
                        Definition = "Schwester"
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Spouse",
                        Definition = "Spouse"
                    },
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.DE,
                        PreferredTerm = "Partner",
                        Definition = "Partner"
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Trainer",
                        Definition = "Trainer"
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Unknown",
                        Definition = "Unknown"
                    },
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.DE,
                        PreferredTerm = "unbekannt",
                        Definition = "unbekannt"
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Ward of court",
                        Definition = "Ward of court"
                    },
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.DE,
                        PreferredTerm = "Vormund",
                        Definition = "Vormund"
                    }
                }
            }
        };

        [DisplayName(CodeSetAttributeNames.MaritalStatus)]
        public static List<ThesaurusEntry> MaritalStatus { get; set; } = new List<ThesaurusEntry>()
        {
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Annulled",
                        Definition = "Marriage contract has been declared null and to not have existed"
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Divorced",
                        Definition = "Marriage contract has been declared dissolved and inactive"
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Interlocutory",
                        Definition = "Subject to an Interlocutory Decree."
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Legally Separated",
                        Definition = ""
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Married",
                        Definition = "A current marriage contract is active"
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Common Law",
                        Definition = "a marriage recognized in some jurisdictions and based on the parties' agreement to consider themselves married and can also be based on documentation of cohabitation."
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Polygamous",
                        Definition = "More than 1 current spouse"
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Domestic partner",
                        Definition = "Person declares that a domestic partner relationship exists"
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "unmarried",
                        Definition = "Currently not in a marriage contract"
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Never Married",
                        Definition = "No marriage contract has ever been entered"
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Widowed",
                        Definition = "The spouse has died"
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "unknown",
                        Definition = "**Description:**A proper value is applicable, but not known."
                    }
                }
            }
        };

        [DisplayName(CodeSetAttributeNames.CommunicationSystem)]
        public static List<string> CommunicationSystem { get; set; } = new List<string>()
        {
            "HL7 messages"
        };

        [DisplayName(CodeSetAttributeNames.TeamType)]
        public static List<ThesaurusEntry> TeamType { get; set; } = new List<ThesaurusEntry>()
        {
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Care Team",
                        Definition = "Team meant to provide care to patients"
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "CPR Team First Floor",
                        Definition = ""
                    }
                }
            }
        };

        [DisplayName(CodeSetAttributeNames.PersonnelTeamRelationshipType)]
        public static List<ThesaurusEntry> PersonnelTeamRelationshipType { get; set; } = new List<ThesaurusEntry>()
        {
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Team Leader",
                        Definition = "Leader of the Team"
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Team Member",
                        Definition = "Member of the Team"
                    }
                }
            }
        };

        [DisplayName(CodeSetAttributeNames.Role)]
        public static List<ThesaurusEntry> Role { get; set; } = new List<ThesaurusEntry>()
        {
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = PredifinedRole.SuperAdministrator.ToString(),
                        Definition = "A super administrator is a person who has full access to user and organizational modules, and other non-patient-related resources of the system. Super administrator can create users, add organizations and change general system properties."
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = PredifinedRole.Administrator.ToString(),
                        Definition = "A administrator is a person who has can manage users within an organization he belongs to and perform other non-patient related system tasks defined by the Super Administrator"
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = PredifinedRole.Doctor.ToString(),
                        Definition = "A doctor is a person who has can edit documents of patient's clinical trials within organization he is employed and do other activities defined by administrators."
                    }
                }
            }
        };

        [DisplayName(CodeSetAttributeNames.Language)]
        public static List<string> Language { get; set; } = new List<string>()
        {
             "Abkhazian" ,
             "Afar" ,
             "Afrikaans" ,
             "Akan" ,
             "Albanian" ,
             "Amharic" ,
             "Arabic" ,
             "Aragonese" ,
             "Armenian" ,
             "Assamese" ,
             "Avaric" ,
             "Avestan" ,
             "Aymara" ,
             "Azerbaijani" ,
             "Bambara" ,
             "Bashkir" ,
             "Basque" ,
             "Belarusian" ,
             "Bengali" ,
             "Bislama" ,
             "Bosnian" ,
             "Breton" ,
             "Bulgarian" ,
             "Burmese" ,
             "Catalan" ,
             "Chamorro" ,
             "Chechen" ,
             "Chichewa" ,
             "Chinese" ,
             "ChurchSlavonic" ,
             "Chuvash" ,
             "Cornish" ,
            "Corsican" ,
            "Cree" ,
            "Croatian" ,
            "Czech" ,
            "Danish" ,
            "Dhivehi" ,
            "Dutch" ,
            "Dzongkha" ,
            "English" ,
            "Esperanto" ,
            "Estonian" ,
            "Ewe" ,
            "Faroese",
            "Fijian",
            "Finnish",
            "French",
            "WesternFrisian",
            "Fulah",
            "Gaelic",
            "Galician",
            "Ganda",
            "Georgian",
            "German",
            "Greek",
            "Kalaallisut",
            "Guarani",
            "Gujarati",
            "Haitian",
            "Hausa",
            "Hebrew",
            "Herero",
            "Hindi",
            "HiriMotu",
            "Hungarian",
            "Icelandic",
            "Ido",
            "Igbo",
            "Indonesian",
            "Interlingua",
            "Interlingue",
            "Inuktitut",
            "Inupiaq",
            "Irish",
            "Italian",
            "Japanese",
            "Javanese",
            "Kannada",
            "Kanuri",
            "Kashmiri",
            "Kazakh",
            "CentralKhmer",
            "Kikuyu",
            "Kinyarwanda",
            "Kirghiz",
            "Komi",
            "Kongo",
            "Korean",
            "Kuanyama",
            "Kurdish",
            "Lao",
            "Latin",
            "Latvian",
            "Limburgan",
            "Lingala",
            "Lithuanian",
            "LubaKatanga",
            "Luxembourgish",
            "Macedonian",
            "Malagasy",
            "Malay",
            "Malayalam",
            "Maltese",
            "Manx",
            "Maori",
            "Marathi",
            "Marshallese",
            "Mongolian",
            "Nauru",
            "Navajo",
            "NorthNdebele",
            "SouthNdebele",
            "Ndonga",
            "Nepali",
            "Norwegian",
            "NorwegianBokmål",
            "NorwegianNynorsk",
            "SichuanYi",
            "Occitan",
            "Ojibwa",
            "Oriya",
            "Oromo",
            "Ossetian",
            "Pali",
            "Pashto",
            "Persian",
            "Polish",
            "Portuguese",
            "Punjabi",
            "Quechua",
            "Romanian",
            "Romansh",
            "Rundi",
            "Russian",
            "NorthernSami",
            "Samoan",
            "Sango",
            "Sanskrit",
            "Sardinian",
            "Serbian",
            "Shona",
            "Sindhi",
            "Sinhala",
            "Slovak",
            "Slovenian",
            "Somali",
            "SouthernSotho",
            "Spanish",
            "Sundanese",
            "Swahili",
            "Swati",
            "Swedish",
            "Tagalog",
            "Tahitian",
            "Tajik",
            "Tamil",
            "Tatar",
            "Telugu",
            "Thai",
            "Tibetan",
            "Tigrinya",
            "Tonga",
            "Tsonga",
            "Tswana",
            "Turkish",
            "Turkmen",
            "Twi",
            "Uighur",
            "Ukrainian",
            "Urdu",
            "Uzbek",
            "Venda",
            "Vietnamese",
            "Volapük",
            "Walloon",
            "Welsh",
            "Wolof",
            "Xhosa",
            "Yiddish",
            "Yoruba",
            "Zhuang",
            "Zulu"
        };

        [DisplayName(CodeSetAttributeNames.ServiceType)]
        public static List<string> ServiceType { get; set; } = new List<string>()
        {
            "Aged Care Assessment",
            "Aged Care Information/Referral",
            "Aged Residential Care",
            "Case Management for Older Persons",
            "Delivered Meals (Meals On Wheels)",
            "Friendly Visiting",
            "Home Care/Housekeeping Assistance",
            "Home Maintenance and Repair",
            "Personal Alarms/Alerts",
            "Personal Care for Older Persons",
            "Planned Activity Groups",
            "Acupuncture",
            "Alexander Technique Therapy",
            "Aromatherapy",
            "Biorhythm Services",
            "Bowen Therapy",
            "Chinese Herbal Medicine",
            "Feldenkrais",
            "Homoeopathy",
            "Hydrotherapy",
            "Hypnotherapy",
            "Kinesiology",
            "Magnetic Therapy",
            "Massage Therapy",
            "Meditation",
            "Myotherapy",
            "Naturopathy",
            "Reflexology",
            "Reiki",
            "Relaxation Therapy",
            "Shiatsu",
            "Western Herbal Medicine",
            "Family Day care",
            "Holiday Programs",
            "Kindergarten Inclusion Support",
            "Kindergarten/Preschool",
            "Long Day Child Care",
            "Occasional Child Care",
            "Outside School Hours Care",
            "Children's Play Programs",
            "Parenting/Family Support/Education",
            "Playgroup",
            "School Nursing",
            "Toy Library",
            "Child Protection/Child Abuse Report",
            "Foster Care",
            "Residential/Out-of-Home Care",
            "Support - Young People Leaving Care",
            "Audiology",
            "Blood Donation",
            "Chiropractic",
            "Dietetics",
            "Family Planning",
            "Health Advocacy/Liaison Service",
            "Health Information/Referral",
            "Immunization",
            "Maternal & Child Health",
            "Nursing",
            "Nutrition",
            "Occupational Therapy",
            "Optometry",
            "Osteopathy",
            "Pharmacy",
            "Physiotherapy",
            "Podiatry",
            "Sexual Health",
            "Speech Pathology/Therapy",
            "Bereavement Counselling",
            "Crisis Counselling",
            "Family Counselling/Therapy",
            "Family Violence Counselling",
            "Financial Counselling",
            "Generalist Counselling",
            "Genetic Counselling",
            "Health Counselling",
            "Mediation",
            "Problem Gambling Counselling",
            "Relationship Counselling",
            "Sexual Assault Counselling",
            "Trauma Counselling",
            "Victims of Crime Counselling",
            "Cemetery Operation",
            "Cremation",
            "Death Service Information",
            "Funeral Services",
            "Endodontic",
            "General Dental",
            "Oral Medicine",
            "Oral Surgery",
            "Orthodontic",
            "Paediatric Dentistry",
            "Periodontic",
            "Prosthodontic",
            "Acquired Brain Injury Info/Referral",
            "Disability Advocacy",
            "Disability Aids & Equipment",
            "Disability Case Management",
            "Disability Day Programs/Activities",
            "Disability Information/Referral",
            "Disability Support Packages",
            "Disability Supported Accommodation",
            "Early Childhood Intervention",
            "Hearing Aids & Equipment",
            "Drug and/or Alcohol Counselling",
            "Drug/Alcohol Information/Referral",
            "Needle & Syringe Exchange",
            "Non-resid. Alcohol/Drug Treatment",
            "Pharmacotherapy",
            "Quit Program",
            "Residential Alcohol/Drug Treatment",
            "Adult/Community Education",
            "Higher Education",
            "Primary Education",
            "Secondary Education",
            "Training & Vocational Education",
            "Emergency Medical",
            "Employment Placement and/or Support",
            "Vocational Rehabilitation",
            "Work Safety/Accident Prevention",
            "Financial Assistance",
            "Financial Information/Advice",
            "Material Aid",
            "General Practice",
            "Accommodation Placement/Support",
            "Crisis/Emergency Accommodation",
            "Homelessness Support",
            "Housing Information/Referral",
            "Public Rental Housing",
            "Interpreting/Multilingual Service",
            "Juvenile Justice",
            "Legal Advocacy",
            "Legal Information/Advice/Referral",
            "Mental Health Advocacy",
            "Mental Health Assess/Triage/Crisis Response",
            "Mental Health Case Management",
            "Mental Health Information/Referral",
            "Mental Health Inpatient Services",
            "Mental Health Non-residential Rehab",
            "Mental Health Residential Rehab/CCU",
            "Psychiatry (Requires Referral)",
            "Psychology",
            "Martial Arts",
            "Personal Fitness Training",
            "Physical Activity Group",
            "Physical Activity Programs",
            "Physical Fitness Testing",
            "Pilates",
            "Self-Defence",
            "Sporting Club",
            "Yoga",
            "Food Safety",
            "Health Regulatory /Inspection /Cert.",
            "Work Health/Safety Inspection/Cert.",
            "Carer Support",
            "Respite Care",
            "Anatomical Pathology",
            "Pathology - Clinical Chemistry",
            "Pathology - General",
            "Pathology - Genetics",
            "Pathology - Haematology",
            "Pathology - Immunology",
            "Pathology - Microbiology",
            "Anaesthesiology - Pain Medicine",
            "Cardiology",
            "Clinical Genetics",
            "Clinical Pharmacology",
            "Dermatology",
            "Endocrinology",
            "Gastroenterology & Hepatology",
            "Geriatric Medicine",
            "Immunology & Allergy",
            "Infectious Diseases",
            "Intensive Care Medicine",
            "Medical Oncology",
            "Nephrology",
            "Neurology",
            "Occupational Medicine",
            "Palliative Medicine",
            "Public Health Medicine",
            "Rehabilitation Medicine",
            "Rheumatology",
            "Sleep Medicine",
            "Thoracic Medicine",
            "Gynaecological Oncology",
            "Obstetrics & Gynaecology",
            "Reproductive Endocrinology/Infertility",
            "Urogynaecology",
            "Neonatology & Perinatology",
            "Paediatric Cardiology",
            "Paediatric Clinical Genetics",
            "Paediatric Clinical Pharmacology",
            "Paediatric Endocrinology",
            "Paed. Gastroenterology/Hepatology",
            "Paediatric Haematology",
            "Paediatric Immunology & Allergy",
            "Paediatric Infectious Diseases",
            "Paediatric Intensive Care Medicine",
            "Paediatric Medical Oncology",
            "Paediatric Medicine",
            "Paediatric Nephrology",
            "Paediatric Neurology",
            "Paediatric Nuclear Medicine",
            "Paediatric Rehabilitation Medicine",
            "Paediatric Rheumatology",
            "Paediatric Sleep Medicine",
            "Paediatric Surgery",
            "Paediatric Thoracic Medicine",
            "Diag. Radiology /Xray /CT /Fluoroscopy",
            "Diagnostic Ultrasound",
            "Magnetic Resonance Imaging (MRI)",
            "Nuclear Medicine",
            "Obstetric/Gynaecological Ultrasound",
            "Radiation Oncology",
            "Cardiothoracic Surgery",
            "Neurosurgery",
            "Ophthalmology",
            "Orthopaedic Surgery",
            "Otolaryngology/Head & Neck Surgery",
            "Plastic & Reconstructive Surgery",
            "Surgery - General",
            "Urology",
            "Vascular Surgery",
            "Support Groups",
            "Air ambulance",
            "Ambulance",
            "Blood Transport",
            "Community Bus",
            "Flying Doctor Service",
            "Patient Transport",
            "A&E",
            "A&EP",
            "Abuse",
            "ACAS",
            "Access",
            "Accident",
            "Acute Inpatient Serv",
            "Adult Day Programs",
            "Adult Mental Health Services",
            "Advice",
            "Advocacy",
            "Aged Persons Mental",
            "Aged Persons Mental Health Services",
            "Aged Persons Mental Health Teams",
            "Aids",
            "Al-Anon",
            "Alcohol",
            "Al-Teen",
            "Antenatal",
            "Anxiety",
            "Arthritis",
            "Assessment",
            "Assistance",
            "Asthma",
            "ATSS",
            "Attendant Care",
            "Babies",
            "Bathroom Modificatio",
            "Behavior",
            "Behavior Interventi",
            "Bereavement",
            "Bipolar",
            "Birth",
            "Birth Control",
            "Birthing Options",
            "BIST",
            "Blood",
            "Bone",
            "Bowel",
            "Brain",
            "Breast Feeding",
            "Breast Screen",
            "Brokerage",
            "Cancer",
            "Cancer Support",
            "Cardiovascular Disea",
            "Care Packages",
            "Carer",
            "Case Management",
            "Casualty",
            "Centrelink",
            "Chemists",
            "Child And Adolescent",
            "Child Care",
            "Child Services",
            "Children",
            "Children's Services",
            "Cholesterol",
            "Clothing",
            "Community Based Acco",
            "Community Care Unit",
            "Community Child And",
            "Community Health",
            "Community Residentia",
            "Community Transport",
            "Companion Visiting",
            "Companionship",
            "Consumer Advice",
            "Consumer Issues",
            "Continuing Care Serv",
            "Contraception Inform",
            "Coordinating Bodies",
            "Correctional Service",
            "Council Environmenta",
            "Counselling",
            "Criminal",
            "Crises",
            "Crisis Assessment An",
            "Crisis Assistance",
            "Crisis Refuge",
            "Day Program",
            "Deaf",
            "Dental Hygiene",
            "Dentistry",
            "Dentures",
            "Depression",
            "Detoxification",
            "Diabetes",
            "Diaphragm Fitting",
            "Dieticians",
            "Disabled Parking",
            "District Nursing",
            "Divorce",
            "Doctors",
            "Drink-Drive",
            "Dual Diagnosis Servi",
            "Early Choice",
            "Eating Disorder",
            "Emergency Relief",
            "Employment And Train",
            "Environment",
            "Equipment",
            "Exercise",
            "Facility",
            "Family Choice",
            "Family Law",
            "Family Options",
            "Family Services",
            "FFYA",
            "Financial Aid",
            "Fitness",
            "Flexible Care Packag",
            "Food",
            "Food Vouchers",
            "Forensic Mental Heal",
            "Futures",
            "Futures For Young Ad",
            "General Practitioner",
            "Grants",
            "Grief",
            "Grief Counselling",
            "HACC",
            "Heart Disease",
            "Help",
            "High Blood Pressure",
            "Home Help",
            "Home Nursing",
            "Homefirst",
            "Hospice Care",
            "Hospital Services",
            "Hospital To Home",
            "Hostel",
            "Hostel Accommodation",
            "Household Items",
            "Hypertension",
            "Illness",
            "Independent Living",
            "Information",
            "Injury",
            "Intake",
            "Intensive Mobile You",
            "Intervention",
            "Job Searching",
            "Justice",
            "Leisure",
            "Loans",
            "Low Income Earners",
            "Lung",
            "Making A Difference",
            "Medical Services",
            "Medical Specialists",
            "Medication Administr",
            "Menstrual Informatio",
            "Methadone",
            "Mobile Support And T",
            "Motor Neurone",
            "Multiple Sclerosis",
            "Neighbourhood House",
            "Nursing Home",
            "Nursing Mothers",
            "Obesity",
            "Occupational Health",
            "Optometrist",
            "Oral Hygiene",
            "Outpatients",
            "Outreach Service",
            "PADP",
            "Pain",
            "Pap Smear",
            "Parenting",
            "Peak Organizations",
            "Personal Care",
            "Pharmacies",
            "Phobias",
            "Physical",
            "Physical Activity",
            "Postnatal",
            "Pregnancy",
            "Pregnancy Tests",
            "Preschool",
            "Prescriptions",
            "Primary Mental Healt",
            "Property Maintenance",
            "Prostate",
            "Psychiatric",
            "Psychiatric Disabili",
            "Psychiatric Disability Support Services - Planned Respite",
            "Psychiatric Disability Support Services - Residential Rehabilitation",
            "Psychiatric Disability Support Services Home-Based Outreach",
            "Psychiatric Disability Support Services Mutual Support And Self Help",
            "Psychiatric Support",
            "Recreation",
            "Referral",
            "Refuge",
            "Rent Assistance",
            "Residential Faciliti",
            "Residential Respite",
            "Respiratory",
            "Response",
            "Rooming Houses",
            "Safe Sex",
            "Secure Extended Care",
            "Self Help",
            "Separation",
            "Services",
            "Sex Education",
            "Sexual Abuse",
            "Sexual Issues",
            "Sexually Transmitted",
            "SIDS",
            "Social Support",
            "Socialisation",
            "Special Needs",
            "Speech Therapist",
            "Splinting",
            "Sport",
            "Statewide And Specia",
            "STD",
            "STI",
            "Stillbirth",
            "Stomal Care",
            "Stroke",
            "Substance Abuse",
            "Support",
            "Syringes",
            "Teeth",
            "Tenancy Advice",
            "Terminal Illness",
            "Therapy",
            "Transcription",
            "Translating Services",
            "Translator",
            "Transport",
            "Vertebrae",
            "Violence",
            "Vocational Guidance",
            "Weight",
            "Welfare Assistance",
            "Welfare Counselling",
            "Wheelchairs",
            "Wound Management",
            "Young People At Risk",
            "Community Health Care",
            "Library",
            "Community Hours",
            "Specialist Medical",
            "Hepatology",
            "Gastroenterology",
            "Gynaecology",
            "Obstetrics",
            "Specialist Surgical",
            "Placement Protection",
            "Family Violence",
            "Integrated Family Services",
            "Diabetes Educator",
            "Kinship Care",
            "General Mental Health Services",
            "Exercise Physiology",
            "Medical Research",
            "Youth",
            "Youth Services",
            "Youth Health",
            "Child and Family Services",
            "Home Visits",
            "Mobile Services",
            "Before and/or After",
            "Cancer Services",
            "Integrated Cancer Services",
            "Multidisciplinary Services",
            "Multidisciplinary Cancer Services",
            "Meetings",
            "Blood pressure monitoring",
            "Dose administration",
            "Medical Equipment Hire",
            "Deputising Service",
            "Cancer Support Groups",
            "Community Cancer Services",
            "Disability Care Transport",
            "Aged Care Transport",
            "Diabetes Education services",
            "Cardiac Rehabilitation",
            "Young Adult Diabetes",
            "Pulmonary Rehabilitation",
            "Art therapy",
            "Medication Reviews",
            "Telephone Counselling",
            "Telephone Help Line",
            "Online Service",
            "Crisis - Mental Health",
            "Youth Crisis",
            "Sexual Assault",
            "GPAH Other",
            "Paediatric Dermatology",
            "Veterans Services",
            "Veterans",
            "Food Relief/Food/Meals",
            "Dementia Care",
            "Alzheimer",
            "Drug and/or Alcohol Support Groups",
            "1-on-1 Support/Mentoring/Coaching",
            "Chronic Disease Management",
            "Liaison Services",
            "Walk-in Centre/Non-Emergency",
            "Inpatients",
            "Spiritual Counselling",
            "Women's Health",
            "Men's Health",
            "Health Education/Awareness Program",
            "Test Message",
            "Remedial Massage",
            "Adolescent Mental Health Services",
            "Youth Drop In/Assistance/Support",
            "Aboriginal Health Worker",
            "Women's Health Clinic",
            "Men's Health Clinic",
            "Migrant Health Clinic",
            "Refugee Health Clinic",
            "Aboriginal Health Clinic",
            "Nurse Practitioner Lead Clinic/s",
            "Nurse Lead Clinic/s",
            "Culturally Tailored Support Groups",
            "Culturally Tailored Health Promotion",
            "Rehabilitation",
            "Education Information/Referral",
            "Social Work",
            "Haematology",
            "Maternity Shared Care",
            "Rehabilitation Services",
            "Cranio-sacral Therapy",
            "Prosthetics & Orthotics",
            "Home Medicine Review",
            "GPAH - Medical",
            "Music Therapy",
            "Falls Prevention",
            "Accommodation/Tenancy",
            "Assess-Skill, Ability, Needs",
            "Assist Access/Maintain Employment",
            "Assist Prod-Personal Care/Safety",
            "Assist-Integrate School/Education",
            "Assist-Life Stage, Transition",
            "Assist-Personal Activities",
            "Assist-Travel/Transport",
            "Assistive Equip-General Tasks",
            "Assistive Equip-Recreation",
            "Assistive Prod-Household Tasks",
            "Behavior Support",
            "Comms & Info Equipment",
            "Community Nursing Care",
            "Daily Tasks/Shared Living",
            "Development-Life Skills",
            "Early Childhood Supports",
            "Equipment Special Assess Setup",
            "Hearing Equipment",
            "Home Modification",
            "Household Tasks",
            "Interpret/Translate",
            "Other Innovative Supports",
            "Participate Community",
            "Personal Mobility Equipment",
            "Physical Wellbeing",
            "Plan Management",
            "Therapeutic Supports",
            "Training-Travel Independence",
            "Vehicle modifications",
            "Vision Equipment"
        };

        [DisplayName(CodeSetAttributeNames.TaskType)]
        public static List<ThesaurusEntry> TaskType { get; set; } = new List<ThesaurusEntry>()
        {
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Complete Questionnaire",
                        Definition = "Complete Questionnaire",
                    }
                }
            }
        };

        [DisplayName(CodeSetAttributeNames.TaskStatus)]
        public static List<ThesaurusEntry> TaskStatus { get; set; } = new List<ThesaurusEntry>()
        {
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Cancelled",
                        Definition = "Cancelled",
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Completed",
                        Definition = "Completed",
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Pending",
                        Definition = "Pending",
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "In Process",
                        Definition = "In Process",
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Requested",
                        Definition = "Requested",
                    }
                }
            }
        };

        [DisplayName(CodeSetAttributeNames.TaskPriority)]
        public static List<ThesaurusEntry> TaskPriority { get; set; } = new List<ThesaurusEntry>()
        {
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Urgent",
                        Definition = "Urgent",
                    }
                }
            },
                        new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Normal",
                        Definition = "Normal",
                    }
                }
            }
        };

        [DisplayName(CodeSetAttributeNames.TaskClass)]
        public static List<ThesaurusEntry> TaskClass { get; set; } = new List<ThesaurusEntry>()
        {
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Scheduled",
                        Definition = "Scheduled",
                    }
                }
            },
                        new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Unscheduled",
                        Definition = "Unscheduled",
                    }
                }
            }
        };

        [DisplayName(CodeSetAttributeNames.TaskDocument)]
        public static List<ThesaurusEntry> TaskDocument { get; set; } = new List<ThesaurusEntry>() {};


        [DisplayName(CodeSetAttributeNames.OrganizationCommunicationEntity)]
        public static List<string> OrganizationCommunicationEntity { get; set; } = new List<string>()
        {
        };

        [DisplayName(CodeSetAttributeNames.ClinicalTrialIdentifiers)]
        public static List<ThesaurusEntry> ClinicalTrialIdentifiers { get; set; } = new List<ThesaurusEntry>()
        {
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Ethics Commission of the Canton of Bern Approval Number",
                        Definition = "Ethics Commission of the Canton of Bern Approval Number",
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "ClinicalTrials.gov identifier",
                        Definition = "ClinicalTrials.gov identifier",
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Swiss National Clinical Trials Portal Identifier",
                        Definition = "Swiss National Clinical Trials Portal Identifier",
                    }
                }
            }
        };

        [DisplayName(CodeSetAttributeNames.ClinicalTrialSponsorIdentifierType)]
        public static List<ThesaurusEntry> ClinicalTrialSponsorIdentifierType { get; set; } = new List<ThesaurusEntry>()
        {
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Academic center",
                        Definition = "Academic center",
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Industry",
                        Definition = "Industry",
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Government",
                        Definition = "Government",
                    }
                }
            },
            new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                {
                    new ThesaurusEntryTranslation()
                    {
                        Language = LanguageConstants.EN,
                        PreferredTerm = "Non- Government Organizations",
                        Definition = "Non- Government Organizations",
                    }
                }
            }
        };

        [DisplayName(CodeSetAttributeNames.OccupationCategory)]
        public static List<string> OccupationCategory { get; set; } = new List<string>()
        {
            "Health Care Worker",
            "Legislators and Senior Officials",
            "Business Services and Administration Managers",
            "Professional Services Managers",
            "Other Services Managers",
            "University and Higher Education Teachers",
            "Librarians, Archivists and Curators"
        };

        [DisplayName(CodeSetAttributeNames.OccupationSubCategory)]
        public static List<string> OccupationSubCategory { get; set; } = new List<string>()
        {
            "Medical Doctors",
            "Nursing and Midwifery Professionals",
            "Traditional and Complementary Medicine Professionals",
            "Paramedical Practitioners",
            "Veterinarians",
            "Other Health Professionals",
            "Medical and Pharmaceutical Technicians",
            "Nursing and Midwifery Associate Professionals",
            "Traditional and Complementary Medicine Associate Professionals",
            "Veterinary Technicians and Assistants",
            "Other Health Associate Professionals"
        };

        [DisplayName(CodeSetAttributeNames.Occupation)]
        public static List<string> Occupation { get; set; } = new List<string>()
        {
            "Health Service Managers",
            "Health Professionals",
            "Other Health Professionals",
            "Environmental and Occupational Health and Hygiene Professionals",
            "Health Professionals Not Elsewhere Classified",
            "Health Associate Professionals",
            "Other Health Associate Professionals",
            "Medical Records and Health Information Technicians",
            "Community Health Workers",
            "Environmental and Occupational Health Inspectors and Associates",
            "Health Associate Professionals Not Elsewhere Classified",
            "Personal Care Workers in Health Services",
            "Health Care Assistants"
        };

        [DisplayName(CodeSetAttributeNames.PersonnelSeniority)]
        public static List<string> PersonnelSeniority { get; set; } = new List<string>()
        {
            "Chief physician",
            "Deputy chief physician",
            "Head physician",
            "Senior hospital specialist",
            "Hospital Specialist",
            "Experienced senior physicians level I"
        };

        [DisplayName(CodeSetAttributeNames.ProjectType)]
        public static List<string> ProjectType { get; set; } = new List<string>()
        {
            "Clinical Trial"
        };

        [DisplayName(CodeSetAttributeNames.RelationType)]
        public static List<string> RelationType { get; set; } = new List<string>()
        {
            "Admitting doctor",
            "Attending doctor",
            "Consulting doctor",
            "Referring doctor"
        };

        [DisplayName(CodeSetAttributeNames.ResultStatus)]
        public static List<string> ResultStatus { get; set; } = new List<string>()
        {
            "Active",
            "Anticipated",
            "Authorized",
            "Modified",
            "Canceled",
            "Dictated",
            "In Error",
            "Processing",
            "In Progress",
            "Corrected",
            "Not Done",
            "Superseded",
            "Transcribed",
            "Started",
            "OnGoing",
            "Signed",
            "Finished"
        };

        [DisplayName(CodeSetAttributeNames.Document)]
        public static List<string> Document { get; set; } = new List<string>()
        {
            
        };

        [DisplayName(CodeSetAttributeNames.NullFlavor)]
        public static List<string> NullFlavor { get; set; } = new List<string>()
        {
            "No information",
            "Invalid",
            "Derived",
            "Other",
            "Negative infinity",
            "Positive infinity",
            "Un-encoded",
            "Masked",
            "Not applicable",
            "Unknown",
            "Asked but unknown",
            "Temporarily unavailable",
            "Not asked",
            "Not available",
            "Sufficient quantity",
            "Trace",
            "Not present"
        };

        [DisplayName(CodeSetAttributeNames.MissingValueDate)]
        public static List<string> MissingValueDate { get; set; } = new List<string>()
        {
            "01/01/3000",
            "01/01/3001",
            "01/01/3002",
            "01/01/3003",
            "01/01/3004",
            "01/01/3005",
            "01/01/3006",
            "01/01/3007",
            "01/01/3008",
            "01/01/3009",
            "01/01/3010",
            "01/01/3011",
            "01/01/3012",
            "01/01/3013",
            "01/01/3014",
            "01/01/3015",
            "01/01/3016",
        };

        [DisplayName(CodeSetAttributeNames.MissingValueDateTime)]
        public static List<string> MissingValueDateTime { get; set; } = new List<string>()
        {
            "01/01/3000 00:00",
            "01/01/3001 00:00",
            "01/01/3002 00:00",
            "01/01/3003 00:00",
            "01/01/3004 00:00",
            "01/01/3005 00:00",
            "01/01/3006 00:00",
            "01/01/3007 00:00",
            "01/01/3008 00:00",
            "01/01/3009 00:00",
            "01/01/3010 00:00",
            "01/01/3011 00:00",
            "01/01/3012 00:00",
            "01/01/3013 00:00",
            "01/01/3014 00:00",
            "01/01/3015 00:00",
            "01/01/3016 00:00",
        };

        [DisplayName(CodeSetAttributeNames.MissingValueNumber)]
        public static List<string> MissingValueNumber { get; set; } = new List<string>()
        {
            "11111111",
            "11112222",
            "11113333",
            "11114444",
            "11115555",
            "11116666",
            "11117777",
            "11118888",
            "11119999",
            "22221111",
            "22222222",
            "22223333",
            "22224444",
            "22225555",
            "22226666",
            "22227777",
            "22228888",
        };

        [DisplayName(CodeSetAttributeNames.Country)]
        public static List<string> Country { get; set; } = new List<string>()
        {

        };


        [DisplayName(CodeSetAttributeNames.ClinicalDomain)]
        public static List<string> ClinicalDomain { get; set; } = new List<string>()
        {
            "Allergy",
            "Acupuncture",
            "Addiction Medicine",
            "Addiction Psychiatry",
            "Adolescent Medicine",
            "Aerodigestive Medicine",
            "Aerospace Medicine",
            "Anesthesiology",
            "Audiology",
            "Bariatric Surgery",
            "Birth Defects",
            "Blood Banking And Transfusion Medicine",
            "Bone Marrow Transplant",
            "Brain Injury",
            "Burn Management",
            "Cardiac Surgery",
            "Cardiovascular Disease",
            "Chemical Pathology",
            "Child And Adolescent Psychiatry",
            "Child And Adolescent Psychology",
            "Chiropractic Medicine",
            "Cleft And Craniofacial",
            "Clinical Biochemical Genetics",
            "Clinical Cardiac Electrophysiology",
            "Clinical Genetics",
            "Clinical Pathology",
            "Clinical Pharmacology",
            "Colon And Rectal Surgery",
            "Community Health Care",
            "Critical Care Medicine",
            "Dentistry",
            "Developmental Behavioral Pediatrics",
            "Diabetology",
            "Dialysis",
            "Eating Disorders",
            "Emergency Medicine",
            "Environmental Health",
            "Epilepsy",
            "Ethics",
            "Family Medicine",
            "Forensic Medicine",
            "General Medicine",
            "Geriatric Medicine",
            "Gynecologic Oncology",
            "Gynecology",
            "Heart Failure",
            "Hepatology",
            "HIV",
            "Infectious Disease",
            "Integrative Medicine",
            "Interventional Cardiology",
            "Interventional Radiology",
            "Kinesiotherapy",
            "Maternal And Fetal Medicine",
            "Medical Aid In Dying",
            "Medical Genetics",
            "Medical Microbiology Pathology",
            "Medical Oncology",
            "Medical Toxicology",
            "Mental Health",
            "Multi Specialty Program",
            "Neonatal Perinatal Medicine",
            "Neurological Surgery",
            "Neurology W Special Qualifications In Child Neuro",
            "Neuropsychology",
            "Nutrition And Dietetics",
            "Obesity Medicine",
            "Obstetrics",
            "Obstetrics And Gynecology",
            "Occupational Therapy",
            "Optometry",
            "Orthopaedic Surgery",
            "Orthotics Prosthetics",
            "Otolaryngology",
            "Pain Medicine",
            "Palliative Care",
            "Pastoral Care",
            "Pediatric Cardiology",
            "Pediatric Critical Care Medicine",
            "Pediatric Dermatology",
            "Pediatric Endocrinology",
            "Pediatric Gastroenterology",
            "Pediatric Hematology Oncology",
            "Pediatric Infectious Diseases",
            "Pediatric Nephrology",
            "Pediatric Otolaryngology",
            "Pediatric Pulmonology",
            "Pediatric Rehabilitation Medicine",
            "Pediatric Rheumatology",
            "Pediatrics",
            "Pediatric Surgery",
            "Pediatric Transplant Hepatology",
            "Pediatric Urology",
            "Pharmacogenomics",
            "Physical Medicine And Rehab",
            "Physical Therapy",
            "Podiatry",
            "Polytrauma",
            "Primary Care",
            "Psychology",
            "Pulmonary Disease",
            "Recreational Therapy",
            "Reproductive Endocrinology And Infertility",
            "Research",
            "Respiratory Therapy",
            "Sleep Medicine",
            "Solid Organ Transplant",
            "Speech Language Pathology",
            "Spinal Cord Injury Medicine",
            "Spinal Surgery",
            "Sports Medicine",
            "Surgery",
            "Surgery Of The Hand",
            "Surgical Critical Care",
            "Surgical Oncology",
            "Therapeutic Apheresis",
            "Thoracic And Cardiac Surgery",
            "Thromboembolism",
            "Transplant Cardiology",
            "Transplant Surgery",
            "Trauma",
            "Tumor Board",
            "Undersea And Hyperbaric Medicine",
            "Vascular Neurology",
            "Vocational Rehabilitation",
            "Women's Health",
            "Wound Care Management",
            "Wound Ostomy And Continence Care",
            "Accident And Emergency Medicine",
            "Allergology",
            "Anaesthetics",
            "Cardiology",
            "Child Psychiatry",
            "Clinical Biology",
            "Clinical Chemistry",
            "Clinical Neurophysiology",
            "Craniofacial Surgery",
            "Dermatology",
            "Endocrinology",
            "Family And General Medicine",
            "Gastroenterologic Surgery",
            "Gastroenterology",
            "General Practice",
            "General Surgery",
            "Geriatrics",
            "Hematology",
            "Immunology",
            "Infectious Diseases",
            "Internal Medicine",
            "Laboratory Medicine",
            "Microbiology",
            "Nephrology",
            "Neuropsychiatry",
            "Neurology",
            "Neurosurgery",
            "Nuclear Medicine",
            "Obstetrics And Gynaecology",
            "Occupational Medicine",
            "Oncology",
            "Ophthalmology",
            "Oral And Maxillofacial Surgery",
            "Orthopaedics",
            "Otorhinolaryngology",
            "Paediatric Surgery",
            "Paediatrics",
            "Pathology",
            "Pharmacology",
            "Physical Medicine And Rehabilitation",
            "Plastic Surgery",
            "Podiatric Surgery",
            "Preventive Medicine",
            "Psychiatry",
            "Public Health",
            "Radiation Oncology",
            "Radiology",
            "Respiratory Medicine",
            "Rheumatology",
            "Stomatology",
            "Thoracic Surgery",
            "Tropical Medicine",
            "Urology",
            "Vascular Surgery",
            "Venereology",
            "Pre Clinical Research",
            "Veterinary Medicine",
            "Clinical Microbiology"
        };

        [DisplayName(CodeSetAttributeNames.Contraception)]
        public static List<string> Contraception { get; set; } = new List<string>()
        {
            "Needed",
            "Not Necessary"
        };

        [DisplayName(CodeSetAttributeNames.DiseaseContext)]
        public static List<string> DiseaseContext { get; set; } = new List<string>()
        {
            "Primary",
            "Secondary"
        };

        [DisplayName(CodeSetAttributeNames.InstanceState)]
        public static List<string> InstanceState { get; set; } = new List<string>()
        {
            "Active",
            "Archived"
        };

        [DisplayName(CodeSetAttributeNames.ChemotherapySchemaInstanceActionType)]
        public static List<string> ChemotherapySchemaInstanceActionType { get; set; } = new List<string>()
        {
            "Delay Dose",
            "Save Dose",
            "Delete Dose",
            "Save Instance",
            "Add Medication",
            "Replace Medication",
            "Delete Medication"
        };

        [DisplayName(CodeSetAttributeNames.ThesaurusState)]
        public static List<string> ThesaurusState { get; set; } = new List<string>()
        {
            "Draft",
            "Production",
            "Deprecated",
            "Disabled",
            "Curated",
            "Uncurated",
            "Metadata Incomplete",
            "Requires Discussion",
            "Pending Final Vetting",
            "Ready for Release",
            "To Be Replaced with External Ontology Term",
            "Organizational Term",
            "Example To Be Eventually Removed"
        };

        [DisplayName(CodeSetAttributeNames.ThesaurusMergeState)]
        public static List<string> ThesaurusMergeState { get; set; } = new List<string>()
        {
            "Pending",
            "Completed",
            "Not Completed"
        };

        [DisplayName(CodeSetAttributeNames.UserState)]
        public static List<string> UserState { get; set; } = new List<string>()
        {
            "Active",
            "Archived",
            "Blocked"
        };

        [DisplayName(CodeSetAttributeNames.CommentState)]
        public static List<string> CommentState { get; set; } = new List<string>()
        {
            "Created",
            "Archived",
            "Resolved",
            "Rejected"
        };

        [DisplayName(CodeSetAttributeNames.GlobalUserSource)]
        public static List<string> GlobalUserSource { get; set; } = new List<string>()
        {
            "Microsoft",
            "Google",
            "Internal"
        };

        [DisplayName(CodeSetAttributeNames.GlobalUserStatus)]
        public static List<string> GlobalUserStatus { get; set; } = new List<string>()
        {
            "Not Verified",
            "Active",
            "Blocked"
        };
    }
}
