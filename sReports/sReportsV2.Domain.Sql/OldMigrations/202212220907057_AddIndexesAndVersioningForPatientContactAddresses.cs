namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIndexesAndVersioningForPatientContactAddresses : DbMigration
    {
        public override void Up()
        {
            using (SReportsContext context = new SReportsContext())
            {
                context.SetSystemVersionedTables("dbo.PatientContactAddresses");
                context.CreateIndexesOnCommonProperties("dbo.PatientContactAddresses");
            }
        }

        public override void Down()
        {
            using (SReportsContext context = new SReportsContext())
            {
                context.DropIndexesOnCommonProperties("dbo.PatientContactAddresses");
                context.UnsetSystemVersionedTables("dbo.PatientContactAddresses");
            }
        }
    }
}
