namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;

    public partial class RemoveNameEmbeddedEntity : DbMigration
    {
        public override void Up()
        {
            using(SReportsContext context = new SReportsContext())
            {
                context.RemoveEmbeddedNameEntity("dbo.Patients");
                context.RemoveEmbeddedNameEntity("dbo.SmartOncologyPatients");
                context.RemoveEmbeddedNameEntity("dbo.PatientContacts");
            }
        }

        public override void Down()
        {
            using (SReportsContext context = new SReportsContext())
            {
                context.RevertEmbeddedNameEntity("dbo.PatientContacts");
                context.RevertEmbeddedNameEntity("dbo.SmartOncologyPatients");
                context.RevertEmbeddedNameEntity("dbo.Patients");
            }
        }
    }
}
