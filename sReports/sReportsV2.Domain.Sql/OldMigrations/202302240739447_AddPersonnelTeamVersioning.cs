namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System.Data.Entity.Migrations;
    
    public partial class AddPersonnelTeamVersioning : DbMigration
    {
        public override void Up()
        {
            using (SReportsContext context = new SReportsContext())
            {
                context.SetSystemVersionedTables("dbo.PersonnelTeams");
                context.CreateIndexesOnCommonProperties("dbo.PersonnelTeams");
                context.SetSystemVersionedTables("dbo.PersonnelTeamRelations");
                context.CreateIndexesOnCommonProperties("dbo.PersonnelTeamRelations");
                context.SetSystemVersionedTables("dbo.PersonnelTeamOrganizationRelations");
                context.CreateIndexesOnCommonProperties("dbo.PersonnelTeamOrganizationRelations");
            }
        }

        public override void Down()
        {
            using (SReportsContext context = new SReportsContext())
            {
                context.DropIndexesOnCommonProperties("dbo.PersonnelTeams");
                context.UnsetSystemVersionedTables("dbo.PersonnelTeams");
                context.DropIndexesOnCommonProperties("dbo.PersonnelTeamRelations");
                context.UnsetSystemVersionedTables("dbo.PersonnelTeamRelations");
                context.DropIndexesOnCommonProperties("dbo.PersonnelTeamOrganizationRelations");
                context.UnsetSystemVersionedTables("dbo.PersonnelTeamOrganizationRelations");
            }
        }
    }
}
