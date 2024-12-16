namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEncounterIdentifier : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EncounterIdentifiers",
                c => new
                    {
                        EncounterIdentifierId = c.Int(nullable: false, identity: true),
                        EncounterId = c.Int(),
                        IdentifierValue = c.String(maxLength: 128),
                        IdentifierTypeCD = c.Int(),
                        IdentifierPoolCD = c.Int(),
                        IdentifierUseCD = c.Int(),
                        Active = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        EntryDatetime = c.DateTime(nullable: false),
                        LastUpdate = c.DateTime(),
                        CreatedById = c.Int(),
                    })
                .PrimaryKey(t => t.EncounterIdentifierId)
                .ForeignKey("dbo.Personnel", t => t.CreatedById)
                .ForeignKey("dbo.Encounters", t => t.EncounterId)
                .ForeignKey("dbo.Codes", t => t.IdentifierPoolCD)
                .ForeignKey("dbo.Codes", t => t.IdentifierTypeCD)
                .ForeignKey("dbo.Codes", t => t.IdentifierUseCD)
                .Index(t => t.EncounterId)
                .Index(t => t.IdentifierTypeCD)
                .Index(t => t.IdentifierPoolCD)
                .Index(t => t.IdentifierUseCD)
                .Index(t => t.CreatedById);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EncounterIdentifiers", "IdentifierUseCD", "dbo.Codes");
            DropForeignKey("dbo.EncounterIdentifiers", "IdentifierTypeCD", "dbo.Codes");
            DropForeignKey("dbo.EncounterIdentifiers", "IdentifierPoolCD", "dbo.Codes");
            DropForeignKey("dbo.EncounterIdentifiers", "EncounterId", "dbo.Encounters");
            DropForeignKey("dbo.EncounterIdentifiers", "CreatedById", "dbo.Personnel");
            DropIndex("dbo.EncounterIdentifiers", new[] { "CreatedById" });
            DropIndex("dbo.EncounterIdentifiers", new[] { "IdentifierUseCD" });
            DropIndex("dbo.EncounterIdentifiers", new[] { "IdentifierPoolCD" });
            DropIndex("dbo.EncounterIdentifiers", new[] { "IdentifierTypeCD" });
            DropIndex("dbo.EncounterIdentifiers", new[] { "EncounterId" });
            DropTable("dbo.EncounterIdentifiers");
        }
    }
}
