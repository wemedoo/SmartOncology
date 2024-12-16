namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SetPatientContactsFromTempTable : DbMigration
    {
        public override void Up()
        {
            SReportsContext context = new SReportsContext();
            string updateAddressTable = $@"
                update cont set cont.PatientId = contTemp.PatientId
                FROM dbo.PatientContactTempTable contTemp
                inner join dbo.PatientContacts cont on cont.ContactId = contTemp.ContactId
            ;";
            string dropTempTable = $@"
                drop table dbo.PatientContactTempTable;
            ";

            context.Database.ExecuteSqlCommand(updateAddressTable);
            context.Database.ExecuteSqlCommand(dropTempTable);
        }

        public override void Down()
        {
        }
    }
}
