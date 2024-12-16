namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOrganizationCommunicationEntity : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OrganizationCommunicationEntities",
                c => new
                    {
                        OrgCommunicationEntityId = c.Int(nullable: false, identity: true),
                        DisplayName = c.String(),
                        OrgCommunicationEntityCD = c.Int(),
                        PrimaryCommunicationSystemCD = c.Int(),
                        SecondaryCommunicationSystemCD = c.Int(),
                        OrganizationId = c.Int(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        EntryDatetime = c.DateTime(nullable: false),
                        LastUpdate = c.DateTime(),
                        CreatedById = c.Int(),
                        ActiveFrom = c.DateTime(nullable: false),
                        ActiveTo = c.DateTime(nullable: false),
                        EntityStateCD = c.Int(),
                    })
                .PrimaryKey(t => t.OrgCommunicationEntityId)
                .ForeignKey("dbo.Personnel", t => t.CreatedById)
                .ForeignKey("dbo.Codes", t => t.EntityStateCD)
                .ForeignKey("dbo.Organizations", t => t.OrganizationId, cascadeDelete: true)
                .ForeignKey("dbo.Codes", t => t.OrgCommunicationEntityCD)
                .ForeignKey("dbo.Codes", t => t.PrimaryCommunicationSystemCD)
                .ForeignKey("dbo.Codes", t => t.SecondaryCommunicationSystemCD)
                .Index(t => t.OrgCommunicationEntityCD)
                .Index(t => t.PrimaryCommunicationSystemCD)
                .Index(t => t.SecondaryCommunicationSystemCD)
                .Index(t => t.OrganizationId)
                .Index(t => t.CreatedById)
                .Index(t => t.EntityStateCD);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrganizationCommunicationEntities", "SecondaryCommunicationSystemCD", "dbo.Codes");
            DropForeignKey("dbo.OrganizationCommunicationEntities", "PrimaryCommunicationSystemCD", "dbo.Codes");
            DropForeignKey("dbo.OrganizationCommunicationEntities", "OrgCommunicationEntityCD", "dbo.Codes");
            DropForeignKey("dbo.OrganizationCommunicationEntities", "OrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.OrganizationCommunicationEntities", "EntityStateCD", "dbo.Codes");
            DropForeignKey("dbo.OrganizationCommunicationEntities", "CreatedById", "dbo.Personnel");
            DropIndex("dbo.OrganizationCommunicationEntities", new[] { "EntityStateCD" });
            DropIndex("dbo.OrganizationCommunicationEntities", new[] { "CreatedById" });
            DropIndex("dbo.OrganizationCommunicationEntities", new[] { "OrganizationId" });
            DropIndex("dbo.OrganizationCommunicationEntities", new[] { "SecondaryCommunicationSystemCD" });
            DropIndex("dbo.OrganizationCommunicationEntities", new[] { "PrimaryCommunicationSystemCD" });
            DropIndex("dbo.OrganizationCommunicationEntities", new[] { "OrgCommunicationEntityCD" });
            DropTable("dbo.OrganizationCommunicationEntities");
        }
    }
}
