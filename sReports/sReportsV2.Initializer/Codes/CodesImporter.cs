using Microsoft.Extensions.Configuration;
using sReportsV2.Common.Constants;
using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.Domain.Sql.Entities.CodeEntry;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using sReportsV2.Initializer.CodeSets;
using sReportsV2.SqlDomain.Interfaces;
using System.Collections.Generic;

namespace sReportsV2.Initializer.Codes
{
    public class CodesImporter
    {
        private readonly ICodeDAL codeDAL;
        private readonly ICodeSetDAL codeSetDAL;
        private readonly IThesaurusDAL thesaurusDAL;
        private readonly ICodeAssociationDAL codeAssociationDAL;
        private readonly IConfiguration configuration;

        public CodesImporter() { }

        public CodesImporter(ICodeDAL codeDAL, ICodeSetDAL codeSetDAL, IThesaurusDAL thesaurusDAL, ICodeAssociationDAL codeAssociationDAL, IConfiguration configuration)
        {
            this.codeDAL = codeDAL;
            this.codeSetDAL = codeSetDAL;
            this.thesaurusDAL = thesaurusDAL;
            this.codeAssociationDAL = codeAssociationDAL;
            this.configuration = configuration;
        }

        public void Import() 
        {
            UpdateCodes(CodeSetValues.OrganizationType, CodeSetAttributeNames.OrganizationType);
            UpdateCodes(CodeSetValues.EpisodeOfCareType, CodeSetAttributeNames.EpisodeOfCareType);
            UpdateCodes(CodeSetValues.EncounterType, CodeSetAttributeNames.EncounterType);
            UpdateCodes(CodeSetValues.EncounterStatus, CodeSetAttributeNames.EncounterStatus);
            UpdateCodes(CodeSetValues.EncounterClassification, CodeSetAttributeNames.EncounterClassification);
            UpdateCodes(CodeSetValues.DiagnosisRole, CodeSetAttributeNames.DiagnosisRole);
            UpdateCodes(CodeSetValues.PatientIdentifierType, CodeSetAttributeNames.PatientIdentifierType);
            UpdateCodes(CodeSetValues.OrganizationIdentifierType, CodeSetAttributeNames.OrganizationIdentifierType);
            UpdateCodes(CodeSetValues.AddressType, CodeSetAttributeNames.AddressType);
            UpdateCodes(CodeSetValues.Citizenship, CodeSetAttributeNames.Citizenship);
            UpdateCodes(CodeSetValues.ReligiousAffiliationType, CodeSetAttributeNames.ReligiousAffiliationType);
            UpdateCodes(CodeSetValues.FormDefinitionState, CodeSetAttributeNames.FormDefinitionState);
            UpdateCodes(CodeSetValues.Gender, CodeSetAttributeNames.Gender);
            UpdateCodes(CodeSetValues.EOCStatus, CodeSetAttributeNames.EOCStatus);
            UpdateCodes(CodeSetValues.TelecomSystemType, CodeSetAttributeNames.TelecomSystemType);
            UpdateCodes(CodeSetValues.TelecomUseType, CodeSetAttributeNames.TelecomUseType);
            UpdateCodes(CodeSetValues.IdentifierUseType, CodeSetAttributeNames.IdentifierUseType);
            UpdateCodes(CodeSetValues.FormState, CodeSetAttributeNames.FormState);
            UpdateCodes(CodeSetValues.InstitutionalLegalForm, CodeSetAttributeNames.InstitutionalLegalForm);
            UpdateCodes(CodeSetValues.InstitutionalOrganizationalForm, CodeSetAttributeNames.InstitutionalOrganizationalForm);
            UpdateCodes(CodeSetValues.UserPrefix, CodeSetAttributeNames.UserPrefix);
            UpdateCodes(CodeSetValues.AcademicPosition, CodeSetAttributeNames.AcademicPosition);
            UpdateCodes(CodeSetValues.ClinicalTrialRecruitmentsStatus, CodeSetAttributeNames.ClinicalTrialRecruitmentsStatus);
            UpdateCodes(CodeSetValues.ClinicalTrialRole, CodeSetAttributeNames.ClinicalTrialRole);
            UpdateCodes(CodeSetValues.VersionType, CodeSetAttributeNames.VersionType);
            UpdateCodes(CodeSetValues.PredifinedGlobalUserRole, CodeSetAttributeNames.PredifinedGlobalUserRole);
            UpdateCodes(CodeSetValues.TelecommunicationUseType, CodeSetAttributeNames.TelecommunicationUseType);
            UpdateCodes(CodeSetValues.ContactRelationship, CodeSetAttributeNames.ContactRelationship);
            UpdateCodes(CodeSetValues.ContactRole, CodeSetAttributeNames.ContactRole);
            UpdateCodes(CodeSetValues.MaritalStatus, CodeSetAttributeNames.MaritalStatus);
            UpdateCodes(CodeSetValues.CommunicationSystem, CodeSetAttributeNames.CommunicationSystem);
            UpdateCodes(CodeSetValues.TeamType, CodeSetAttributeNames.TeamType);
            UpdateCodes(CodeSetValues.PersonnelTeamRelationshipType, CodeSetAttributeNames.PersonnelTeamRelationshipType);
            UpdateCodes(CodeSetValues.PersonnelType, CodeSetAttributeNames.PersonnelType);
            UpdateCodes(CodeSetValues.Role, CodeSetAttributeNames.Role);
            UpdateCodes(CodeSetValues.EntityState, CodeSetAttributeNames.EntityState);
            UpdateCodes(CodeSetValues.IdentifierPool, CodeSetAttributeNames.IdentifierPool);
            UpdateCodes(CodeSetValues.TransactionDirection, CodeSetAttributeNames.TransactionDirection);
            UpdateCodes(CodeSetValues.SourceSystem, CodeSetAttributeNames.SourceSystem);
            UpdateCodes(CodeSetValues.ErrorType, CodeSetAttributeNames.ErrorType);
            UpdateCodes(CodeSetValues.Language, CodeSetAttributeNames.Language);
            UpdateCodes(CodeSetValues.ServiceType, CodeSetAttributeNames.ServiceType);
            UpdateCodes(CodeSetValues.TaskType, CodeSetAttributeNames.TaskType);
            UpdateCodes(CodeSetValues.TaskStatus, CodeSetAttributeNames.TaskStatus);
            UpdateCodes(CodeSetValues.TaskPriority, CodeSetAttributeNames.TaskPriority);
            UpdateCodes(CodeSetValues.TaskClass, CodeSetAttributeNames.TaskClass);
            UpdateCodes(CodeSetValues.TaskDocument, CodeSetAttributeNames.TaskDocument);
            UpdateCodes(CodeSetValues.OrganizationCommunicationEntity, CodeSetAttributeNames.OrganizationCommunicationEntity);
            UpdateCodes(CodeSetValues.ClinicalTrialIdentifiers, CodeSetAttributeNames.ClinicalTrialIdentifiers);
            UpdateCodes(CodeSetValues.ClinicalTrialSponsorIdentifierType, CodeSetAttributeNames.ClinicalTrialSponsorIdentifierType);
            UpdateCodes(CodeSetValues.OccupationCategory, CodeSetAttributeNames.OccupationCategory);
            UpdateCodes(CodeSetValues.OccupationSubCategory, CodeSetAttributeNames.OccupationSubCategory);
            UpdateCodes(CodeSetValues.Occupation, CodeSetAttributeNames.Occupation);
            UpdateCodes(CodeSetValues.PersonnelSeniority, CodeSetAttributeNames.PersonnelSeniority, true);
            UpdateCodes(CodeSetValues.ProjectType, CodeSetAttributeNames.ProjectType);
            UpdateCodes(CodeSetValues.RelationType, CodeSetAttributeNames.RelationType);
            UpdateCodes(CodeSetValues.ResultStatus, CodeSetAttributeNames.ResultStatus);
            UpdateCodes(CodeSetValues.NullFlavor, CodeSetAttributeNames.NullFlavor);
            UpdateCodes(CodeSetValues.MissingValueDate, CodeSetAttributeNames.MissingValueDate);
            UpdateCodes(CodeSetValues.MissingValueDateTime, CodeSetAttributeNames.MissingValueDateTime);
            UpdateCodes(CodeSetValues.MissingValueNumber, CodeSetAttributeNames.MissingValueNumber, true);
            UpdateCodes(CodeSetValues.ClinicalDomain, CodeSetAttributeNames.ClinicalDomain);
            UpdateCodes(CodeSetValues.Contraception, CodeSetAttributeNames.Contraception);
            UpdateCodes(CodeSetValues.DiseaseContext, CodeSetAttributeNames.DiseaseContext);
            UpdateCodes(CodeSetValues.InstanceState, CodeSetAttributeNames.InstanceState);
            UpdateCodes(CodeSetValues.ChemotherapySchemaInstanceActionType, CodeSetAttributeNames.ChemotherapySchemaInstanceActionType);
            UpdateCodes(CodeSetValues.ThesaurusState, CodeSetAttributeNames.ThesaurusState);
            UpdateCodes(CodeSetValues.ThesaurusMergeState, CodeSetAttributeNames.ThesaurusMergeState);
            UpdateCodes(CodeSetValues.UserState, CodeSetAttributeNames.UserState);
            UpdateCodes(CodeSetValues.CommentState, CodeSetAttributeNames.CommentState);
            UpdateCodes(CodeSetValues.GlobalUserSource, CodeSetAttributeNames.GlobalUserSource);
            UpdateCodes(CodeSetValues.GlobalUserStatus, CodeSetAttributeNames.GlobalUserStatus);
        }

        private void UpdateCodes(List<string> codes, string preferredTerm, bool addInitialAssociations = false)
        {
            int codeSetId = codeSetDAL.GetIdByPreferredTerm(preferredTerm);
            Dictionary<string, int> terms = new Dictionary<string, int>();

            foreach (var codeName in codes) 
            {
                Code code = codeDAL.GetByPreferredTerm(codeName, codeSetId);
                if (code == null)
                {
                    int? existingCodeId = codeDAL.GetIdByPreferredTermAndNullCodeset(codeName);
                    if (existingCodeId == null || existingCodeId == 0)
                    {
                        terms.Add(codeName, 0);
                    }
                    else
                    {
                        codeDAL.UpdateCodesetByCodeId((int)existingCodeId, codeSetId);
                    }
                }
            }
            InsertData(terms, codeSetId);

            if (addInitialAssociations)
            {
                if (preferredTerm == CodeSetAttributeNames.MissingValueNumber)
                {
                    AddMissingValueAssociations();
                }
                else
                {
                    int occupationCategoryCodeSetId = codeSetDAL.GetByPreferredTerm(CodeSetAttributeNames.OccupationCategory).CodeSetId;
                    int healthCareWorkerCodeId = codeDAL.GetByCodeSetIdAndPreferredTerm(occupationCategoryCodeSetId, "Health Care Worker");
                    var initialAssociation = codeAssociationDAL.GetByParentId(healthCareWorkerCodeId);
                    if (initialAssociation.Count == 0)
                    {
                        AddOccupationCategoryAssociations();
                        AddOccupationSubCategoryAssociations();
                    }
                }
            }
        }
 
        private void InsertData(Dictionary<string, int> terms, int codeSetId)
        {
            foreach (var term in terms)
            {
                int thesaurusId = InsertThesaurus(term);

                codeDAL.Insert(new Code()
                {
                    CodeId = term.Value,
                    ThesaurusEntryId = thesaurusId,
                    CodeSetId = codeSetId
                });
            }
        }

        private int InsertThesaurus(KeyValuePair<string, int> term) 
        {
            int thesaurusId;
            ThesaurusEntry thesaurusEntryDb = thesaurusDAL.GetByPreferredTerm(term.Key);

            if (thesaurusEntryDb != null)
                thesaurusId = thesaurusEntryDb.ThesaurusEntryId;
            else
            {
                ThesaurusEntry thesaurus = new ThesaurusEntry()
                {
                    Translations = new List<ThesaurusEntryTranslation>()
                        {
                            new ThesaurusEntryTranslation()
                            {
                                Language = LanguageConstants.EN,
                                PreferredTerm = term.Key,
                                Definition = term.Key
                            }
                        }
                };
                thesaurusDAL.InsertOrUpdate(thesaurus);
                thesaurusId = thesaurus.ThesaurusEntryId;
            }

            return thesaurusId;
        }

        private void UpdateCodes(List<ThesaurusEntry> thesauruses, string preferredTerm)
        {
            int codeSetId = codeSetDAL.GetIdByPreferredTerm(preferredTerm);
            List<ThesaurusEntry> codesToAdd = new List<ThesaurusEntry>();

            foreach (var thesaurus in thesauruses)
            {
                string codeName = thesaurus.GetPreferredTermByTranslationOrDefault(LanguageConstants.EN);
                Code code = codeDAL.GetByPreferredTerm(codeName, codeSetId);
                if (code == null)
                {
                    codesToAdd.Add(thesaurus);
                }
            }
                InsertData(codesToAdd, codeSetId);
        }

        private void InsertData(List<ThesaurusEntry> codesToAdd, int codeSetId)
        {
            foreach (var codeToAdd in codesToAdd)
            {
                int thesaurusId;
                ThesaurusEntry thesarus = codeToAdd;
                string codeName = thesarus.GetPreferredTermByTranslationOrDefault(LanguageConstants.EN);
                ThesaurusEntry thesaurusEntryDb = thesaurusDAL.GetByPreferredTerm(codeName);

                if (thesaurusEntryDb != null)
                {
                    thesaurusId = thesaurusEntryDb.ThesaurusEntryId;
                }
                else
                {
                    ThesaurusEntry thesaurusToAdd = thesarus;
                    thesaurusDAL.InsertOrUpdate(thesaurusToAdd);
                    thesaurusId = thesaurusToAdd.ThesaurusEntryId;
                }
                codeDAL.Insert(new Code()
                {
                    ThesaurusEntryId = thesaurusId,
                    CodeSetId = codeSetId
                });
            }
        }

        private void AddOccupationCategoryAssociations()
        {
            List<CodeAssociation> codeAssociations = new List<CodeAssociation>();
            int occupationCategoryCodeSetId = codeSetDAL.GetByPreferredTerm(CodeSetAttributeNames.OccupationCategory).CodeSetId;
            int occupationSubCategoryCodeSetId = codeSetDAL.GetByPreferredTerm(CodeSetAttributeNames.OccupationSubCategory).CodeSetId;
            int healthCareWorkerCodeId = codeDAL.GetByCodeSetIdAndPreferredTerm(occupationCategoryCodeSetId, "Health Care Worker");

            var associationsToAdd = new[]
            {
                new CodeAssociation
                {
                    ParentId = healthCareWorkerCodeId,
                    ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(occupationSubCategoryCodeSetId, "Medical Doctors")
                },
                new CodeAssociation
                {
                    ParentId = healthCareWorkerCodeId,
                    ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(occupationSubCategoryCodeSetId, "Nursing and Midwifery Professionals")
                },
                new CodeAssociation
                {
                    ParentId = healthCareWorkerCodeId,
                    ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(occupationSubCategoryCodeSetId, "Traditional and Complementary Medicine Professionals")
                },
                new CodeAssociation
                {
                    ParentId = healthCareWorkerCodeId,
                    ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(occupationSubCategoryCodeSetId, "Paramedical Practitioners")
                },
                new CodeAssociation
                {
                    ParentId = healthCareWorkerCodeId,
                    ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(occupationSubCategoryCodeSetId, "Veterinarians")
                },
                new CodeAssociation
                {
                    ParentId = healthCareWorkerCodeId,
                    ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(occupationSubCategoryCodeSetId, "Other Health Professionals")
                }
            };

            codeAssociations.AddRange(associationsToAdd);
            codeAssociationDAL.Insert(codeAssociations);
        }

        private void AddOccupationSubCategoryAssociations()
        {
            List<CodeAssociation> codeAssociations = new List<CodeAssociation>();
            int occupationSubCategoryCodeSetId = codeSetDAL.GetByPreferredTerm(CodeSetAttributeNames.OccupationSubCategory).CodeSetId;
            int occupationCodeSetId = codeSetDAL.GetByPreferredTerm(CodeSetAttributeNames.Occupation).CodeSetId;
            int medicalDoctorsCodeId = codeDAL.GetByCodeSetIdAndPreferredTerm(occupationSubCategoryCodeSetId, "Medical Doctors");
            int nursingCodeId = codeDAL.GetByCodeSetIdAndPreferredTerm(occupationSubCategoryCodeSetId, "Nursing and Midwifery Professionals");
            int medicineProfessionalsCodeId = codeDAL.GetByCodeSetIdAndPreferredTerm(occupationSubCategoryCodeSetId, "Traditional and Complementary Medicine Professionals");
            int veterinariansCodeId = codeDAL.GetByCodeSetIdAndPreferredTerm(occupationSubCategoryCodeSetId, "Veterinarians");
            int medicalAndPharmaceuticalTechniciansCodeId = codeDAL.GetByCodeSetIdAndPreferredTerm(occupationSubCategoryCodeSetId, "Medical and Pharmaceutical Technicians");

            var associationsToAdd = new[]
            {
                new CodeAssociation
                {
                    ParentId = medicalDoctorsCodeId,
                    ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(occupationCodeSetId, "Health Professionals")
                },
                new CodeAssociation
                {
                    ParentId = medicalDoctorsCodeId,
                    ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(occupationCodeSetId, "Medical Records and Health Information Technicians")
                },
                new CodeAssociation
                {
                    ParentId = medicalDoctorsCodeId,
                    ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(occupationCodeSetId, "Community Health Workers")
                },
                new CodeAssociation
                {
                    ParentId = nursingCodeId,
                    ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(occupationCodeSetId, "Health Professionals")
                },
                new CodeAssociation
                {
                    ParentId = nursingCodeId,
                    ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(occupationCodeSetId, "Health Associate Professionals")
                },
                new CodeAssociation
                {
                    ParentId = nursingCodeId,
                    ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(occupationCodeSetId, "Health Care Assistants")
                },
                new CodeAssociation
                {
                    ParentId = medicineProfessionalsCodeId,
                    ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(occupationCodeSetId, "Health Professionals")
                },
                new CodeAssociation
                {
                    ParentId = veterinariansCodeId,
                    ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(occupationCodeSetId, "Health Professionals")
                },
                new CodeAssociation
                {
                    ParentId = medicalAndPharmaceuticalTechniciansCodeId,
                    ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(occupationCodeSetId, "Medical Records and Health Information Technicians")
                }
            };

            codeAssociations.AddRange(associationsToAdd);
            codeAssociationDAL.Insert(codeAssociations);
        }

        private void AddMissingValueAssociations()
        {
            List<CodeAssociation> codeAssociations = new List<CodeAssociation>();
            int nullFlavorsCodeSetId = codeSetDAL.GetByPreferredTerm(CodeSetAttributeNames.NullFlavor).CodeSetId;
            int missingValueDateCodeSetId = codeSetDAL.GetByPreferredTerm(CodeSetAttributeNames.MissingValueDate).CodeSetId;
            int missingValueDateTimeCodeSetId = codeSetDAL.GetByPreferredTerm(CodeSetAttributeNames.MissingValueDateTime).CodeSetId;
            int missingValueNumberCodeSetId = codeSetDAL.GetByPreferredTerm(CodeSetAttributeNames.MissingValueNumber).CodeSetId;

            var notApplicableCodeId = codeDAL.GetByCodeSetIdAndPreferredTerm(nullFlavorsCodeSetId, "Not Applicable");
            var initialAssociation = codeAssociationDAL.GetByParentId(notApplicableCodeId);

            if (initialAssociation.Count == 0)
            {
                var associationsToAdd = new[]
                {
                    new CodeAssociation
                    {
                        ParentId = notApplicableCodeId,
                        ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(missingValueDateCodeSetId, "01/01/3000")
                    },
                    new CodeAssociation
                    {
                        ParentId = notApplicableCodeId,
                        ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(missingValueDateTimeCodeSetId, "01/01/3000 00:00")
                    },
                    new CodeAssociation
                    {
                        ParentId = notApplicableCodeId,
                        ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(missingValueNumberCodeSetId, "11111111")
                    },

                    new CodeAssociation
                    {
                        ParentId = codeDAL.GetByCodeSetIdAndPreferredTerm(nullFlavorsCodeSetId, "Unknown"),
                        ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(missingValueDateCodeSetId, "01/01/3001")
                    },
                    new CodeAssociation
                    {
                        ParentId = codeDAL.GetByCodeSetIdAndPreferredTerm(nullFlavorsCodeSetId, "Unknown"),
                        ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(missingValueDateTimeCodeSetId, "01/01/3001 00:00")
                    },
                    new CodeAssociation
                    {
                        ParentId = codeDAL.GetByCodeSetIdAndPreferredTerm(nullFlavorsCodeSetId, "Unknown"),
                        ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(missingValueNumberCodeSetId, "11112222")
                    },

                    new CodeAssociation
                    {
                        ParentId = codeDAL.GetByCodeSetIdAndPreferredTerm(nullFlavorsCodeSetId, "Asked but unknown"),
                        ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(missingValueDateCodeSetId, "01/01/3002")
                    },
                    new CodeAssociation
                    {
                        ParentId = codeDAL.GetByCodeSetIdAndPreferredTerm(nullFlavorsCodeSetId, "Asked but unknown"),
                        ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(missingValueDateTimeCodeSetId, "01/01/3002 00:00")
                    },
                    new CodeAssociation
                    {
                        ParentId = codeDAL.GetByCodeSetIdAndPreferredTerm(nullFlavorsCodeSetId, "Asked but unknown"),
                        ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(missingValueNumberCodeSetId, "11113333")
                    },

                     new CodeAssociation
                    {
                        ParentId = codeDAL.GetByCodeSetIdAndPreferredTerm(nullFlavorsCodeSetId, "Temporarily unavailable"),
                        ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(missingValueDateCodeSetId, "01/01/3003")
                    },
                    new CodeAssociation
                    {
                        ParentId = codeDAL.GetByCodeSetIdAndPreferredTerm(nullFlavorsCodeSetId, "Temporarily unavailable"),
                        ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(missingValueDateTimeCodeSetId, "01/01/3003 00:00")
                    },
                    new CodeAssociation
                    {
                        ParentId = codeDAL.GetByCodeSetIdAndPreferredTerm(nullFlavorsCodeSetId, "Temporarily unavailable"),
                        ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(missingValueNumberCodeSetId, "11114444")
                    },

                     new CodeAssociation
                    {
                        ParentId = codeDAL.GetByCodeSetIdAndPreferredTerm(nullFlavorsCodeSetId, "Not asked"),
                        ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(missingValueDateCodeSetId, "01/01/3004")
                    },
                    new CodeAssociation
                    {
                        ParentId = codeDAL.GetByCodeSetIdAndPreferredTerm(nullFlavorsCodeSetId, "Not asked"),
                        ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(missingValueDateTimeCodeSetId, "01/01/3004 00:00")
                    },
                    new CodeAssociation
                    {
                        ParentId = codeDAL.GetByCodeSetIdAndPreferredTerm(nullFlavorsCodeSetId, "Not asked"),
                        ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(missingValueNumberCodeSetId, "11115555")
                    },

                     new CodeAssociation
                    {
                        ParentId = codeDAL.GetByCodeSetIdAndPreferredTerm(nullFlavorsCodeSetId, "Not available"),
                        ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(missingValueDateCodeSetId, "01/01/3005")
                    },
                    new CodeAssociation
                    {
                        ParentId = codeDAL.GetByCodeSetIdAndPreferredTerm(nullFlavorsCodeSetId, "Not available"),
                        ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(missingValueDateTimeCodeSetId, "01/01/3005 00:00")
                    },
                    new CodeAssociation
                    {
                        ParentId = codeDAL.GetByCodeSetIdAndPreferredTerm(nullFlavorsCodeSetId, "Not available"),
                        ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(missingValueNumberCodeSetId, "11116666")
                    },

                    new CodeAssociation
                    {
                        ParentId = codeDAL.GetByCodeSetIdAndPreferredTerm(nullFlavorsCodeSetId, "Sufficient quantity"),
                        ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(missingValueDateCodeSetId, "01/01/3006")
                    },
                    new CodeAssociation
                    {
                        ParentId = codeDAL.GetByCodeSetIdAndPreferredTerm(nullFlavorsCodeSetId, "Sufficient quantity"),
                        ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(missingValueDateTimeCodeSetId, "01/01/3006 00:00")
                    },
                    new CodeAssociation
                    {
                        ParentId = codeDAL.GetByCodeSetIdAndPreferredTerm(nullFlavorsCodeSetId, "Sufficient quantity"),
                        ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(missingValueNumberCodeSetId, "11117777")
                    },

                    new CodeAssociation
                    {
                        ParentId = codeDAL.GetByCodeSetIdAndPreferredTerm(nullFlavorsCodeSetId, "Trace"),
                        ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(missingValueDateCodeSetId, "01/01/3007")
                    },
                    new CodeAssociation
                    {
                        ParentId = codeDAL.GetByCodeSetIdAndPreferredTerm(nullFlavorsCodeSetId, "Trace"),
                        ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(missingValueDateTimeCodeSetId, "01/01/3007 00:00")
                    },
                    new CodeAssociation
                    {
                        ParentId = codeDAL.GetByCodeSetIdAndPreferredTerm(nullFlavorsCodeSetId, "Trace"),
                        ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(missingValueNumberCodeSetId, "11118888")
                    },

                    new CodeAssociation
                    {
                        ParentId = codeDAL.GetByCodeSetIdAndPreferredTerm(nullFlavorsCodeSetId, "No information"),
                        ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(missingValueDateCodeSetId, "01/01/3008")
                    },
                    new CodeAssociation
                    {
                        ParentId = codeDAL.GetByCodeSetIdAndPreferredTerm(nullFlavorsCodeSetId, "No information"),
                        ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(missingValueDateTimeCodeSetId, "01/01/3008 00:00")
                    },
                    new CodeAssociation
                    {
                        ParentId = codeDAL.GetByCodeSetIdAndPreferredTerm(nullFlavorsCodeSetId, "No information"),
                        ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(missingValueNumberCodeSetId, "11119999")
                    },

                    new CodeAssociation
                    {
                        ParentId = codeDAL.GetByCodeSetIdAndPreferredTerm(nullFlavorsCodeSetId, "Invalid"),
                        ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(missingValueDateCodeSetId, "01/01/3009")
                    },
                    new CodeAssociation
                    {
                        ParentId = codeDAL.GetByCodeSetIdAndPreferredTerm(nullFlavorsCodeSetId, "Invalid"),
                        ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(missingValueDateTimeCodeSetId, "01/01/3009 00:00")
                    },
                    new CodeAssociation
                    {
                        ParentId = codeDAL.GetByCodeSetIdAndPreferredTerm(nullFlavorsCodeSetId, "Invalid"),
                        ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(missingValueNumberCodeSetId, "22221111")
                    },

                    new CodeAssociation
                    {
                        ParentId = codeDAL.GetByCodeSetIdAndPreferredTerm(nullFlavorsCodeSetId, "Derived"),
                        ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(missingValueDateCodeSetId, "01/01/3010")
                    },
                    new CodeAssociation
                    {
                        ParentId = codeDAL.GetByCodeSetIdAndPreferredTerm(nullFlavorsCodeSetId, "Derived"),
                        ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(missingValueDateTimeCodeSetId, "01/01/3010 00:00")
                    },
                    new CodeAssociation
                    {
                        ParentId = codeDAL.GetByCodeSetIdAndPreferredTerm(nullFlavorsCodeSetId, "Derived"),
                        ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(missingValueNumberCodeSetId, "22222222")
                    },

                    new CodeAssociation
                    {
                        ParentId = codeDAL.GetByCodeSetIdAndPreferredTerm(nullFlavorsCodeSetId, "Other"),
                        ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(missingValueDateCodeSetId, "01/01/3011")
                    },
                    new CodeAssociation
                    {
                        ParentId = codeDAL.GetByCodeSetIdAndPreferredTerm(nullFlavorsCodeSetId, "Other"),
                        ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(missingValueDateTimeCodeSetId, "01/01/3011 00:00")
                    },
                    new CodeAssociation
                    {
                        ParentId = codeDAL.GetByCodeSetIdAndPreferredTerm(nullFlavorsCodeSetId, "Other"),
                        ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(missingValueNumberCodeSetId, "22223333")
                    },

                    new CodeAssociation
                    {
                        ParentId = codeDAL.GetByCodeSetIdAndPreferredTerm(nullFlavorsCodeSetId, "Negative infinity"),
                        ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(missingValueDateCodeSetId, "01/01/3012")
                    },
                    new CodeAssociation
                    {
                        ParentId = codeDAL.GetByCodeSetIdAndPreferredTerm(nullFlavorsCodeSetId, "Negative infinity"),
                        ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(missingValueDateTimeCodeSetId, "01/01/3012 00:00")
                    },
                    new CodeAssociation
                    {
                        ParentId = codeDAL.GetByCodeSetIdAndPreferredTerm(nullFlavorsCodeSetId, "Negative infinity"),
                        ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(missingValueNumberCodeSetId, "22224444")
                    },

                    new CodeAssociation
                    {
                        ParentId = codeDAL.GetByCodeSetIdAndPreferredTerm(nullFlavorsCodeSetId, "Positive infinity"),
                        ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(missingValueDateCodeSetId, "01/01/3013")
                    },
                    new CodeAssociation
                    {
                        ParentId = codeDAL.GetByCodeSetIdAndPreferredTerm(nullFlavorsCodeSetId, "Positive infinity"),
                        ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(missingValueDateTimeCodeSetId, "01/01/3013 00:00")
                    },
                    new CodeAssociation
                    {
                        ParentId = codeDAL.GetByCodeSetIdAndPreferredTerm(nullFlavorsCodeSetId, "Positive infinity"),
                        ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(missingValueNumberCodeSetId, "22225555")
                    },

                    new CodeAssociation
                    {
                        ParentId = codeDAL.GetByCodeSetIdAndPreferredTerm(nullFlavorsCodeSetId, "Un-encoded"),
                        ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(missingValueDateCodeSetId, "01/01/3014")
                    },
                    new CodeAssociation
                    {
                        ParentId = codeDAL.GetByCodeSetIdAndPreferredTerm(nullFlavorsCodeSetId, "Un-encoded"),
                        ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(missingValueDateTimeCodeSetId, "01/01/3014 00:00")
                    },
                    new CodeAssociation
                    {
                        ParentId = codeDAL.GetByCodeSetIdAndPreferredTerm(nullFlavorsCodeSetId, "Un-encoded"),
                        ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(missingValueNumberCodeSetId, "22226666")
                    },

                    new CodeAssociation
                    {
                        ParentId = codeDAL.GetByCodeSetIdAndPreferredTerm(nullFlavorsCodeSetId, "Masked"),
                        ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(missingValueDateCodeSetId, "01/01/3015")
                    },
                    new CodeAssociation
                    {
                        ParentId = codeDAL.GetByCodeSetIdAndPreferredTerm(nullFlavorsCodeSetId, "Masked"),
                        ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(missingValueDateTimeCodeSetId, "01/01/3015 00:00")
                    },
                    new CodeAssociation
                    {
                        ParentId = codeDAL.GetByCodeSetIdAndPreferredTerm(nullFlavorsCodeSetId, "Masked"),
                        ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(missingValueNumberCodeSetId, "22227777")
                    },

                    new CodeAssociation
                    {
                        ParentId = codeDAL.GetByCodeSetIdAndPreferredTerm(nullFlavorsCodeSetId, "not present"),
                        ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(missingValueDateCodeSetId, "01/01/3016")
                    },
                    new CodeAssociation
                    {
                        ParentId = codeDAL.GetByCodeSetIdAndPreferredTerm(nullFlavorsCodeSetId, "not present"),
                        ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(missingValueDateTimeCodeSetId, "01/01/3016 00:00")
                    },
                    new CodeAssociation
                    {
                        ParentId = codeDAL.GetByCodeSetIdAndPreferredTerm(nullFlavorsCodeSetId, "not present"),
                        ChildId = codeDAL.GetByCodeSetIdAndPreferredTerm(missingValueNumberCodeSetId, "22228888")
                    },
                };

                codeAssociations.AddRange(associationsToAdd);
                codeAssociationDAL.Insert(codeAssociations);
            }
        }
    }
}
