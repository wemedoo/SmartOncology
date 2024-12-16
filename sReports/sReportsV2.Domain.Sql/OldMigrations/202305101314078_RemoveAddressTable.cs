namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveAddressTable : DbMigration
    {
        public override void Up()
        {
            string removeOnlyOnePersonnelAddress = @"
                alter table dbo.Organizations drop constraint if exists [FK_dbo.Organizations_dbo.Addresses_AddressId];
                drop index if exists dbo.Organizations.IX_AddressId;
                alter table dbo.Organizations drop column if exists AddressId;

                alter table dbo.OutsideUsers drop constraint if exists [FK_dbo.OutsideUsers_dbo.Addresses_Address_Id];
                drop index if exists dbo.OutsideUsers.IX_Address_Id;
                alter table dbo.OutsideUsers drop column if exists AddressId;
                drop table if exists dbo.Addresses;
                "
            ;
            Sql(removeOnlyOnePersonnelAddress);
        }

        public override void Down()
        {
            CreateTable(
                "dbo.Addresses",
                c => new
                {
                    AddressId = c.Int(nullable: false, identity: true),
                    City = c.String(maxLength: 100),
                    State = c.String(maxLength: 50),
                    PostalCode = c.String(maxLength: 10),
                    CountryCD = c.Int(),
                    Street = c.String(maxLength: 200),
                    StreetNumber = c.Int(),
                    AddressTypeCD = c.Int(),
                    RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    EntryDatetime = c.DateTime(nullable: false),
                    LastUpdate = c.DateTime(),
                    CreatedById = c.Int(),
                    ActiveFrom = c.DateTime(nullable: false),
                    ActiveTo = c.DateTime(nullable: false),
                    EntityStateCD = c.Int(),
                })
                .PrimaryKey(t => t.AddressId);

            AddColumn("dbo.OutsideUsers", "AddressId", c => c.Int());
            AddColumn("dbo.Organizations", "AddressId", c => c.Int());
            CreateIndex("dbo.OutsideUsers", "AddressId");
            CreateIndex("dbo.Organizations", "AddressId");
            CreateIndex("dbo.Addresses", "EntityStateCD");
            CreateIndex("dbo.Addresses", "CreatedById");
            CreateIndex("dbo.Addresses", "AddressTypeCD");
            CreateIndex("dbo.Addresses", "CountryCD");
            AddForeignKey("dbo.OutsideUsers", "AddressId", "dbo.Addresses", "AddressId");
            AddForeignKey("dbo.Addresses", "EntityStateCD", "dbo.Codes", "CodeId");
            AddForeignKey("dbo.Addresses", "CreatedById", "dbo.Personnel", "UserId");
            AddForeignKey("dbo.Addresses", "CountryCD", "dbo.Codes", "CodeId");
            AddForeignKey("dbo.Addresses", "AddressTypeCD", "dbo.Codes", "CodeId");
            AddForeignKey("dbo.Organizations", "AddressId", "dbo.Addresses", "AddressId");
        }
    }
}
