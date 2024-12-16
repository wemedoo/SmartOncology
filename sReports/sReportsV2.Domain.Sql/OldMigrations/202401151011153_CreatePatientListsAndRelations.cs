namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreatePatientListsAndRelations : DbMigration
    {
        public override void Up()
        {            
            CreateTable(
                "dbo.PatientLists",
                c => new
                    {
                        PatientListId = c.Int(nullable: false, identity: true),
                        PatientListName = c.String(),
                        ArePatientsSelected = c.Boolean(nullable: false),
                        PatientActiveFrom = c.DateTime(),
                        PatientActiveTo = c.DateTime(),
                        EpisodeOfCareTypeCD = c.Int(),
                        PersonnelTeamId = c.Int(),
                        EncounterTypeCD = c.Int(),
                        AttendingDoctorId = c.Int(),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        EntryDatetime = c.DateTimeOffset(nullable: false, precision: 7),
                        LastUpdate = c.DateTimeOffset(precision: 7),
                        CreatedById = c.Int(nullable: false),
                        ActiveFrom = c.DateTimeOffset(nullable: false, precision: 7),
                        ActiveTo = c.DateTimeOffset(nullable: false, precision: 7),
                        EntityStateCD = c.Int(),
                    })
                .PrimaryKey(t => t.PatientListId)
                .ForeignKey("dbo.Personnel", t => t.AttendingDoctorId)
                .ForeignKey("dbo.Personnel", t => t.CreatedById)
                .ForeignKey("dbo.Codes", t => t.EncounterTypeCD)
                .ForeignKey("dbo.Codes", t => t.EntityStateCD)
                .ForeignKey("dbo.Codes", t => t.EpisodeOfCareTypeCD)
                .ForeignKey("dbo.PersonnelTeams", t => t.PersonnelTeamId)
                .Index(t => t.EpisodeOfCareTypeCD)
                .Index(t => t.PersonnelTeamId)
                .Index(t => t.EncounterTypeCD)
                .Index(t => t.AttendingDoctorId)
                .Index(t => t.CreatedById)
                .Index(t => t.EntityStateCD);
            
            CreateTable(
                "dbo.PatientListPersonnelRelations",
                c => new
                    {
                        PatientListPersonnelRelationId = c.Int(nullable: false, identity: true),
                        PersonnelId = c.Int(nullable: false),
                        PatientListId = c.Int(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        EntryDatetime = c.DateTimeOffset(nullable: false, precision: 7),
                        LastUpdate = c.DateTimeOffset(precision: 7),
                        CreatedById = c.Int(nullable: false),
                        ActiveFrom = c.DateTimeOffset(nullable: false, precision: 7),
                        ActiveTo = c.DateTimeOffset(nullable: false, precision: 7),
                        EntityStateCD = c.Int(),
                    })
                .PrimaryKey(t => t.PatientListPersonnelRelationId)
                .ForeignKey("dbo.Personnel", t => t.CreatedById)
                .ForeignKey("dbo.Codes", t => t.EntityStateCD)
                .ForeignKey("dbo.PatientLists", t => t.PatientListId, cascadeDelete: true)
                .ForeignKey("dbo.Personnel", t => t.PersonnelId, cascadeDelete: true)
                .Index(t => t.PersonnelId)
                .Index(t => t.PatientListId)
                .Index(t => t.CreatedById)
                .Index(t => t.EntityStateCD);

            CreateTable(
                "dbo.PatientListPatientRelations",
                c => new
                    {
                        PatientListPatientRelationId = c.Int(nullable: false, identity: true),
                        PatientId = c.Int(nullable: false),
                        PatientListId = c.Int(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        EntryDatetime = c.DateTimeOffset(nullable: false, precision: 7),
                        LastUpdate = c.DateTimeOffset(precision: 7),
                        CreatedById = c.Int(),
                        ActiveFrom = c.DateTimeOffset(nullable: false, precision: 7),
                        ActiveTo = c.DateTimeOffset(nullable: false, precision: 7),
                        EntityStateCD = c.Int(),
                    })
                .PrimaryKey(t => t.PatientListPatientRelationId)
                .ForeignKey("dbo.Personnel", t => t.CreatedById)
                .ForeignKey("dbo.Codes", t => t.EntityStateCD)
                .ForeignKey("dbo.Patients", t => t.PatientId, cascadeDelete: true)
                .ForeignKey("dbo.PatientLists", t => t.PatientListId, cascadeDelete: true)
                .Index(t => t.PatientId)
                .Index(t => t.PatientListId)
                .Index(t => t.CreatedById)
                .Index(t => t.EntityStateCD);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PatientListPersonnelRelations", "PersonnelId", "dbo.Personnel");
            DropForeignKey("dbo.PatientLists", "PersonnelTeamId", "dbo.PersonnelTeams");
            DropForeignKey("dbo.PatientListPersonnelRelations", "PatientListId", "dbo.PatientLists");
            DropForeignKey("dbo.PatientListPatientRelations", "PatientListId", "dbo.PatientLists");
            DropForeignKey("dbo.PatientListPatientRelations", "PatientId", "dbo.Patients");
            DropForeignKey("dbo.PatientListPatientRelations", "EntityStateCD", "dbo.Codes");
            DropForeignKey("dbo.PatientListPatientRelations", "CreatedById", "dbo.Personnel");
            DropForeignKey("dbo.PatientLists", "EpisodeOfCareTypeCD", "dbo.Codes");
            DropForeignKey("dbo.PatientLists", "EntityStateCD", "dbo.Codes");
            DropForeignKey("dbo.PatientLists", "EncounterTypeCD", "dbo.Codes");
            DropForeignKey("dbo.PatientLists", "CreatedById", "dbo.Personnel");
            DropForeignKey("dbo.PatientLists", "AttendingDoctorId", "dbo.Personnel");
            DropForeignKey("dbo.PatientListPersonnelRelations", "EntityStateCD", "dbo.Codes");
            DropForeignKey("dbo.PatientListPersonnelRelations", "CreatedById", "dbo.Personnel");
            DropIndex("dbo.PatientListPatientRelations", new[] { "EntityStateCD" });
            DropIndex("dbo.PatientListPatientRelations", new[] { "CreatedById" });
            DropIndex("dbo.PatientListPatientRelations", new[] { "PatientListId" });
            DropIndex("dbo.PatientListPatientRelations", new[] { "PatientId" });
            DropIndex("dbo.PatientLists", new[] { "EntityStateCD" });
            DropIndex("dbo.PatientLists", new[] { "CreatedById" });
            DropIndex("dbo.PatientLists", new[] { "AttendingDoctorId" });
            DropIndex("dbo.PatientLists", new[] { "EncounterTypeCD" });
            DropIndex("dbo.PatientLists", new[] { "PersonnelTeamId" });
            DropIndex("dbo.PatientLists", new[] { "EpisodeOfCareTypeCD" });
            DropIndex("dbo.PatientListPersonnelRelations", new[] { "EntityStateCD" });
            DropIndex("dbo.PatientListPersonnelRelations", new[] { "CreatedById" });
            DropIndex("dbo.PatientListPersonnelRelations", new[] { "PatientListId" });
            DropIndex("dbo.PatientListPersonnelRelations", new[] { "PersonnelId" });
            DropTable("dbo.PatientListPatientRelations");
            DropTable("dbo.PatientListPersonnelRelations");
            DropTable("dbo.PatientLists");
        }
    }
}
