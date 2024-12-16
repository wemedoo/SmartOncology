namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTempPatientContactTable : DbMigration
    {
        public override void Up()
        {
            SReportsContext context = new SReportsContext();
            string createTempTable = $@"
                CREATE TABLE dbo.PatientContactTempTable (PatientId int, ContactId int);
            ";
            string saveDataInTempTable = $@"
                INSERT INTO dbo.PatientContactTempTable (PatientId, ContactId) 
                SELECT p.PatientId pId
                      ,cont.ContactId cId
                  FROM dbo.Patients p
                  INNER JOIN dbo.PatientContacts cont
                  on cont.ContactId = p.ContactId;
            ";

            context.Database.ExecuteSqlCommand(createTempTable);
            context.Database.ExecuteSqlCommand(saveDataInTempTable);
        }

        public override void Down()
        {
            SReportsContext context = new SReportsContext();
            string dropTempTable = $@"
                DROP TABLE IF EXISTS dbo.PatientContactTempTable;            
            ";
            context.Database.ExecuteSqlCommand(dropTempTable);
        }
    }
}
