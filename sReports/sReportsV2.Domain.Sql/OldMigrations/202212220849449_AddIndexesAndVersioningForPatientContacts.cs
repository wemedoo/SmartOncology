namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIndexesAndVersioningForPatientContacts : DbMigration
    {
        public override void Up()
        {
            using (SReportsContext context = new SReportsContext())
            {
                context.SetSystemVersionedTables("dbo.PatientContacts");
                context.CreateIndexesOnCommonProperties("dbo.PatientContacts");
            }
        }

        public override void Down()
        {
            using (SReportsContext context = new SReportsContext())
            {
                context.DropIndexesOnCommonProperties("dbo.PatientContacts");
                context.UnsetSystemVersionedTables("dbo.PatientContacts");
            }
        }
    }
}
