namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System.Data.Entity.Migrations;

    public partial class AddVersioningOnEncounterIdentifiers : DbMigration
    {
        public override void Up()
        {
            using (var context = new SReportsContext())
            {
                context.SetSystemVersionedTables("dbo.EncounterIdentifiers");
                context.CreateIndexesOnCommonProperties("dbo.EncounterIdentifiers");
            }
        }
        
        public override void Down()
        {
            using (var context = new SReportsContext())
            {
                context.DropIndexesOnCommonProperties("dbo.EncounterIdentifiers");
                context.UnsetSystemVersionedTables("dbo.EncounterIdentifiers");
            }
        }
    }
}
