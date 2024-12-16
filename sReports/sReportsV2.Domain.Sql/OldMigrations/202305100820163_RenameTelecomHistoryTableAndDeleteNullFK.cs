namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameTelecomHistoryTableAndDeleteNullFK : DbMigration
    {
        public override void Up()
        {
            SReportsContext context = new SReportsContext();
            string removeNullFKEntries = "delete from dbo.OrganizationTelecoms where OrganizationId is null";
            string renameHistoryTable = "EXEC sp_rename 'dbo.TelecomsHistory', 'OrganizationTelecomsHistory';";

            context.Database.ExecuteSqlCommand(removeNullFKEntries);
            context.Database.ExecuteSqlCommand(renameHistoryTable);

        }
        
        public override void Down()
        {
            SReportsContext context = new SReportsContext();
            string renameHistoryTableToPrevious = "";

            context.Database.ExecuteSqlCommand(renameHistoryTableToPrevious);
        }
    }
}
