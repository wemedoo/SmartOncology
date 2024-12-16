namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddHL7MessageLog : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.HL7MessageLog",
                c => new
                    {
                        HL7MessageLogId = c.Int(nullable: false, identity: true),
                        MessageControlId = c.String(),
                        Message = c.String(),
                        Active = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        EntryDatetime = c.DateTime(nullable: false),
                        LastUpdate = c.DateTime(),
                        CreatedById = c.Int(),
                    })
                .PrimaryKey(t => t.HL7MessageLogId)
                .ForeignKey("dbo.Personnel", t => t.CreatedById)
                .Index(t => t.CreatedById);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.HL7MessageLog", "CreatedById", "dbo.Personnel");
            DropIndex("dbo.HL7MessageLog", new[] { "CreatedById" });
            DropTable("dbo.HL7MessageLog");
        }
    }
}
