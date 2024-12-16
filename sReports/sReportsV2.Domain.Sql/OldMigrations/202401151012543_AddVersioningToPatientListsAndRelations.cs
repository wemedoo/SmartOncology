namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System.Data.Entity.Migrations;

    public partial class AddVersioningToPatientListsAndRelations : DbMigration
    {
        public override void Up()
        {
            using (var context = new SReportsContext())
            {
                context.SetSystemVersionedTables("dbo.PatientLists");
                context.CreateIndexesOnCommonProperties("dbo.PatientLists");
                context.SetSystemVersionedTables("dbo.PatientListPersonnelRelations");
                context.CreateIndexesOnCommonProperties("dbo.PatientListPersonnelRelations");
                context.SetSystemVersionedTables("dbo.PatientListPatientRelations");
                context.CreateIndexesOnCommonProperties("dbo.PatientListPatientRelations");
            }
        }

        public override void Down()
        {
            using (var context = new SReportsContext())
            {
                context.DropIndexesOnCommonProperties("dbo.PatientLists");
                context.UnsetSystemVersionedTables("dbo.PatientLists");
                context.DropIndexesOnCommonProperties("dbo.PatientListPersonnelRelations");
                context.UnsetSystemVersionedTables("dbo.PatientListPersonnelRelations");
                context.DropIndexesOnCommonProperties("dbo.PatientListPatientRelations");
                context.UnsetSystemVersionedTables("dbo.PatientListPatientRelations");
            }
        }
    }
}
