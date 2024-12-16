namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveUnusedCodes : DbMigration
    {
        public override void Up()
        {
            Sql($@"ALTER TABLE [dbo].[Tasks]
                DROP CONSTRAINT [FK_dbo.Tasks_dbo.Codes_TaskDocumentId]");
            Sql($@"ALTER TABLE [dbo].[Tasks]
                            ADD CONSTRAINT [FK_dbo.Tasks_dbo.TaskDocuments_TaskDocumentId]
                            FOREIGN KEY ([TaskDocumentId])
                            REFERENCES [dbo].[TaskDocuments] ([TaskDocumentId])");
            Sql("delete FROM [dbo].[Codes] where CodeSetId is null");
        }

        public override void Down()
        {
        }
    }
}
