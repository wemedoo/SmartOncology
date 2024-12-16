namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UnsetVersioningForTrialRelationTables : DbMigration
    {
        public override void Up()
        {
            SReportsContext context = new SReportsContext();
            context.DropIndexesOnCommonProperties("dbo.ClinicalTrialPersonnelRelations");
            context.UnsetSystemVersionedTables("dbo.ClinicalTrialPersonnelRelations");
            context.DropIndexesOnCommonProperties("dbo.ClinicalTrialPatientRelations");
            context.UnsetSystemVersionedTables("dbo.ClinicalTrialPatientRelations");
            context.DropIndexesOnCommonProperties("dbo.ClinicalTrialDocumentRelations");
            context.UnsetSystemVersionedTables("dbo.ClinicalTrialDocumentRelations");
        }

        public override void Down()
        {
            SReportsContext context = new SReportsContext();
            context.SetSystemVersionedTables("dbo.ClinicalTrialDocumentRelations");
            context.CreateIndexesOnCommonProperties("dbo.ClinicalTrialDocumentRelations");
            context.SetSystemVersionedTables("dbo.ClinicalTrialPatientRelations");
            context.CreateIndexesOnCommonProperties("dbo.ClinicalTrialPatientRelations");
            context.SetSystemVersionedTables("dbo.ClinicalTrialPersonnelRelations");
            context.CreateIndexesOnCommonProperties("dbo.ClinicalTrialPersonnelRelations");
        }
    }
}
