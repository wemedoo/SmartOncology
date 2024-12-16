namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCodeSetIdFkTOCodes : DbMigration
    {
        public override void Up()
        {
            string addColumn = @"ALTER TABLE [dbo].[Codes]
                                ADD CodeSetId int";

            string addForeignKey = @"ALTER TABLE [dbo].[Codes]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Codes_dbo.CodeSets_CodeSetId] FOREIGN KEY([CodeSetId])
                                    REFERENCES [dbo].[CodeSets] ([CodeSetId])
                                    ON Update CASCADE

                                    ALTER TABLE [dbo].[Codes] CHECK CONSTRAINT [FK_dbo.Codes_dbo.CodeSets_CodeSetId]";

            string addIndex = @"CREATE NONCLUSTERED INDEX [IX_CodeSetId] ON [dbo].[Codes]
                                (
	                                [CodeSetId] ASC
                                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]";

            SReportsContext sReportsContext = new SReportsContext();
            sReportsContext.Database.ExecuteSqlCommand(addColumn);
            sReportsContext.Database.ExecuteSqlCommand(addForeignKey);
            sReportsContext.Database.ExecuteSqlCommand(addIndex);
        }
        
        public override void Down()
        {
        }
    }
}
