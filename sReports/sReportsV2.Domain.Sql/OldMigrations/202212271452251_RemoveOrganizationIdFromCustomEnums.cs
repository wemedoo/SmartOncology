namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveOrganizationIdFromCustomEnums : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CustomEnums", "OrganizationId", "dbo.Organizations");
            DropIndex("dbo.CustomEnums", new[] { "OrganizationId" });
            DropColumn("dbo.CustomEnums", "OrganizationId");
        }

        public override void Down()
        {
            AddColumn("dbo.CustomEnums", "OrganizationId", c => c.Int(nullable: false));
            CreateIndex("dbo.CustomEnums", "OrganizationId");
            AddForeignKey("dbo.CustomEnums", "OrganizationId", "dbo.Organizations", "Id", cascadeDelete: true);
        }
    }
}
