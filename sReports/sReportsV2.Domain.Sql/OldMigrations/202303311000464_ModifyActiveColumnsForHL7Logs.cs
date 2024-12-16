namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyActiveColumnsForHL7Logs : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.EncounterIdentifiers", "Active");
            DropColumn("dbo.ErrorMessageLogs", "Active");
            DropColumn("dbo.HL7MessageLog", "Active");
            DropColumn("dbo.ProcedeedMessageLogs", "Active");
            AddColumn("dbo.EncounterIdentifiers", "ActiveFrom", c => c.DateTime(nullable: false, defaultValueSql: "CONVERT(datetime, '2022-01-01 12:00:00', 120)"));
            AddColumn("dbo.EncounterIdentifiers", "ActiveTo", c => c.DateTime(nullable: false, defaultValueSql: "CONVERT(datetime, '9999-12-31 23:59:59', 120)"));
            AddColumn("dbo.ErrorMessageLogs", "ActiveFrom", c => c.DateTime(nullable: false, defaultValueSql: "CONVERT(datetime, '2022-01-01 12:00:00', 120)"));
            AddColumn("dbo.ErrorMessageLogs", "ActiveTo", c => c.DateTime(nullable: false, defaultValueSql: "CONVERT(datetime, '9999-12-31 23:59:59', 120)"));
            AddColumn("dbo.HL7MessageLog", "ActiveFrom", c => c.DateTime(nullable: false, defaultValueSql: "CONVERT(datetime, '2022-01-01 12:00:00', 120)"));
            AddColumn("dbo.HL7MessageLog", "ActiveTo", c => c.DateTime(nullable: false, defaultValueSql: "CONVERT(datetime, '9999-12-31 23:59:59', 120)"));
            AddColumn("dbo.ProcedeedMessageLogs", "ActiveFrom", c => c.DateTime(nullable: false, defaultValueSql: "CONVERT(datetime, '2022-01-01 12:00:00', 120)"));
            AddColumn("dbo.ProcedeedMessageLogs", "ActiveTo", c => c.DateTime(nullable: false, defaultValueSql: "CONVERT(datetime, '9999-12-31 23:59:59', 120)"));
        }

        public override void Down()
        {
            DropColumn("dbo.EncounterIdentifiers", "ActiveTo");
            DropColumn("dbo.EncounterIdentifiers", "ActiveFrom");
            DropColumn("dbo.ErrorMessageLogs", "ActiveTo");
            DropColumn("dbo.ErrorMessageLogs", "ActiveFrom");
            DropColumn("dbo.HL7MessageLog", "ActiveTo");
            DropColumn("dbo.HL7MessageLog", "ActiveFrom");
            DropColumn("dbo.ProcedeedMessageLogs", "ActiveTo");
            DropColumn("dbo.ProcedeedMessageLogs", "ActiveFrom");
            AddColumn("dbo.EncounterIdentifiers", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.ErrorMessageLogs", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.HL7MessageLog", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.ProcedeedMessageLogs", "Active", c => c.Boolean(nullable: false));
        }
    }
}
