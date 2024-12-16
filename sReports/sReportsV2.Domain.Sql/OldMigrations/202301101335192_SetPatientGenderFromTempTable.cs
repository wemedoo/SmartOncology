namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SetPatientGenderFromTempTable : DbMigration
    {
        public override void Up()
        {
            SReportsContext context = new SReportsContext();
            string updatePatientTable = $@"
                update patient set patient.GenderCD = code.CodeId
                FROM dbo.Codes code
                inner join dbo.ThesaurusEntryTranslations tranThCode on tranThCode.ThesaurusEntryId = code.ThesaurusEntryId
                inner join dbo.PatientGenderTempTable genderTemp on genderTemp.Gender = tranThCode.PreferredTerm
                inner join dbo.Patients patient on patient.PatientId = genderTemp.PatientId
                inner join dbo.codeSets cSP on code.CodeSetId = cSP.CodeSetId
                inner join dbo.ThesaurusEntryTranslations tranThCodeSetP on tranThCodeSetP.ThesaurusEntryId = cSP.ThesaurusEntryId
                where tranThCodeSetP.PreferredTerm = 'Gender'
                ;
            ";
            string dropPatientTempTable = $@"
                drop table if exists dbo.PatientGenderTempTable;
            ";
            string updateSmartOncologyPatientTable = $@"
                update smartOncologyPatient set smartOncologyPatient.GenderCD = code.CodeId
                FROM dbo.Codes code
                inner join dbo.ThesaurusEntryTranslations tranThCode on tranThCode.ThesaurusEntryId = code.ThesaurusEntryId
                inner join dbo.SmartOncologyPatientGenderTempTable genderTemp on genderTemp.Gender = tranThCode.PreferredTerm
                inner join dbo.SmartOncologyPatients smartOncologyPatient on smartOncologyPatient.SmartOncologyPatientId = genderTemp.SmartOncologyPatientId
                inner join dbo.codeSets cSSP on code.CodeSetId = cSSP.CodeSetId
                inner join dbo.ThesaurusEntryTranslations tranThCodeSetSP on tranThCodeSetSP.ThesaurusEntryId = cSSP.ThesaurusEntryId
                where tranThCodeSetSP.PreferredTerm = 'Gender'
                ;
            ";
            string dropSmartOncologyPatientTempTable = $@"
                drop table if exists dbo.SmartOncologyPatientGenderTempTable;
            ";

            context.Database.ExecuteSqlCommand(updatePatientTable);
            context.Database.ExecuteSqlCommand(dropPatientTempTable);
            context.Database.ExecuteSqlCommand(updateSmartOncologyPatientTable);
            context.Database.ExecuteSqlCommand(dropSmartOncologyPatientTempTable);
        }
        
        public override void Down()
        {
        }
    }
}
