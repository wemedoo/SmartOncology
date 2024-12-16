namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEntityParentToPatientContact : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PatientContacts", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.PatientContacts", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.PatientContacts", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            AddColumn("dbo.PatientContacts", "EntryDatetime", c => c.DateTime(nullable: false, defaultValueSql: "getdate()"));
            AddColumn("dbo.PatientContacts", "LastUpdate", c => c.DateTime());
            AddColumn("dbo.PatientContacts", "CreatedById", c => c.Int());
            CreateIndex("dbo.PatientContacts", "CreatedById");
            AddForeignKey("dbo.PatientContacts", "CreatedById", "dbo.Users", "UserId");
        }

        public override void Down()
        {
            DropForeignKey("dbo.PatientContacts", "CreatedById", "dbo.Users");
            DropIndex("dbo.PatientContacts", new[] { "CreatedById" });
            DropColumn("dbo.PatientContacts", "CreatedById");
            DropColumn("dbo.PatientContacts", "LastUpdate");
            DropColumn("dbo.PatientContacts", "EntryDatetime");
            DropColumn("dbo.PatientContacts", "RowVersion");
            DropColumn("dbo.PatientContacts", "IsDeleted");
            DropColumn("dbo.PatientContacts", "Active");
        }
    }
}
