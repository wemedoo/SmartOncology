namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddClinicalTrialRelatedTablesVersioning : DbMigration
    {
        public override void Up()
        {
            using (SReportsContext context = new SReportsContext())
            {
                context.SetSystemVersionedTables("dbo.ClinicalTrialPersonnelRelations");
                context.CreateIndexesOnCommonProperties("dbo.ClinicalTrialPersonnelRelations");

                context.SetSystemVersionedTables("dbo.ClinicalTrialDocumentRelations");
                context.CreateIndexesOnCommonProperties("dbo.ClinicalTrialDocumentRelations");

                context.SetSystemVersionedTables("dbo.ClinicalTrialPatientRelations");
                context.CreateIndexesOnCommonProperties("dbo.ClinicalTrialPatientRelations");
            }
        }

        public override void Down()
        {
            using (SReportsContext context = new SReportsContext())
            {
                context.DropIndexesOnCommonProperties("dbo.ClinicalTrialPersonnelRelations");
                context.UnsetSystemVersionedTables("dbo.ClinicalTrialPersonnelRelations");

                context.DropIndexesOnCommonProperties("dbo.ClinicalTrialDocumentRelations");
                context.UnsetSystemVersionedTables("dbo.ClinicalTrialDocumentRelations");

                context.DropIndexesOnCommonProperties("dbo.ClinicalTrialPatientRelations");
                context.UnsetSystemVersionedTables("dbo.ClinicalTrialPatientRelations");
            }
        }
    }
}
