namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyPatientEncounterDoctorRelation : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Encounters", "AdmittingDoctorId", "dbo.Personnel");
            DropForeignKey("dbo.Encounters", "AttendingDoctorId", "dbo.Personnel");
            DropForeignKey("dbo.Encounters", "ConsultingDoctorId", "dbo.Personnel");
            DropForeignKey("dbo.Encounters", "ReferringDoctorId", "dbo.Personnel");
            DropForeignKey("dbo.AttendingDoctors", "CreatedById", "dbo.Personnel");
            DropForeignKey("dbo.AttendingDoctors", "EncounterId", "dbo.Encounters");
            DropForeignKey("dbo.AttendingDoctors", "EntityStateCD", "dbo.Codes");
            DropForeignKey("dbo.AttendingDoctors", "PersonnelId", "dbo.Personnel");
            DropIndex("dbo.AttendingDoctors", new[] { "EncounterId" });
            DropIndex("dbo.AttendingDoctors", new[] { "PersonnelId" });
            DropIndex("dbo.AttendingDoctors", new[] { "CreatedById" });
            DropIndex("dbo.AttendingDoctors", new[] { "EntityStateCD" });
            DropIndex("dbo.Encounters", new[] { "AttendingDoctorId" });
            DropIndex("dbo.Encounters", new[] { "ReferringDoctorId" });
            DropIndex("dbo.Encounters", new[] { "ConsultingDoctorId" });
            DropIndex("dbo.Encounters", new[] { "AdmittingDoctorId" });
            DropColumn("dbo.Encounters", "AttendingDoctorId");
            DropColumn("dbo.Encounters", "ReferringDoctorId");
            DropColumn("dbo.Encounters", "ConsultingDoctorId");
            DropColumn("dbo.Encounters", "AdmittingDoctorId");
            DropTable("dbo.AttendingDoctors");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.AttendingDoctors",
                c => new
                    {
                        AttendingDoctorId = c.Int(nullable: false, identity: true),
                        EncounterId = c.Int(nullable: false),
                        PersonnelId = c.Int(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        EntryDatetime = c.DateTime(nullable: false),
                        LastUpdate = c.DateTime(),
                        CreatedById = c.Int(),
                        ActiveFrom = c.DateTime(nullable: false),
                        ActiveTo = c.DateTime(nullable: false),
                        EntityStateCD = c.Int(),
                    })
                .PrimaryKey(t => t.AttendingDoctorId);
            
            AddColumn("dbo.Encounters", "AdmittingDoctorId", c => c.Int());
            AddColumn("dbo.Encounters", "ConsultingDoctorId", c => c.Int());
            AddColumn("dbo.Encounters", "ReferringDoctorId", c => c.Int());
            AddColumn("dbo.Encounters", "AttendingDoctorId", c => c.Int());
            CreateIndex("dbo.Encounters", "AdmittingDoctorId");
            CreateIndex("dbo.Encounters", "ConsultingDoctorId");
            CreateIndex("dbo.Encounters", "ReferringDoctorId");
            CreateIndex("dbo.Encounters", "AttendingDoctorId");
            CreateIndex("dbo.AttendingDoctors", "EntityStateCD");
            CreateIndex("dbo.AttendingDoctors", "CreatedById");
            CreateIndex("dbo.AttendingDoctors", "PersonnelId");
            CreateIndex("dbo.AttendingDoctors", "EncounterId");
            AddForeignKey("dbo.AttendingDoctors", "PersonnelId", "dbo.Personnel", "PersonnelId", cascadeDelete: true);
            AddForeignKey("dbo.AttendingDoctors", "EntityStateCD", "dbo.Codes", "CodeId");
            AddForeignKey("dbo.AttendingDoctors", "EncounterId", "dbo.Encounters", "EncounterId", cascadeDelete: true);
            AddForeignKey("dbo.AttendingDoctors", "CreatedById", "dbo.Personnel", "PersonnelId");
            AddForeignKey("dbo.Encounters", "ReferringDoctorId", "dbo.Personnel", "PersonnelId");
            AddForeignKey("dbo.Encounters", "ConsultingDoctorId", "dbo.Personnel", "PersonnelId");
            AddForeignKey("dbo.Encounters", "AttendingDoctorId", "dbo.Personnel", "PersonnelId");
            AddForeignKey("dbo.Encounters", "AdmittingDoctorId", "dbo.Personnel", "PersonnelId");
        }
    }
}
