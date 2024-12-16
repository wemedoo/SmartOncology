namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReplacePlusWithBlankInPatientContactEntries : DbMigration
    {
        public override void Up()
        {
            Sql($@"
                update [dbo].[PatientContactTelecoms] set Value = REPLACE(Value, '+', ' ')
                    where Value not like '+%' and Value like '%+%';

                update [dbo].[PatientContactAddresses] set City = REPLACE(City, '+', ' ')
                    where City not like '+%' and City like '%+%';

                update [dbo].[PatientContactAddresses] set Street = REPLACE(Street, '+', ' ')
                    where Street not like '+%' and Street like '%+%';

                update [dbo].[PatientContacts]
                  set NameGiven = REPLACE(NameGiven, '+', '')
                  where CHARINDEX('+', NameGiven) > 0;"
            );
        }
        
        public override void Down()
        {
        }
    }
}
