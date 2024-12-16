namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTasksTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Tasks",
                c => new
                    {
                        TaskId = c.Int(nullable: false, identity: true),
                        PatientId = c.Int(nullable: false),
                        EncounterId = c.Int(nullable: false),
                        TaskDocumentCD = c.Int(),
                        TaskTypeCD = c.Int(nullable: false),
                        TaskStatusCD = c.Int(nullable: false),
                        TaskPriorityCD = c.Int(),
                        TaskClassCD = c.Int(),
                        TaskDescription = c.String(),
                        TaskEntityId = c.String(),
                        TaskStartDateTime = c.DateTime(nullable: false),
                        TaskEndDateTime = c.DateTime(),
                        ScheduledDateTime = c.DateTime(),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        EntryDatetime = c.DateTime(nullable: false),
                        LastUpdate = c.DateTime(),
                        CreatedById = c.Int(),
                        ActiveFrom = c.DateTime(nullable: false),
                        ActiveTo = c.DateTime(nullable: false),
                        EntityStateCD = c.Int(),
                    })
                .PrimaryKey(t => t.TaskId)
                .ForeignKey("dbo.Personnel", t => t.CreatedById)
                .ForeignKey("dbo.Encounters", t => t.EncounterId, cascadeDelete: true)
                .ForeignKey("dbo.Codes", t => t.EntityStateCD)
                .ForeignKey("dbo.Patients", t => t.PatientId, cascadeDelete: false)
                .ForeignKey("dbo.Codes", t => t.TaskClassCD)
                .ForeignKey("dbo.Codes", t => t.TaskDocumentCD)
                .ForeignKey("dbo.Codes", t => t.TaskPriorityCD)
                .ForeignKey("dbo.Codes", t => t.TaskStatusCD, cascadeDelete: true)
                .ForeignKey("dbo.Codes", t => t.TaskTypeCD, cascadeDelete: false)
                .Index(t => t.PatientId)
                .Index(t => t.EncounterId)
                .Index(t => t.TaskDocumentCD)
                .Index(t => t.TaskTypeCD)
                .Index(t => t.TaskStatusCD)
                .Index(t => t.TaskPriorityCD)
                .Index(t => t.TaskClassCD)
                .Index(t => t.CreatedById)
                .Index(t => t.EntityStateCD);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tasks", "TaskTypeCD", "dbo.Codes");
            DropForeignKey("dbo.Tasks", "TaskStatusCD", "dbo.Codes");
            DropForeignKey("dbo.Tasks", "TaskPriorityCD", "dbo.Codes");
            DropForeignKey("dbo.Tasks", "TaskDocumentCD", "dbo.Codes");
            DropForeignKey("dbo.Tasks", "TaskClassCD", "dbo.Codes");
            DropForeignKey("dbo.Tasks", "PatientId", "dbo.Patients");
            DropForeignKey("dbo.Tasks", "EntityStateCD", "dbo.Codes");
            DropForeignKey("dbo.Tasks", "EncounterId", "dbo.Encounters");
            DropForeignKey("dbo.Tasks", "CreatedById", "dbo.Personnel");
            DropIndex("dbo.Tasks", new[] { "EntityStateCD" });
            DropIndex("dbo.Tasks", new[] { "CreatedById" });
            DropIndex("dbo.Tasks", new[] { "TaskClassCD" });
            DropIndex("dbo.Tasks", new[] { "TaskPriorityCD" });
            DropIndex("dbo.Tasks", new[] { "TaskStatusCD" });
            DropIndex("dbo.Tasks", new[] { "TaskTypeCD" });
            DropIndex("dbo.Tasks", new[] { "TaskDocumentCD" });
            DropIndex("dbo.Tasks", new[] { "EncounterId" });
            DropIndex("dbo.Tasks", new[] { "PatientId" });
            DropTable("dbo.Tasks");
        }
    }
}
