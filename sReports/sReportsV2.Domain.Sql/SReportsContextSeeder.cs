using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Enums;
using sReportsV2.DAL.Sql.Sql;
using System;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.Domain.Sql
{
    public static class SReportsContextSeeder
    {
        private const string DefaultDateTime = "9999-12-31 23:59:59.9999999 +00:00";

        public static void Seed(SReportsContext dbContext)
        {
            if (dbContext.CodeSets.Any())
            {
                InsertCodeSetCodes(dbContext, (int)CodeSetList.Contraception, CodeSetAttributeNames.Contraception, GetContraceptionList());
                InsertCodeSetCodes(dbContext, (int)CodeSetList.DiseaseContext, CodeSetAttributeNames.DiseaseContext, GetDiseaseContextList());
                InsertCodeSetCodes(dbContext, (int)CodeSetList.InstanceState, CodeSetAttributeNames.InstanceState, GetInstanceStateList());
                InsertCodeSetCodes(dbContext, (int)CodeSetList.ChemotherapySchemaInstanceActionType, CodeSetAttributeNames.ChemotherapySchemaInstanceActionType, GetChemotherapySchemaInstanceActionTypeList());
                InsertCodeSetCodes(dbContext, (int)CodeSetList.ThesaurusState, CodeSetAttributeNames.ThesaurusState, GetThesaurusStateList());
                InsertCodeSetCodes(dbContext, (int)CodeSetList.ThesaurusMergeState, CodeSetAttributeNames.ThesaurusMergeState, GetThesaurusMergeStateList());
                InsertCodeSetCodes(dbContext, (int)CodeSetList.UserState, CodeSetAttributeNames.UserState, GetUserStateList());
                InsertCodeSetCodes(dbContext, (int)CodeSetList.CommentState, CodeSetAttributeNames.CommentState, GetCommentStateList());
                InsertCodeSetCodes(dbContext, (int)CodeSetList.GlobalUserSource, CodeSetAttributeNames.GlobalUserSource, GetGlobalUserSourceList());
                InsertCodeSetCodes(dbContext, (int)CodeSetList.GlobalUserStatus, CodeSetAttributeNames.GlobalUserStatus, GetGlobalUserStatusList());
            }
        }

        private static void InsertCodeSetCodes(SReportsContext dbContext, int codeSetId, string codeSetName, List<string> codeList)
        {
            if (!dbContext.CodeSets.Any(x => x.CodeSetId == codeSetId))
            {
                int createdCodeSetId = CreateCodeSet(dbContext, codeSetName, codeSetId);

                foreach (var item in codeList)
                {
                    GetOrCreateCodeByPreferredTerm(dbContext, createdCodeSetId, item);
                }

                dbContext.SaveChanges();
            }
        }

        private static int CreateCodeSet(SReportsContext dbContext, string codeSetName, int codeSetId)
        {
            int thesaurusId = GetOrCreateThesaurusId(dbContext, codeSetName);

            const string insertQuery = @"
                INSERT INTO CodeSets (CodeSetId, ThesaurusEntryId, EntryDatetime, EntityStateCD, ActiveFrom, ActiveTo, ApplicableInDesigner) 
                VALUES (@CodeSetId, @ThesaurusEntryId, GETDATE(), @EntityStateCD, SYSDATETIMEOFFSET(), @DefaultDateTime, 0)";

            int? activeStateCodeId = GetActiveStateCodeId(dbContext);
            var parameters = new[]
            {
                new SqlParameter("@CodeSetId", codeSetId),
                new SqlParameter("@ThesaurusEntryId", thesaurusId),
                new SqlParameter("@EntityStateCD", activeStateCodeId.HasValue ? activeStateCodeId.Value : DBNull.Value),
                new SqlParameter("@DefaultDateTime", DefaultDateTime)
            };

            dbContext.Database.ExecuteSqlRaw(insertQuery, parameters);

            return codeSetId;
        }

        private static void GetOrCreateCodeByPreferredTerm(SReportsContext dbContext, int codeSetId, string codePreferredTerm)
        {
            int thesaurusId = GetOrCreateThesaurusId(dbContext, codePreferredTerm);
            int codeId = GetNextCodeId(dbContext);

            InsertNewCode(dbContext, codeId, thesaurusId, codeSetId);
        }

        private static int GetNextCodeId(SReportsContext dbContext)
        {
            const string query = @"SELECT TOP (1) CodeId FROM Codes ORDER BY CodeId DESC";

            var lastCodeId = dbContext.Codes
                                       .FromSqlRaw(query)
                                       .Select(c => c.CodeId)
                                       .FirstOrDefault();

            return lastCodeId + 1;
        }

        private static void InsertNewCode(SReportsContext dbContext, int codeId, int thesaurusId, int codeSetId)
        {
            const string query = @"
            SET IDENTITY_INSERT dbo.Codes ON;
            INSERT INTO Codes (CodeId, ThesaurusEntryId, CodeSetId, EntryDatetime, EntityStateCD, ActiveFrom, ActiveTo)
            VALUES (@CodeId, @ThesaurusEntryId, @CodeSetId, GETDATE(), @EntityStateCD, SYSDATETIMEOFFSET(), @DefaultDateTime);
            SET IDENTITY_INSERT dbo.Codes OFF;";

            int? activeStateCodeId = GetActiveStateCodeId(dbContext);
            var parameters = new SqlParameter[]
            {
            new SqlParameter("@CodeId", codeId),
            new SqlParameter("@ThesaurusEntryId", thesaurusId),
            new SqlParameter("@CodeSetId", codeSetId),
            new SqlParameter("@EntityStateCD", activeStateCodeId.HasValue ? activeStateCodeId.Value : DBNull.Value),
            new SqlParameter("@DefaultDateTime", DefaultDateTime)
            };

            dbContext.Database.ExecuteSqlRaw(query, parameters);
        }

        private static int GetOrCreateThesaurusId(SReportsContext dbContext, string preferredTerm, string definition = null)
        {
            const string selectQuery = @"
                SELECT ThesaurusEntryId 
                FROM ThesaurusEntryTranslations 
                WHERE PreferredTerm = @PreferredTerm";

            var parameters = new[] { new SqlParameter("@PreferredTerm", preferredTerm) };

            var thesaurusId = dbContext.ThesaurusEntryTranslations
                                        .FromSqlRaw(selectQuery, parameters)
                                        .Select(te => te.ThesaurusEntryId)
                                        .FirstOrDefault();

            if (thesaurusId <= 0)
            {
                thesaurusId = InsertNewThesaurusEntry(dbContext);

                InsertThesaurusEntryTranslation(dbContext, thesaurusId, preferredTerm, definition);
            }

            return thesaurusId;
        }

        private static int InsertNewThesaurusEntry(SReportsContext dbContext)
        {
            int? entityStateCode = GetActiveStateCodeId(dbContext);

            var idParameter = new SqlParameter
            {
                ParameterName = "@NewId",
                SqlDbType = System.Data.SqlDbType.Int,
                Direction = System.Data.ParameterDirection.Output
            };

            dbContext.Database.ExecuteSqlRaw(@"
                INSERT INTO ThesaurusEntries (EntryDatetime, EntityStateCD, ActiveFrom, ActiveTo)
                VALUES (GETDATE(), {0}, SYSDATETIMEOFFSET(), {1});
                SET @NewId = SCOPE_IDENTITY();",
                entityStateCode, DefaultDateTime, idParameter);

            return (int)idParameter.Value;
        }

        private static void InsertThesaurusEntryTranslation(SReportsContext dbContext, int thesaurusId, string preferredTerm, string definition)
        {
            const string query = @"
            INSERT INTO ThesaurusEntryTranslations (ThesaurusEntryId, Language, PreferredTerm, Definition)
            VALUES (@ThesaurusEntryId, @Language, @PreferredTerm, @Definition)";

            var translationParameters = new SqlParameter[]
            {
            new SqlParameter("@ThesaurusEntryId", thesaurusId),
            new SqlParameter("@Language", LanguageConstants.EN),
            new SqlParameter("@PreferredTerm", preferredTerm),
            new SqlParameter("@Definition", definition ?? preferredTerm)
            };

            dbContext.Database.ExecuteSqlRaw(query, translationParameters);
        }

        private static int? GetActiveStateCodeId(SReportsContext dbContext)
        {
            return dbContext.Codes.Where(x => x.CodeSetId == (int)CodeSetList.EntityState).FirstOrDefault()?.CodeId;
        }

        public static List<string> GetContraceptionList()
        {
            return new List<string> { "Needed", "Not Necessary" };
        }

        public static List<string> GetDiseaseContextList()
        {
            return new List<string> { "Primary", "Secondary" };
        }

        public static List<string> GetInstanceStateList()
        {
            return new List<string> { "Active", "Archived" };
        }

        public static List<string> GetChemotherapySchemaInstanceActionTypeList()
        {
            return new List<string>
            {
                "Delay Dose",
                "Save Dose",
                "Delete Dose",
                "Save Instance",
                "Add Medication",
                "Replace Medication",
                "Delete Medication"
            };
        }

        public static List<string> GetThesaurusStateList()
        {
            return new List<string>()
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
        }

        public static List<string> GetThesaurusMergeStateList()
        {
            return new List<string>()
            {
                "Pending",
                "Completed",
                "Not Completed"
            };
        }

        public static List<string> GetUserStateList()
        {
            return new List<string>()
            {
                "Active",
                "Archived",
                "Blocked"
            };
        }

        public static List<string> GetCommentStateList()
        {
            return new List<string>()
            {
                "Created",
                "Archived",
                "Resolved",
                "Rejected"
            };
        }

        public static List<string> GetGlobalUserSourceList()
        {
            return new List<string>()
            {
                "Microsoft",
                "Google",
                "Internal"
            };
        }

        public static List<string> GetGlobalUserStatusList()
        {
            return new List<string>()
            {
                "Not Verified",
                "Active",
                "Blocked"
            };
        }
    }
}
