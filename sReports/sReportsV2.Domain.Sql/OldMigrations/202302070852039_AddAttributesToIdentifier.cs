namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddAttributesToIdentifier : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Identifiers", "IdentifierValue", c => c.String(maxLength: 128));
            AddColumn("dbo.Identifiers", "IdentifierTypeCD", c => c.Int());
            AddColumn("dbo.Identifiers", "IdentifierPoolCD", c => c.Int());
            AddColumn("dbo.Identifiers", "IdentifierUseCD", c => c.Int());
            AddColumn("dbo.Identifiers", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.Identifiers", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Identifiers", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            AddColumn("dbo.Identifiers", "EntryDatetime", c => c.DateTime(nullable: false, defaultValueSql: "getdate()"));
            AddColumn("dbo.Identifiers", "LastUpdate", c => c.DateTime());
            AddColumn("dbo.Identifiers", "CreatedById", c => c.Int());
            CreateIndex("dbo.Identifiers", "IdentifierTypeCD");
            CreateIndex("dbo.Identifiers", "IdentifierPoolCD");
            CreateIndex("dbo.Identifiers", "IdentifierUseCD");
            CreateIndex("dbo.Identifiers", "CreatedById");
            AddForeignKey("dbo.Identifiers", "CreatedById", "dbo.Personnel", "UserId");
            AddForeignKey("dbo.Identifiers", "IdentifierPoolCD", "dbo.Codes", "CodeId");
            AddForeignKey("dbo.Identifiers", "IdentifierTypeCD", "dbo.Codes", "CodeId");
            AddForeignKey("dbo.Identifiers", "IdentifierUseCD", "dbo.Codes", "CodeId");
        }

        public override void Down()
        {
            DropForeignKey("dbo.Identifiers", "IdentifierUseCD", "dbo.Codes");
            DropForeignKey("dbo.Identifiers", "IdentifierTypeCD", "dbo.Codes");
            DropForeignKey("dbo.Identifiers", "IdentifierPoolCD", "dbo.Codes");
            DropForeignKey("dbo.Identifiers", "CreatedById", "dbo.Personnel");
            DropIndex("dbo.Identifiers", new[] { "CreatedById" });
            DropIndex("dbo.Identifiers", new[] { "IdentifierUseCD" });
            DropIndex("dbo.Identifiers", new[] { "IdentifierPoolCD" });
            DropIndex("dbo.Identifiers", new[] { "IdentifierTypeCD" });
            DropColumn("dbo.Identifiers", "CreatedById");
            DropColumn("dbo.Identifiers", "LastUpdate");
            DropColumn("dbo.Identifiers", "EntryDatetime");
            DropColumn("dbo.Identifiers", "RowVersion");
            DropColumn("dbo.Identifiers", "IsDeleted");
            DropColumn("dbo.Identifiers", "Active");
            DropColumn("dbo.Identifiers", "IdentifierUseCD");
            DropColumn("dbo.Identifiers", "IdentifierPoolCD");
            DropColumn("dbo.Identifiers", "IdentifierTypeCD");
            DropColumn("dbo.Identifiers", "IdentifierValue");
        }
    }
}
