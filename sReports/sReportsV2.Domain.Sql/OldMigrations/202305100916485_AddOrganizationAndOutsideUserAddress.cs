namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOrganizationAndOutsideUserAddress : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OrganizationAddresses",
                c => new
                    {
                        OrganizationAddressId = c.Int(nullable: false, identity: true),
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
                .PrimaryKey(t => t.OrganizationAddressId)
                .ForeignKey("dbo.Codes", t => t.AddressTypeCD)
                .ForeignKey("dbo.Codes", t => t.CountryCD)
                .ForeignKey("dbo.Personnel", t => t.CreatedById)
                .ForeignKey("dbo.Codes", t => t.EntityStateCD)
                .Index(t => t.CountryCD)
                .Index(t => t.AddressTypeCD)
                .Index(t => t.CreatedById)
                .Index(t => t.EntityStateCD);
            
            CreateTable(
                "dbo.OutsideUserAddresses",
                c => new
                    {
                        OutsideUserAddressId = c.Int(nullable: false, identity: true),
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
                .PrimaryKey(t => t.OutsideUserAddressId)
                .ForeignKey("dbo.Codes", t => t.AddressTypeCD)
                .ForeignKey("dbo.Codes", t => t.CountryCD)
                .ForeignKey("dbo.Personnel", t => t.CreatedById)
                .ForeignKey("dbo.Codes", t => t.EntityStateCD)
                .Index(t => t.CountryCD)
                .Index(t => t.AddressTypeCD)
                .Index(t => t.CreatedById)
                .Index(t => t.EntityStateCD);
            
            AddColumn("dbo.Organizations", "OrganizationAddressId", c => c.Int());
            AddColumn("dbo.OutsideUsers", "OutsideUserAddressId", c => c.Int());
            CreateIndex("dbo.Organizations", "OrganizationAddressId");
            CreateIndex("dbo.OutsideUsers", "OutsideUserAddressId");
            AddForeignKey("dbo.Organizations", "OrganizationAddressId", "dbo.OrganizationAddresses", "OrganizationAddressId");
            AddForeignKey("dbo.OutsideUsers", "OutsideUserAddressId", "dbo.OutsideUserAddresses", "OutsideUserAddressId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OutsideUsers", "OutsideUserAddressId", "dbo.OutsideUserAddresses");
            DropForeignKey("dbo.OutsideUserAddresses", "EntityStateCD", "dbo.Codes");
            DropForeignKey("dbo.OutsideUserAddresses", "CreatedById", "dbo.Personnel");
            DropForeignKey("dbo.OutsideUserAddresses", "CountryCD", "dbo.Codes");
            DropForeignKey("dbo.OutsideUserAddresses", "AddressTypeCD", "dbo.Codes");
            DropForeignKey("dbo.Organizations", "OrganizationAddressId", "dbo.OrganizationAddresses");
            DropForeignKey("dbo.OrganizationAddresses", "EntityStateCD", "dbo.Codes");
            DropForeignKey("dbo.OrganizationAddresses", "CreatedById", "dbo.Personnel");
            DropForeignKey("dbo.OrganizationAddresses", "CountryCD", "dbo.Codes");
            DropForeignKey("dbo.OrganizationAddresses", "AddressTypeCD", "dbo.Codes");
            DropIndex("dbo.OutsideUserAddresses", new[] { "EntityStateCD" });
            DropIndex("dbo.OutsideUserAddresses", new[] { "CreatedById" });
            DropIndex("dbo.OutsideUserAddresses", new[] { "AddressTypeCD" });
            DropIndex("dbo.OutsideUserAddresses", new[] { "CountryCD" });
            DropIndex("dbo.OutsideUsers", new[] { "OutsideUserAddressId" });
            DropIndex("dbo.OrganizationAddresses", new[] { "EntityStateCD" });
            DropIndex("dbo.OrganizationAddresses", new[] { "CreatedById" });
            DropIndex("dbo.OrganizationAddresses", new[] { "AddressTypeCD" });
            DropIndex("dbo.OrganizationAddresses", new[] { "CountryCD" });
            DropIndex("dbo.Organizations", new[] { "OrganizationAddressId" });
            DropColumn("dbo.OutsideUsers", "OutsideUserAddressId");
            DropColumn("dbo.Organizations", "OrganizationAddressId");
            DropTable("dbo.OutsideUserAddresses");
            DropTable("dbo.OrganizationAddresses");
        }
    }
}
