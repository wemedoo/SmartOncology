namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAdditionalHL7LogTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ErrorMessageLogs",
                c => new
                    {
                        ErrorMessageLogId = c.Int(nullable: false, identity: true),
                        HL7MessageLogId = c.Int(nullable: false),
                        ErrorTypeCD = c.Int(),
                        ErrorText = c.String(),
                        HL7EventType = c.String(),
                        SourceSystemCD = c.Int(),
                        TransactionDatetime = c.DateTime(),
                        Active = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        EntryDatetime = c.DateTime(nullable: false),
                        LastUpdate = c.DateTime(),
                        CreatedById = c.Int(),
                    })
                .PrimaryKey(t => t.ErrorMessageLogId)
                .ForeignKey("dbo.Personnel", t => t.CreatedById)
                .ForeignKey("dbo.Codes", t => t.ErrorTypeCD)
                .ForeignKey("dbo.HL7MessageLog", t => t.HL7MessageLogId, cascadeDelete: true)
                .ForeignKey("dbo.Codes", t => t.SourceSystemCD)
                .Index(t => t.HL7MessageLogId)
                .Index(t => t.ErrorTypeCD)
                .Index(t => t.SourceSystemCD)
                .Index(t => t.CreatedById);
            
            CreateTable(
                "dbo.ProcedeedMessageLogs",
                c => new
                    {
                        ProcedeedMessageLogId = c.Int(nullable: false, identity: true),
                        HL7MessageLogId = c.Int(nullable: false),
                        PatientId = c.Int(),
                        EncounterId = c.Int(),
                        TransactionType = c.String(),
                        FhirResource = c.String(),
                        HL7EventType = c.String(),
                        SourceSystemCD = c.Int(),
                        TransactionDirectionCD = c.Int(),
                        TransactionDatetime = c.DateTime(nullable: false),
                        Active = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        EntryDatetime = c.DateTime(nullable: false),
                        LastUpdate = c.DateTime(),
                        CreatedById = c.Int(),
                    })
                .PrimaryKey(t => t.ProcedeedMessageLogId)
                .ForeignKey("dbo.Personnel", t => t.CreatedById)
                .ForeignKey("dbo.Encounters", t => t.EncounterId)
                .ForeignKey("dbo.HL7MessageLog", t => t.HL7MessageLogId, cascadeDelete: true)
                .ForeignKey("dbo.Patients", t => t.PatientId)
                .ForeignKey("dbo.Codes", t => t.SourceSystemCD)
                .ForeignKey("dbo.Codes", t => t.TransactionDirectionCD)
                .Index(t => t.HL7MessageLogId)
                .Index(t => t.PatientId)
                .Index(t => t.EncounterId)
                .Index(t => t.SourceSystemCD)
                .Index(t => t.TransactionDirectionCD)
                .Index(t => t.CreatedById);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProcedeedMessageLogs", "TransactionDirectionCD", "dbo.Codes");
            DropForeignKey("dbo.ProcedeedMessageLogs", "SourceSystemCD", "dbo.Codes");
            DropForeignKey("dbo.ProcedeedMessageLogs", "PatientId", "dbo.Patients");
            DropForeignKey("dbo.ProcedeedMessageLogs", "HL7MessageLogId", "dbo.HL7MessageLog");
            DropForeignKey("dbo.ProcedeedMessageLogs", "EncounterId", "dbo.Encounters");
            DropForeignKey("dbo.ProcedeedMessageLogs", "CreatedById", "dbo.Personnel");
            DropForeignKey("dbo.ErrorMessageLogs", "SourceSystemCD", "dbo.Codes");
            DropForeignKey("dbo.ErrorMessageLogs", "HL7MessageLogId", "dbo.HL7MessageLog");
            DropForeignKey("dbo.ErrorMessageLogs", "ErrorTypeCD", "dbo.Codes");
            DropForeignKey("dbo.ErrorMessageLogs", "CreatedById", "dbo.Personnel");
            DropIndex("dbo.ProcedeedMessageLogs", new[] { "CreatedById" });
            DropIndex("dbo.ProcedeedMessageLogs", new[] { "TransactionDirectionCD" });
            DropIndex("dbo.ProcedeedMessageLogs", new[] { "SourceSystemCD" });
            DropIndex("dbo.ProcedeedMessageLogs", new[] { "EncounterId" });
            DropIndex("dbo.ProcedeedMessageLogs", new[] { "PatientId" });
            DropIndex("dbo.ProcedeedMessageLogs", new[] { "HL7MessageLogId" });
            DropIndex("dbo.ErrorMessageLogs", new[] { "CreatedById" });
            DropIndex("dbo.ErrorMessageLogs", new[] { "SourceSystemCD" });
            DropIndex("dbo.ErrorMessageLogs", new[] { "ErrorTypeCD" });
            DropIndex("dbo.ErrorMessageLogs", new[] { "HL7MessageLogId" });
            DropTable("dbo.ProcedeedMessageLogs");
            DropTable("dbo.ErrorMessageLogs");
        }
    }
}
