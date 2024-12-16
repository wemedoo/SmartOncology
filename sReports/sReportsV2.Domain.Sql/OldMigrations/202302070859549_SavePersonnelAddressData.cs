namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;

    public partial class SavePersonnelAddressData : DbMigration
    {
        public override void Up()
        {
            SReportsContext context = new SReportsContext();
            string insertCommand = @"
                insert into dbo.PersonnelAddresses 
                (PersonnelId, Active, AddressTypeCD, City, CountryCD, CreatedById, EntryDatetime, IsDeleted, LastUpdate, PostalCode, [State], Street, StreetNumber)
	            select p.UserId,
	                   adr.Active,
	                   adr.AddressTypeCD,
	                   adr.City,
	                   adr.CountryCD,
	                   adr.CreatedById,
	                   adr.EntryDatetime,
	                   adr.IsDeleted,
	                   adr.LastUpdate,
	                   adr.PostalCode,
	                   adr.[State],
	                   adr.Street,
	                   adr.StreetNumber
                  from dbo.Addresses adr
                  inner join dbo.Personnel p on adr.AddressId = p.AddressId;
            ";
            context.Database.ExecuteSqlCommand(insertCommand);
        }

        public override void Down()
        {
        }
    }
}
