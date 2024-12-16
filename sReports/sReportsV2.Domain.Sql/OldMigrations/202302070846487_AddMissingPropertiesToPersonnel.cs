namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddMissingPropertiesToPersonnel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Personnel", "SystemName", c => c.String(maxLength: 128));
            AddColumn("dbo.Personnel", "PersonnelTypeCD", c => c.Int());
            DropColumn("dbo.Personnel", "PrefixCD");
            AddColumn("dbo.Personnel", "PrefixCD", c => c.Int());
            CreateIndex("dbo.Personnel", "PrefixCD");
            CreateIndex("dbo.Personnel", "PersonnelTypeCD");
            AddForeignKey("dbo.Personnel", "PersonnelTypeCD", "dbo.Codes", "CodeId");
            AddForeignKey("dbo.Personnel", "PrefixCD", "dbo.Codes", "CodeId");
        }

        public override void Down()
        {
            DropForeignKey("dbo.Personnel", "PrefixCD", "dbo.Codes");
            DropForeignKey("dbo.Personnel", "PersonnelTypeCD", "dbo.Codes");
            DropIndex("dbo.Personnel", new[] { "PersonnelTypeCD" });
            DropIndex("dbo.Personnel", new[] { "PrefixCD" });
            DropColumn("dbo.Personnel", "PrefixCD");
            AddColumn("dbo.Personnel", "PrefixCD", c => c.Int(nullable: false));
            DropColumn("dbo.Personnel", "PersonnelTypeCD");
            DropColumn("dbo.Personnel", "SystemName");
        }
    }
}
