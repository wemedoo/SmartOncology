namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakeEncounterTypeAndClassNullable : DbMigration
    {
        public override void Up()
        {
            SReportsContext context = new SReportsContext();
            string encounterClassCDNullable = "alter table dbo.Encounters alter column ClassCD int null;";
            string encounterTypeCDNullable = "alter table dbo.Encounters alter column TypeCD int null;";
            context.Database.ExecuteSqlCommand(encounterClassCDNullable);
            context.Database.ExecuteSqlCommand(encounterTypeCDNullable);
        }
        
        public override void Down()
        {
            SReportsContext context = new SReportsContext();
            string encounterClassCDNotNullable = "alter table dbo.Encounters alter column ClassCD int not null;";
            string encounterTypeCDNotNullable = "alter table dbo.Encounters alter column TypeCD int not null;";
            context.Database.ExecuteSqlCommand(encounterClassCDNotNullable);
            context.Database.ExecuteSqlCommand(encounterTypeCDNotNullable);
        }
    }
}
