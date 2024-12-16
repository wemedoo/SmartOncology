namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAttendingDoctors : DbMigration
    {
        public override void Up()
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
                .PrimaryKey(t => t.AttendingDoctorId)
                .ForeignKey("dbo.Personnel", t => t.CreatedById)
                .ForeignKey("dbo.Encounters", t => t.EncounterId, cascadeDelete: true)
                .ForeignKey("dbo.Codes", t => t.EntityStateCD)
                .ForeignKey("dbo.Personnel", t => t.PersonnelId, cascadeDelete: true)
                .Index(t => t.EncounterId)
                .Index(t => t.PersonnelId)
                .Index(t => t.CreatedById)
                .Index(t => t.EntityStateCD);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AttendingDoctors", "PersonnelId", "dbo.Personnel");
            DropForeignKey("dbo.AttendingDoctors", "EntityStateCD", "dbo.Codes");
            DropForeignKey("dbo.AttendingDoctors", "EncounterId", "dbo.Encounters");
            DropForeignKey("dbo.AttendingDoctors", "CreatedById", "dbo.Personnel");
            DropIndex("dbo.AttendingDoctors", new[] { "EntityStateCD" });
            DropIndex("dbo.AttendingDoctors", new[] { "CreatedById" });
            DropIndex("dbo.AttendingDoctors", new[] { "PersonnelId" });
            DropIndex("dbo.AttendingDoctors", new[] { "EncounterId" });
            DropTable("dbo.AttendingDoctors");
        }
    }
}
