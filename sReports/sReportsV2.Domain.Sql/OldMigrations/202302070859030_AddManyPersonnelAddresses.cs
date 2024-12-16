namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddManyPersonnelAddresses : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PersonnelAddresses",
                c => new
                {
                    PersonnelAddressId = c.Int(nullable: false, identity: true),
                    PersonnelId = c.Int(nullable: false),
                    City = c.String(maxLength: 100),
                    State = c.String(maxLength: 50),
                    PostalCode = c.String(maxLength: 10),
                    CountryCD = c.Int(),
                    Street = c.String(maxLength: 200),
                    StreetNumber = c.Int(),
                    AddressTypeCD = c.Int(),
                    Active = c.Boolean(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    EntryDatetime = c.DateTime(nullable: false),
                    LastUpdate = c.DateTime(),
                    CreatedById = c.Int(),
                })
                .PrimaryKey(t => t.PersonnelAddressId)
                .ForeignKey("dbo.Codes", t => t.AddressTypeCD)
                .ForeignKey("dbo.Codes", t => t.CountryCD)
                .ForeignKey("dbo.Personnel", t => t.CreatedById)
                .ForeignKey("dbo.Personnel", t => t.PersonnelId, cascadeDelete: true)
                .Index(t => t.PersonnelId)
                .Index(t => t.CountryCD)
                .Index(t => t.AddressTypeCD)
                .Index(t => t.CreatedById);

        }

        public override void Down()
        {
            DropForeignKey("dbo.PersonnelAddresses", "PersonnelId", "dbo.Personnel");
            DropForeignKey("dbo.PersonnelAddresses", "CreatedById", "dbo.Personnel");
            DropForeignKey("dbo.PersonnelAddresses", "CountryCD", "dbo.Codes");
            DropForeignKey("dbo.PersonnelAddresses", "AddressTypeCD", "dbo.Codes");
            DropIndex("dbo.PersonnelAddresses", new[] { "CreatedById" });
            DropIndex("dbo.PersonnelAddresses", new[] { "AddressTypeCD" });
            DropIndex("dbo.PersonnelAddresses", new[] { "CountryCD" });
            DropIndex("dbo.PersonnelAddresses", new[] { "PersonnelId" });
            DropTable("dbo.PersonnelAddresses");
        }
    }
}
