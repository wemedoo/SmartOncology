namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIndexesAndVersioningForPatientContactTelecom : DbMigration
    {
        public override void Up()
        {
            using (SReportsContext context = new SReportsContext())
            {
                context.SetSystemVersionedTables("dbo.PatientContactTelecoms");
                context.CreateIndexesOnCommonProperties("dbo.PatientContactTelecoms");
            }
        }

        public override void Down()
        {
            using (SReportsContext context = new SReportsContext())
            {
                context.DropIndexesOnCommonProperties("dbo.PatientContactTelecoms");
                context.UnsetSystemVersionedTables("dbo.PatientContactTelecoms");
            }
        }
    }
}
