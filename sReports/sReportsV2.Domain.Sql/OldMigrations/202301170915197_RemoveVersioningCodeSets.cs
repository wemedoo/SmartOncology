namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveVersioningCodeSets : DbMigration
    {
        public override void Up()
        {
            using (SReportsContext context = new SReportsContext())
            {
                context.DropIndexesOnCommonProperties("dbo.CodeSets");
                context.UnsetSystemVersionedTables("dbo.CodeSets");
            }
        }

        public override void Down()
        {
            using (SReportsContext context = new SReportsContext())
            {
                context.SetSystemVersionedTables("dbo.CodeSets");
                context.CreateIndexesOnCommonProperties("dbo.CodeSets");
            }
        }
    }
}
