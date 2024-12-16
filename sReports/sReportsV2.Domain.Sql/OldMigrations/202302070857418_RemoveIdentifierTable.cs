namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class RemoveIdentifierTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Identifiers", "CreatedById", "dbo.Personnel");
            DropForeignKey("dbo.Identifiers", "IdentifierPoolCD", "dbo.Codes");
            DropForeignKey("dbo.Identifiers", "IdentifierTypeCD", "dbo.Codes");
            DropForeignKey("dbo.Identifiers", "IdentifierUseCD", "dbo.Codes");
            DropForeignKey("dbo.Identifiers", "OrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.Identifiers", "SmartOncologyPatientId", "dbo.SmartOncologyPatients");
            DropForeignKey("dbo.Identifiers", "PatientId", "dbo.Patients");
            DropIndex("dbo.Identifiers", new[] { "OrganizationId" });
            DropIndex("dbo.Identifiers", new[] { "PatientId" });
            DropIndex("dbo.Identifiers", new[] { "SmartOncologyPatientId" });
            DropIndex("dbo.Identifiers", new[] { "IdentifierTypeCD" });
            DropIndex("dbo.Identifiers", new[] { "IdentifierPoolCD" });
            DropIndex("dbo.Identifiers", new[] { "IdentifierUseCD" });
            DropIndex("dbo.Identifiers", new[] { "CreatedById" });
            DropTable("dbo.Identifiers");
        }

        public override void Down()
        {
            CreateTable(
                "dbo.Identifiers",
                c => new
                {
                    IdentifierId = c.Int(nullable: false, identity: true),
                    OrganizationId = c.Int(),
                    PatientId = c.Int(),
                    SmartOncologyPatientId = c.Int(),
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
                .PrimaryKey(t => t.IdentifierId);

            CreateIndex("dbo.Identifiers", "CreatedById");
            CreateIndex("dbo.Identifiers", "IdentifierUseCD");
            CreateIndex("dbo.Identifiers", "IdentifierPoolCD");
            CreateIndex("dbo.Identifiers", "IdentifierTypeCD");
            CreateIndex("dbo.Identifiers", "SmartOncologyPatientId");
            CreateIndex("dbo.Identifiers", "PatientId");
            CreateIndex("dbo.Identifiers", "OrganizationId");
            AddForeignKey("dbo.Identifiers", "PatientId", "dbo.Patients", "PatientId");
            AddForeignKey("dbo.Identifiers", "SmartOncologyPatientId", "dbo.SmartOncologyPatients", "SmartOncologyPatientId");
            AddForeignKey("dbo.Identifiers", "OrganizationId", "dbo.Organizations", "OrganizationId");
            AddForeignKey("dbo.Identifiers", "IdentifierUseCD", "dbo.Codes", "CodeId");
            AddForeignKey("dbo.Identifiers", "IdentifierTypeCD", "dbo.Codes", "CodeId");
            AddForeignKey("dbo.Identifiers", "IdentifierPoolCD", "dbo.Codes", "CodeId");
            AddForeignKey("dbo.Identifiers", "CreatedById", "dbo.Personnel", "UserId");
        }
    }
}
