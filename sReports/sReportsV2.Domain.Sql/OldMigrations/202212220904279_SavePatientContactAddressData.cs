namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SavePatientContactAddressData : DbMigration
    {
        public override void Up()
        {
            SReportsContext context = new SReportsContext();
            string insertCommand = @"
                insert into dbo.PatientContactAddresses (PatientContactId, City, State, PostalCode, CountryId, Street, StreetNumber, IsDeleted, EntryDatetime, Active)
               select pc.ContactId, a.City, a.State, a.PostalCode, a.CountryId, a.Street, a.StreetNumber, pc.IsDeleted, pc.EntryDatetime, pc.Active 
               from dbo.PatientContacts pc inner join dbo.Addresses a on pc.AddressId = a.AddressId;
            ";
            context.Database.ExecuteSqlCommand(insertCommand);
        }

        public override void Down()
        {
        }
    }
}
