namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddVersioningToProjectRelationTables : DbMigration
    {
        public override void Up()
        {
            using (var context = new SReportsContext())
            {
                context.SetSystemVersionedTables("dbo.ProjectPersonnelRelations");
                context.CreateIndexesOnCommonProperties("dbo.ProjectPersonnelRelations");
                context.SetSystemVersionedTables("dbo.ProjectDocumentRelations");
                context.CreateIndexesOnCommonProperties("dbo.ProjectDocumentRelations");
                context.SetSystemVersionedTables("dbo.ProjectPatientRelations");
                context.CreateIndexesOnCommonProperties("dbo.ProjectPatientRelations");
            }
        }

        public override void Down()
        {
            using (var context = new SReportsContext())
            {
                context.DropIndexesOnCommonProperties("dbo.ProjectPatientRelations");
                context.UnsetSystemVersionedTables("dbo.ProjectPatientRelations");
                context.DropIndexesOnCommonProperties("dbo.ProjectDocumentRelations");
                context.UnsetSystemVersionedTables("dbo.ProjectDocumentRelations");
                context.DropIndexesOnCommonProperties("dbo.ProjectPersonnelRelations");
                context.UnsetSystemVersionedTables("dbo.ProjectPersonnelRelations");
            }
        }
    }
}
