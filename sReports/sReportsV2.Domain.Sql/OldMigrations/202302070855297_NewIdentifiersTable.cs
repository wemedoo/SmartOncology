namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class NewIdentifiersTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PersonnelIdentifiers",
                c => new
                {
                    PersonnelIdentifierId = c.Int(nullable: false, identity: true),
                    PersonnelId = c.Int(),
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
                .PrimaryKey(t => t.PersonnelIdentifierId)
                .ForeignKey("dbo.Personnel", t => t.CreatedById)
                .ForeignKey("dbo.Codes", t => t.IdentifierPoolCD)
                .ForeignKey("dbo.Codes", t => t.IdentifierTypeCD)
                .ForeignKey("dbo.Codes", t => t.IdentifierUseCD)
                .ForeignKey("dbo.Personnel", t => t.PersonnelId)
                .Index(t => t.PersonnelId)
                .Index(t => t.IdentifierTypeCD)
                .Index(t => t.IdentifierPoolCD)
                .Index(t => t.IdentifierUseCD)
                .Index(t => t.CreatedById);

            CreateTable(
                "dbo.OrganizationIdentifiers",
                c => new
                {
                    OrganizationIdentifierId = c.Int(nullable: false, identity: true),
                    OrganizationId = c.Int(),
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
                .PrimaryKey(t => t.OrganizationIdentifierId)
                .ForeignKey("dbo.Personnel", t => t.CreatedById)
                .ForeignKey("dbo.Codes", t => t.IdentifierPoolCD)
                .ForeignKey("dbo.Codes", t => t.IdentifierTypeCD)
                .ForeignKey("dbo.Codes", t => t.IdentifierUseCD)
                .ForeignKey("dbo.Organizations", t => t.OrganizationId)
                .Index(t => t.OrganizationId)
                .Index(t => t.IdentifierTypeCD)
                .Index(t => t.IdentifierPoolCD)
                .Index(t => t.IdentifierUseCD)
                .Index(t => t.CreatedById);

            CreateTable(
                "dbo.PatientIdentifiers",
                c => new
                {
                    PatientIdentifierId = c.Int(nullable: false, identity: true),
                    PatientId = c.Int(),
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
                .PrimaryKey(t => t.PatientIdentifierId)
                .ForeignKey("dbo.Personnel", t => t.CreatedById)
                .ForeignKey("dbo.Codes", t => t.IdentifierPoolCD)
                .ForeignKey("dbo.Codes", t => t.IdentifierTypeCD)
                .ForeignKey("dbo.Codes", t => t.IdentifierUseCD)
                .ForeignKey("dbo.Patients", t => t.PatientId)
                .Index(t => t.PatientId)
                .Index(t => t.IdentifierTypeCD)
                .Index(t => t.IdentifierPoolCD)
                .Index(t => t.IdentifierUseCD)
                .Index(t => t.CreatedById);

        }

        public override void Down()
        {
            DropForeignKey("dbo.PatientIdentifiers", "PatientId", "dbo.Patients");
            DropForeignKey("dbo.PatientIdentifiers", "IdentifierUseCD", "dbo.Codes");
            DropForeignKey("dbo.PatientIdentifiers", "IdentifierTypeCD", "dbo.Codes");
            DropForeignKey("dbo.PatientIdentifiers", "IdentifierPoolCD", "dbo.Codes");
            DropForeignKey("dbo.PatientIdentifiers", "CreatedById", "dbo.Personnel");
            DropForeignKey("dbo.OrganizationIdentifiers", "OrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.OrganizationIdentifiers", "IdentifierUseCD", "dbo.Codes");
            DropForeignKey("dbo.OrganizationIdentifiers", "IdentifierTypeCD", "dbo.Codes");
            DropForeignKey("dbo.OrganizationIdentifiers", "IdentifierPoolCD", "dbo.Codes");
            DropForeignKey("dbo.OrganizationIdentifiers", "CreatedById", "dbo.Personnel");
            DropForeignKey("dbo.PersonnelIdentifiers", "PersonnelId", "dbo.Personnel");
            DropForeignKey("dbo.PersonnelIdentifiers", "IdentifierUseCD", "dbo.Codes");
            DropForeignKey("dbo.PersonnelIdentifiers", "IdentifierTypeCD", "dbo.Codes");
            DropForeignKey("dbo.PersonnelIdentifiers", "IdentifierPoolCD", "dbo.Codes");
            DropForeignKey("dbo.PersonnelIdentifiers", "CreatedById", "dbo.Personnel");
            DropIndex("dbo.PatientIdentifiers", new[] { "CreatedById" });
            DropIndex("dbo.PatientIdentifiers", new[] { "IdentifierUseCD" });
            DropIndex("dbo.PatientIdentifiers", new[] { "IdentifierPoolCD" });
            DropIndex("dbo.PatientIdentifiers", new[] { "IdentifierTypeCD" });
            DropIndex("dbo.PatientIdentifiers", new[] { "PatientId" });
            DropIndex("dbo.OrganizationIdentifiers", new[] { "CreatedById" });
            DropIndex("dbo.OrganizationIdentifiers", new[] { "IdentifierUseCD" });
            DropIndex("dbo.OrganizationIdentifiers", new[] { "IdentifierPoolCD" });
            DropIndex("dbo.OrganizationIdentifiers", new[] { "IdentifierTypeCD" });
            DropIndex("dbo.OrganizationIdentifiers", new[] { "OrganizationId" });
            DropIndex("dbo.PersonnelIdentifiers", new[] { "CreatedById" });
            DropIndex("dbo.PersonnelIdentifiers", new[] { "IdentifierUseCD" });
            DropIndex("dbo.PersonnelIdentifiers", new[] { "IdentifierPoolCD" });
            DropIndex("dbo.PersonnelIdentifiers", new[] { "IdentifierTypeCD" });
            DropIndex("dbo.PersonnelIdentifiers", new[] { "PersonnelId" });
            DropTable("dbo.PatientIdentifiers");
            DropTable("dbo.OrganizationIdentifiers");
            DropTable("dbo.PersonnelIdentifiers");
        }
    }
}
