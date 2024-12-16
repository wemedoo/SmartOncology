namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;

    public partial class SetPrefixDataFromTempTable : DbMigration
    {
        public override void Up()
        {
            SReportsContext context = new SReportsContext();
            string updatePatientTable = $@"
                update personnel set personnel.PrefixCD = code.CodeId
                    FROM dbo.Codes code
                    inner join dbo.ThesaurusEntryTranslations tranThCode on tranThCode.ThesaurusEntryId = code.ThesaurusEntryId
                    inner join dbo.PersonnelPrefixTempTable prefixTemp on prefixTemp.Prefix = tranThCode.PreferredTerm
                    inner join dbo.Personnel personnel on personnel.UserId = prefixTemp.UserId
                    inner join dbo.codeSets cSP on code.CodeSetId = cSP.CodeSetId
                    inner join dbo.ThesaurusEntryTranslations tranThCodeSetP on tranThCodeSetP.ThesaurusEntryId = cSP.ThesaurusEntryId
                where tranThCodeSetP.PreferredTerm = 'User prefix'
            ";
            string dropPatientTempTable = $@"
                drop table if exists dbo.PersonnelPrefixTempTable;            
            ";

            context.Database.ExecuteSqlCommand(updatePatientTable);
            context.Database.ExecuteSqlCommand(dropPatientTempTable);
        }

        public override void Down()
        {
        }
    }
}
