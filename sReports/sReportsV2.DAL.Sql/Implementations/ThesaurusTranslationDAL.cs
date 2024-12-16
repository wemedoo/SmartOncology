using Microsoft.Extensions.Configuration;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using sReportsV2.SqlDomain.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace sReportsV2.SqlDomain.Implementations
{
    public class ThesaurusTranslationDAL : IThesaurusTranslationDAL
    {
        public void InsertMany(List<ThesaurusEntry> thesauruses, List<int> bulkedThesauruses, IConfiguration configuration)
        {
            DataTable translationTable = new DataTable();
            translationTable.Columns.Add(new DataColumn("Language", typeof(string)));
            translationTable.Columns.Add(new DataColumn("Definition", typeof(string)));
            translationTable.Columns.Add(new DataColumn("PreferredTerm", typeof(string)));
            translationTable.Columns.Add(new DataColumn("ThesaurusEntryId", typeof(int)));
            translationTable.Columns.Add(new DataColumn("SynonymsString", typeof(string)));
            translationTable.Columns.Add(new DataColumn("AbbreviationsString", typeof(string)));

            int i = 0;
            foreach (var thesaurus in thesauruses)
            {
                foreach (var translation in thesaurus.Translations)
                {
                    DataRow translationRow = translationTable.NewRow();
                    translationRow["Language"] = translation.Language;
                    translationRow["Definition"] = translation.Definition;
                    translationRow["PreferredTerm"] = translation.PreferredTerm;
                    translationRow["ThesaurusEntryId"] = bulkedThesauruses[i];
                    translationRow["SynonymsString"] = translation.SynonymsString;
                    translationRow["AbbreviationsString"] = translation.AbbreviationsString;

                    translationTable.Rows.Add(translationRow);
                }

                i++;
            }

            string connection = configuration["Sql"];
            SqlConnection con = new SqlConnection(connection);
            SqlBulkCopy objbulk = new SqlBulkCopy(con);
            objbulk.BulkCopyTimeout = 0;

            objbulk.DestinationTableName = "ThesaurusEntryTranslations";
            objbulk.ColumnMappings.Add("Language", "Language");
            objbulk.ColumnMappings.Add("Definition", "Definition");
            objbulk.ColumnMappings.Add("PreferredTerm", "PreferredTerm");
            objbulk.ColumnMappings.Add("ThesaurusEntryId", "ThesaurusEntryId");
            objbulk.ColumnMappings.Add("SynonymsString", "SynonymsString");
            objbulk.ColumnMappings.Add("AbbreviationsString", "AbbreviationsString");

            con.Open();
            objbulk.WriteToServer(translationTable);
            con.Close();
        }
    }
}
