namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPatientContactGenderTempTable : DbMigration
    {
        public override void Up()
        {
            SReportsContext context = new SReportsContext();
            string createTempTable = $@"
                create table dbo.PatientContactGenderTempTable (PatientContactId int, Gender nvarchar(max));
            ";
            string saveDataInTempTable = $@"
                insert into dbo.PatientContactGenderTempTable (PatientContactId, Gender) 
                select pC.PatientContactId
                      ,pC.Gender
                  from dbo.PatientContacts pC
				  where Gender is not null;
            ";

            context.Database.ExecuteSqlCommand(createTempTable);
            context.Database.ExecuteSqlCommand(saveDataInTempTable);
        }

        public override void Down()
        {
            SReportsContext context = new SReportsContext();
            string dropTempTable = $@"
                drop table if exists dbo.PatientContactGenderTempTable;
            ";
            context.Database.ExecuteSqlCommand(dropTempTable);
        }
    }
}
