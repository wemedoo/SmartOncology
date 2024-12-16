namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPersonnelOccupationFKToPersonnel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Personnel", "PersonnelOccupationId", c => c.Int());
            CreateIndex("dbo.Personnel", "PersonnelOccupationId");
            AddForeignKey("dbo.Personnel", "PersonnelOccupationId", "dbo.PersonnelOccupations", "PersonnelOccupationId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Personnel", "PersonnelOccupationId", "dbo.PersonnelOccupations");
            DropIndex("dbo.Personnel", new[] { "PersonnelOccupationId" });
            DropColumn("dbo.Personnel", "PersonnelOccupationId");
        }
    }
}
