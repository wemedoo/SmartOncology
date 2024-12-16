namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveFKCodeSetId : DbMigration
    {
        public override void Up()
        {
            string removeForeignKey = @"ALTER TABLE [dbo].[Codes] DROP CONSTRAINT [FK_dbo.Codes_dbo.CodeSets_CodeSetId]";
            string removeIndex = @"DROP INDEX [IX_CodeSetId] ON [dbo].[Codes]";
            string removeColumn = @"ALTER TABLE [dbo].[Codes] DROP COLUMN CodeSetId";

            SReportsContext sReportsContext = new SReportsContext();
            sReportsContext.Database.ExecuteSqlCommand(removeForeignKey);
            sReportsContext.Database.ExecuteSqlCommand(removeIndex);
            sReportsContext.Database.ExecuteSqlCommand(removeColumn);
        }
        
        public override void Down()
        {
        }
    }
}
