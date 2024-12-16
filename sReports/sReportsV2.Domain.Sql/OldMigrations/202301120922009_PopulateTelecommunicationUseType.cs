using sReportsV2.Common.Constants;
using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Linq;

namespace sReportsV2.Domain.Sql.Migrations
{
    public partial class PopulateTelecommunicationUseType : DbMigration
    {
        public override void Up()
        {
            SReportsContext dbContext = new SReportsContext();

            bool hasEntities = false;
            try
            {
                hasEntities = hasEntities || dbContext.OrganizationTelecoms.Any();  //EntityCommandExecutionException
            }
            catch (EntityCommandExecutionException) { }

            hasEntities = hasEntities || dbContext.PatientContactTelecoms.Any();

            hasEntities = hasEntities || dbContext.PatientTelecoms.Any();

            //bool hasEntities = dbContext.OrganizationTelecoms.Any() || dbContext.PatientContactTelecoms.Any() || dbContext.PatientTelecoms.Any();
            if (hasEntities)
            {
                CodeMigrationHelper codeMigrationHelper = new CodeMigrationHelper(dbContext, GetTelecommunicationUseType(), true);
                codeMigrationHelper.InsertCodes(CodeSetAttributeNames.TelecommunicationUseType);
            }
        }
        
        public override void Down()
        {
            CodeMigrationHelper codeMigrationHelper = new CodeMigrationHelper(new SReportsContext());
            codeMigrationHelper.RemoveCodes(CodeSetAttributeNames.TelecommunicationUseType);
        }

        private List<ThesaurusEntry> GetTelecommunicationUseType()
        {
            return new List<ThesaurusEntry>()
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
        }
    }
}
