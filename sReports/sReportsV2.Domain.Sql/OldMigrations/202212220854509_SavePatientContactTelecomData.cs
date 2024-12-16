namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SavePatientContactTelecomData : DbMigration
    {
        public override void Up()
        {
            SReportsContext context = new SReportsContext();
            string insertPatientTelecoms = @"insert into dbo.PatientContactTelecoms (System, Value, [Use], PatientContactId, Active, IsDeleted, EntryDatetime) 
	            select System
                  ,Value
                  ,[Use]
                  ,PatientContactId
                  ,t.Active
                  ,t.IsDeleted
                  ,t.EntryDatetime
                  from dbo.Telecoms t
                  inner join dbo.PatientContacts contact
                  on contact.ContactId = t.PatientContactId;"
            ;

            context.Database.ExecuteSqlCommand(insertPatientTelecoms);
        }

        public override void Down()
        {
        }
    }
}
