namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SetPatientContactGenderFromTempTable : DbMigration
    {
        public override void Up()
        {
            SReportsContext context = new SReportsContext();
            string updateAddressTable = $@"
                update patientContact set patientContact.GenderCD = code.CodeId
                FROM dbo.Codes code
                inner join dbo.ThesaurusEntryTranslations tranThCode on tranThCode.ThesaurusEntryId = code.ThesaurusEntryId
                inner join dbo.PatientContactGenderTempTable contGenderTemp on contGenderTemp.Gender = tranThCode.PreferredTerm
                inner join dbo.PatientContacts patientContact on patientContact.PatientContactId = contGenderTemp.PatientContactId
                inner join dbo.codeSets cS on code.CodeSetId = cS.CodeSetId
                inner join dbo.ThesaurusEntryTranslations tranThCodeSet on tranThCodeSet.ThesaurusEntryId = cS.ThesaurusEntryId
                where tranThCodeSet.PreferredTerm = 'Gender'
                ;
            ";
            string dropTempTable = $@"
                drop table if exists dbo.PatientContactGenderTempTable;
            ";

            context.Database.ExecuteSqlCommand(updateAddressTable);
            context.Database.ExecuteSqlCommand(dropTempTable);
        }
        
        public override void Down()
        {
        }
    }
}
