using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Helpers;
using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.Domain.Sql
{
    public class CodeMigrationHelper
    {
        private readonly SReportsContext dbContext;
        private readonly List<ThesaurusEntry> codeThesaurusToInsert;
        private readonly bool insertWithCustomSql;
        private readonly IConfiguration configuration;

        public CodeMigrationHelper(SReportsContext dbContext, IConfiguration configuration = null)
        {
            this.dbContext = dbContext;
            this.configuration = configuration;
        }

        public CodeMigrationHelper(SReportsContext dbContext, List<ThesaurusEntry> codeThesaurusToInsert, bool insertWithCustomSql) : this(dbContext)
        {
            this.codeThesaurusToInsert = codeThesaurusToInsert;
            this.insertWithCustomSql = insertWithCustomSql;
        }

        public void InsertCodes(string codeSetName, int codesetid = 0)
        {
            int codeSetId = InsertCodeSet(codeSetName, codesetid);
            Dictionary<string, int> terms = new Dictionary<string, int>();
            List<string> codeNames = new List<string>() { "Active", "Merged", "Deleted"};

            if (codesetid != 0)
            {
                foreach (var codeName in codeNames)
                {
                    terms.Add(codeName, 0);
                }
                InsertEntityStateCodes(codeSetId, terms);
            }
            else
                InsertCodes(codeSetId);
        }

        public void InsertCode(string codeSetName)
        {
            int codeSetId = InsertCodeSet(codeSetName);
            InsertCodes(codeSetId);
        }

        public void RemoveCodes(string codeSetName)
        {
            int codeSetId = dbContext.CodeSets.Where(x => x.ThesaurusEntry.Translations.Any(m => m.PreferredTerm == codeSetName))
                .Select(x => x.CodeSetId).FirstOrDefault();

            if (codeSetId != 0)
            {
                CodeSet codeSet = dbContext.CodeSets
                .FirstOrDefault(x => x.CodeSetId == codeSetId);
                dbContext.CodeSets.Remove(codeSet);

                var codesToDelete = dbContext.Codes.Where(c => c.CodeSetId == codeSetId);
                dbContext.Codes.RemoveRange(codesToDelete);
                dbContext.SaveChanges();
            }
        }

        private int InsertCodeSet(string codeSetName, int codesetid = 0)
        {
            int thesaurusId = InsertThesaurusForCodeSet(codeSetName);
            int codeSetId;

            if (codesetid != 0)
                codeSetId = codesetid;
            else
                codeSetId = dbContext.CodeSets.Select(cS => cS.CodeSetId).OrderByDescending(x => x).FirstOrDefault() + 1;
            
            CodeSet codeSet = new CodeSet(true)
            {
                CodeSetId = codeSetId,
                ThesaurusEntryId = thesaurusId
            };

            if (insertWithCustomSql)
            {
                string script = $@"
                    SET IDENTITY_INSERT dbo.CodeSets ON; 
                    insert into CodeSets (CodeSetId, ThesaurusEntryId, Active, IsDeleted, EntryDatetime) values ({codeSetId}, {thesaurusId}, 1, 0, GETDATE());
                    SET IDENTITY_INSERT dbo.CodeSets OFF; 
                ";
                string connectionString = configuration["Sql"];
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    using (SqlCommand sqlCommand = new SqlCommand(script, sqlConnection))
                    {
                        try
                        {
                            sqlConnection.Open();
                            sqlCommand.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            LogHelper.Error("Error while executing custom sql command, error: " + ex.Message);
                        }
                        finally
                        {
                            sqlConnection.Close();
                        }
                    }
                }
            }
            else
            {
                dbContext.CodeSets.Add(codeSet);
                dbContext.SaveChanges();
            }

            return codeSet.CodeSetId;
        }

        public  int InsertThesaurusForCodeSet(string codeSetName)
        {
            int thesaurusId;
            ThesaurusEntry codeSetThDB = GetByPreferredTerm(codeSetName);

            if (codeSetThDB != null)
            {
                thesaurusId = codeSetThDB.ThesaurusEntryId;
            }
            else
            {
                ThesaurusEntry thesaurus = new ThesaurusEntry(true)
                {
                    Translations = new List<ThesaurusEntryTranslation>()
                    {
                        new ThesaurusEntryTranslation()
                        {
                            Language = LanguageConstants.EN,
                            PreferredTerm = codeSetName,
                            Definition = codeSetName
                        }
                    }
                };
                InsertOrUpdate(thesaurus);
                thesaurusId = thesaurus.ThesaurusEntryId;
            }

            return thesaurusId;
        }

        public void RemoveInitialAssociations() 
        {
            int healthCareWorkerCodeId = dbContext.Codes.Where(x => x.ThesaurusEntry.Translations.Any(m => m.PreferredTerm == "Health Care Worker"))
                .Select(x => x.CodeId).FirstOrDefault();
            int medicalDoctorsCodeId = dbContext.Codes.Where(x => x.ThesaurusEntry.Translations.Any(m => m.PreferredTerm == "Medical Doctors"))
                .Select(x => x.CodeId).FirstOrDefault();
            int nursingCodeId = dbContext.Codes.Where(x => x.ThesaurusEntry.Translations.Any(m => m.PreferredTerm == "Nursing and Midwifery Professionals"))
                .Select(x => x.CodeId).FirstOrDefault();
            int medicineProfessionalsCodeId = dbContext.Codes.Where(x => x.ThesaurusEntry.Translations.Any(m => m.PreferredTerm == "Traditional and Complementary Medicine Professionals"))
                .Select(x => x.CodeId).FirstOrDefault();
            int veterinariansCodeId = dbContext.Codes.Where(x => x.ThesaurusEntry.Translations.Any(m => m.PreferredTerm == "Veterinarians"))
                .Select(x => x.CodeId).FirstOrDefault();
            int medicalAndPharmaceuticalTechniciansCodeId = dbContext.Codes.Where(x => x.ThesaurusEntry.Translations.Any(m => m.PreferredTerm == "Medical and Pharmaceutical Technicians"))
               .Select(x => x.CodeId).FirstOrDefault();

            var healthCareWorkerAssociationsToRemove = dbContext.CodeAssociations
             .Where(ca => ca.ParentId == healthCareWorkerCodeId)
             .ToList();
            dbContext.CodeAssociations.RemoveRange(healthCareWorkerAssociationsToRemove);
            var medicalDoctorsAssociationsToRemove = dbContext.CodeAssociations
             .Where(ca => ca.ParentId == medicalDoctorsCodeId)
             .ToList();
            dbContext.CodeAssociations.RemoveRange(medicalDoctorsAssociationsToRemove);
            var nursingAssociationsToRemove = dbContext.CodeAssociations
             .Where(ca => ca.ParentId == nursingCodeId)
             .ToList();
            dbContext.CodeAssociations.RemoveRange(nursingAssociationsToRemove);
            var medicineProfessionalsAssociationsToRemove = dbContext.CodeAssociations
             .Where(ca => ca.ParentId == medicineProfessionalsCodeId)
             .ToList();
            dbContext.CodeAssociations.RemoveRange(medicineProfessionalsAssociationsToRemove);
            var veterinariansAssociationsToRemove = dbContext.CodeAssociations
             .Where(ca => ca.ParentId == veterinariansCodeId)
             .ToList();
            dbContext.CodeAssociations.RemoveRange(veterinariansAssociationsToRemove);
            var medicalAndPharmaceuticalTechniciansToRemove = dbContext.CodeAssociations
             .Where(ca => ca.ParentId == medicalAndPharmaceuticalTechniciansCodeId)
             .ToList();
            dbContext.CodeAssociations.RemoveRange(medicalAndPharmaceuticalTechniciansToRemove);

            dbContext.SaveChanges();
        }

        private void InsertCodes(int codeSetId)
        {
            List<Tuple<ThesaurusEntry, int>> codesToAdd = PrepareCodes(codeSetId);
            InsertData(codesToAdd, codeSetId);
        }

        private void InsertEntityStateCodes(int codeSetId, Dictionary<string, int> terms)
        {
            string connectionString = configuration["Sql"];
            int i = 2001;

            foreach (var term in terms)
            {
                int thesaurusId = InsertThesaurus(term);

                string script = $@"SET IDENTITY_INSERT dbo.Codes ON;
                                   insert into Codes (CodeId, ThesaurusEntryId, CodeSetId, TypeCD, IsDeleted, EntryDatetime) values ({i}, {thesaurusId}, {codeSetId}, 0, 0, GETDATE());";
                ExecuteSqlRaw(script, connectionString);
                i++;
            }

            string setIdentityOff = $@"SET IDENTITY_INSERT dbo.Codes OFF;";
            ExecuteSqlRaw(setIdentityOff, connectionString);
        }

        private int InsertThesaurus(KeyValuePair<string, int> term)
        {
            int thesaurusId;
            ThesaurusEntry thesaurusEntryDb = GetByPreferredTerm(term.Key);

            if (thesaurusEntryDb != null)
                thesaurusId = thesaurusEntryDb.ThesaurusEntryId;
            else
            {
                ThesaurusEntry thesaurus = new ThesaurusEntry(true)
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
                InsertOrUpdate(thesaurus);
                thesaurusId = thesaurus.ThesaurusEntryId;
            }

            return thesaurusId;
        }

        private void ExecuteSqlRaw(string script, string connectionString)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                using (SqlCommand sqlCommand = new SqlCommand(script, sqlConnection))
                {
                    try
                    {
                        sqlConnection.Open();
                        sqlCommand.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error("Error while executing custom sql command, error: " + ex.Message);
                    }
                    finally
                    {
                        sqlConnection.Close();
                    }
                }
            }
        }

        private List<Tuple<ThesaurusEntry, int>> PrepareCodes(int codeSetId)
        {
            List<Tuple<ThesaurusEntry, int>> codesToAdd = new List<Tuple<ThesaurusEntry, int>>();

            foreach (var thesaurus in codeThesaurusToInsert)
            {
                string codeName = thesaurus.GetPreferredTermByTranslationOrDefault(LanguageConstants.EN);
                Code code = dbContext.Codes.Where(x => x.ThesaurusEntry.Translations
            .Any(m => m.PreferredTerm == codeName)).FirstOrDefault();
                if (code == null)
                {
                    codesToAdd.Add(new Tuple<ThesaurusEntry, int>(thesaurus, 0));
                }
                else
                {
                    if (code.CodeSetId == null)
                    {
                        code.CodeSetId = codeSetId;
                        InsertOrUpdate(code);
                    }
                    else if (code.CodeSetId != codeSetId)
                    {
                        codesToAdd.Add(new Tuple<ThesaurusEntry, int>(thesaurus, 0));
                    }
                }
            }
            return codesToAdd;
        }

        private void InsertData(List<Tuple<ThesaurusEntry, int>> codesToAdd, int codeSetId)
        {
            foreach (var codeToAdd in codesToAdd)
            {
                int thesaurusId = InsertThesaurusForCode(codeToAdd.Item1);
                InsertOrUpdate(new Code()
                {
                    CodeId = codeToAdd.Item2,
                    ThesaurusEntryId = thesaurusId,
                    CodeSetId = codeSetId
                });
            }
        }

        private int InsertThesaurusForCode(ThesaurusEntry thesarus)
        {
            int thesaurusId;
            string codeName = thesarus.GetPreferredTermByTranslationOrDefault(LanguageConstants.EN);
            ThesaurusEntry thesaurusEntryDb = GetByPreferredTerm(codeName);

            if (thesaurusEntryDb != null)
            {
                thesaurusId = thesaurusEntryDb.ThesaurusEntryId;
            }
            else
            {
                ThesaurusEntry thesaurusToAdd = thesarus;
                InsertOrUpdate(thesaurusToAdd);
                thesaurusId = thesaurusToAdd.ThesaurusEntryId;
            }

            return thesaurusId;
        }

        private void InsertOrUpdate(ThesaurusEntry thesaurus)
        {
            if (thesaurus.ThesaurusEntryId == 0)
            {
                dbContext.Thesauruses.Add(thesaurus);
            }
            else
            {
                thesaurus.SetLastUpdate();
            }

            dbContext.SaveChanges();
        }

        private void InsertOrUpdate(Code code)
        {
            Code codeFromDb = dbContext.Codes.FirstOrDefault(x => x.CodeId == code.CodeId);

            if (codeFromDb == null)
            {
                dbContext.Codes.Add(code);
            }
            else
            {
                if (codeFromDb.ThesaurusEntryId != code.ThesaurusEntryId)
                    codeFromDb.ThesaurusEntryId = code.ThesaurusEntryId;

                code.SetLastUpdate();
            }
            dbContext.SaveChanges();
        }

        private ThesaurusEntry GetByPreferredTerm(string preferredTerm)
        {
            return dbContext.ThesaurusEntryTranslations.Where(x => x.PreferredTerm == preferredTerm)
            .Select(x => x.ThesaurusEntry).FirstOrDefault();
        }

        public void InsertCodesFromList(string codeSetName, int codesetid, List<string> codeNames)
        {
            int codeSetId = AddCodeSet(codeSetName, codesetid);
            Dictionary<string, int> terms = new Dictionary<string, int>();
            foreach (var codeName in codeNames)
            {
                terms.Add(codeName, 0);
            }
            InsertEntityStateCodesFromList(codeSetId, terms);
        }

        private void InsertEntityStateCodesFromList(int codeSetId, Dictionary<string, int> terms)
        {
            foreach (var term in terms)
            {
                int thesaurusId = InsertThesaurus(term);

                if(!CheckIfExist(codeSetId, thesaurusId))
                {
                    Code code = new Code()
                    {
                        CodeSetId = codeSetId,
                        ThesaurusEntryId = thesaurusId
                    };
                    dbContext.Codes.Add(code);
                    dbContext.SaveChanges();
                }
            }
        }

        private int AddCodeSet(string codeSetName, int codesetid)
        {
            int thesaurusId = InsertThesaurusForCodeSet(codeSetName);

            var codeSetFromDb = GetById(codesetid);
            if (codeSetFromDb != null)
            {
                return codeSetFromDb.CodeSetId;
            }
            else
            {
                CodeSet codeSet = new CodeSet()
                {
                    CodeSetId = codesetid,
                    ThesaurusEntryId = thesaurusId
                };
                dbContext.CodeSets.Add(codeSet);
                dbContext.SaveChanges();

                return codeSet.CodeSetId;
            }
        }

        private CodeSet GetById(int codeSetId)
        {
            return dbContext.CodeSets.Where(x => x.CodeSetId == codeSetId).FirstOrDefault();
        }

        private bool CheckIfExist(int codeSetId, int thesaurusId)
        {
            return dbContext.Codes.Where(x => x.CodeSetId == codeSetId && x.ThesaurusEntryId == thesaurusId).Any();
        }
    }
}
