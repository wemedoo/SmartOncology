namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddVersioningForAliasesTable : DbMigration
    {
        public override void Up()
        {
            using (SReportsContext context = new SReportsContext())
            {
                context.SetSystemVersionedTables("dbo.CodeAliases");
                context.CreateIndexesOnCommonProperties("dbo.CodeAliases");
            }
        }
        
        public override void Down()
        {
            using (SReportsContext context = new SReportsContext())
            {
                context.DropIndexesOnCommonProperties("dbo.CodeAliases");
                context.UnsetSystemVersionedTables("dbo.CodeAliases");
            }
        }
    }
}
