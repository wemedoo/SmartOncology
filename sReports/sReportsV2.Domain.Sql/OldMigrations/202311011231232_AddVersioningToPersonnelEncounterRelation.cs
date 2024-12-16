using sReportsV2.DAL.Sql.Sql;
using System.Data.Entity.Migrations;

namespace sReportsV2.Domain.Sql.Migrations
{
    public partial class AddVersioningToPersonnelEncounterRelation : DbMigration
    {
        public override void Up()
        {
            using (var context = new SReportsContext())
            {
                context.SetSystemVersionedTables("dbo.PersonnelEncounterRelations");
                context.CreateIndexesOnCommonProperties("dbo.PersonnelEncounterRelations");
            }
        }

        public override void Down()
        {
            using (var context = new SReportsContext())
            {
                context.DropIndexesOnCommonProperties("dbo.PersonnelEncounterRelations");
                context.UnsetSystemVersionedTables("dbo.PersonnelEncounterRelations");
            }
        }
    }
}
