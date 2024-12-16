namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameTaskDocumentFKForTasks : DbMigration
    {
        public override void Up()
        {
            SReportsContext context = new SReportsContext();
            string removeFk = $@"ALTER TABLE [dbo].[Tasks]
                            DROP CONSTRAINT [FK_dbo.Tasks_dbo.Codes_TaskDocumentCD]";
            string addFk = $@"ALTER TABLE [dbo].[Tasks]
                            ADD CONSTRAINT [FK_dbo.Tasks_dbo.Codes_TaskDocumentId]
                            FOREIGN KEY ([TaskDocumentId])
                            REFERENCES [dbo].[Codes] ([CodeId])";

            string updateTasks = @"UPDATE [dbo].[Tasks]
                            SET ActiveTo = GETDATE(),
                                EntityStateCD = 2003,
                                TaskDocumentId = NULL";
            context.Database.ExecuteSqlCommand(removeFk);
            context.Database.ExecuteSqlCommand(addFk);
            context.Database.ExecuteSqlCommand(updateTasks);
        }

        public override void Down()
        {
        }
    }
}
