namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTaskDocumentTable : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Tasks", name: "TaskDocumentCD", newName: "TaskDocumentId");
            RenameIndex(table: "dbo.Tasks", name: "IX_TaskDocumentCD", newName: "IX_TaskDocumentId");
            CreateTable(
                "dbo.TaskDocuments",
                c => new
                    {
                        TaskDocumentId = c.Int(nullable: false, identity: true),
                        TaskDocumentCD = c.Int(nullable: false),
                        FormId = c.String(),
                        FormTitle = c.String(),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        EntryDatetime = c.DateTime(nullable: false),
                        LastUpdate = c.DateTime(),
                        CreatedById = c.Int(),
                        ActiveFrom = c.DateTime(nullable: false),
                        ActiveTo = c.DateTime(nullable: false),
                        EntityStateCD = c.Int(),
                    })
                .PrimaryKey(t => t.TaskDocumentId)
                .ForeignKey("dbo.Personnel", t => t.CreatedById)
                .ForeignKey("dbo.Codes", t => t.EntityStateCD)
                .ForeignKey("dbo.Codes", t => t.TaskDocumentCD, cascadeDelete: true)
                .Index(t => t.TaskDocumentCD)
                .Index(t => t.CreatedById)
                .Index(t => t.EntityStateCD);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TaskDocuments", "TaskDocumentCD", "dbo.Codes");
            DropForeignKey("dbo.TaskDocuments", "EntityStateCD", "dbo.Codes");
            DropForeignKey("dbo.TaskDocuments", "CreatedById", "dbo.Personnel");
            DropIndex("dbo.TaskDocuments", new[] { "EntityStateCD" });
            DropIndex("dbo.TaskDocuments", new[] { "CreatedById" });
            DropIndex("dbo.TaskDocuments", new[] { "TaskDocumentCD" });
            DropTable("dbo.TaskDocuments");
            RenameIndex(table: "dbo.Tasks", name: "IX_TaskDocumentId", newName: "IX_TaskDocumentCD");
            RenameColumn(table: "dbo.Tasks", name: "TaskDocumentId", newName: "TaskDocumentCD");
        }
    }
}
