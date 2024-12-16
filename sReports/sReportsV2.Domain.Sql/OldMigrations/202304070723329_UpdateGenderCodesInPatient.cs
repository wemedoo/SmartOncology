namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateGenderCodesInPatient : DbMigration
    {
        public override void Up()
        {
            string updatePatientMale =
                    @"update [dbo].[Patients]
                      set GenderCD=1744 
                      where GenderCD=1205";

            string updateSmartOncologyPatientsMale =
                    @"update [dbo].[SmartOncologyPatients]
                      set GenderCD=1744 
                      where GenderCD=1205";

            string updatePatientFemale =
                    @"update [dbo].[Patients]
                      set GenderCD=1745 
                      where GenderCD=1206";

            string updateSmartOncologyPatientsFemale =
                    @"update [dbo].[SmartOncologyPatients]
                      set GenderCD=1745 
                      where GenderCD=1206";

            SReportsContext sReportsContext = new SReportsContext();
            sReportsContext.Database.ExecuteSqlCommand(updatePatientMale);
            sReportsContext.Database.ExecuteSqlCommand(updateSmartOncologyPatientsMale);
            sReportsContext.Database.ExecuteSqlCommand(updatePatientFemale);
            sReportsContext.Database.ExecuteSqlCommand(updateSmartOncologyPatientsFemale);
        }
        
        public override void Down()
        {
        }
    }
}
