namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TelecomPropertiesCD : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Telecoms", "SystemCD", c => c.Int());
            AddColumn("dbo.Telecoms", "UseCD", c => c.Int());
            AddColumn("dbo.PatientTelecoms", "SystemCD", c => c.Int());
            AddColumn("dbo.PatientTelecoms", "UseCD", c => c.Int());
            AddColumn("dbo.PatientContactTelecoms", "SystemCD", c => c.Int());
            AddColumn("dbo.PatientContactTelecoms", "UseCD", c => c.Int());
            CreateIndex("dbo.Telecoms", "SystemCD");
            CreateIndex("dbo.Telecoms", "UseCD");
            CreateIndex("dbo.PatientTelecoms", "SystemCD");
            CreateIndex("dbo.PatientTelecoms", "UseCD");
            CreateIndex("dbo.PatientContactTelecoms", "SystemCD");
            CreateIndex("dbo.PatientContactTelecoms", "UseCD");
            AddForeignKey("dbo.Telecoms", "SystemCD", "dbo.Codes", "CodeId");
            AddForeignKey("dbo.Telecoms", "UseCD", "dbo.Codes", "CodeId");
            AddForeignKey("dbo.PatientTelecoms", "SystemCD", "dbo.Codes", "CodeId");
            AddForeignKey("dbo.PatientTelecoms", "UseCD", "dbo.Codes", "CodeId");
            AddForeignKey("dbo.PatientContactTelecoms", "SystemCD", "dbo.Codes", "CodeId");
            AddForeignKey("dbo.PatientContactTelecoms", "UseCD", "dbo.Codes", "CodeId");
            DropColumn("dbo.Telecoms", "System");
            DropColumn("dbo.Telecoms", "Use");
            DropColumn("dbo.PatientTelecoms", "System");
            DropColumn("dbo.PatientTelecoms", "Use");
            DropColumn("dbo.PatientContactTelecoms", "System");
            DropColumn("dbo.PatientContactTelecoms", "Use");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PatientContactTelecoms", "Use", c => c.String());
            AddColumn("dbo.PatientContactTelecoms", "System", c => c.String());
            AddColumn("dbo.PatientTelecoms", "Use", c => c.String());
            AddColumn("dbo.PatientTelecoms", "System", c => c.String());
            AddColumn("dbo.Telecoms", "Use", c => c.String());
            AddColumn("dbo.Telecoms", "System", c => c.String());
            DropForeignKey("dbo.PatientContactTelecoms", "UseCD", "dbo.Codes");
            DropForeignKey("dbo.PatientContactTelecoms", "SystemCD", "dbo.Codes");
            DropForeignKey("dbo.PatientTelecoms", "UseCD", "dbo.Codes");
            DropForeignKey("dbo.PatientTelecoms", "SystemCD", "dbo.Codes");
            DropForeignKey("dbo.Telecoms", "UseCD", "dbo.Codes");
            DropForeignKey("dbo.Telecoms", "SystemCD", "dbo.Codes");
            DropIndex("dbo.PatientContactTelecoms", new[] { "UseCD" });
            DropIndex("dbo.PatientContactTelecoms", new[] { "SystemCD" });
            DropIndex("dbo.PatientTelecoms", new[] { "UseCD" });
            DropIndex("dbo.PatientTelecoms", new[] { "SystemCD" });
            DropIndex("dbo.Telecoms", new[] { "UseCD" });
            DropIndex("dbo.Telecoms", new[] { "SystemCD" });
            DropColumn("dbo.PatientContactTelecoms", "UseCD");
            DropColumn("dbo.PatientContactTelecoms", "SystemCD");
            DropColumn("dbo.PatientTelecoms", "UseCD");
            DropColumn("dbo.PatientTelecoms", "SystemCD");
            DropColumn("dbo.Telecoms", "UseCD");
            DropColumn("dbo.Telecoms", "SystemCD");
        }
    }
}
