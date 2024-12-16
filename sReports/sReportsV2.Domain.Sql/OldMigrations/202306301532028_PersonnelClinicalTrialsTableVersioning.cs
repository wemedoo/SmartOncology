namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System.Data.Entity.Migrations;

    public partial class PersonnelClinicalTrialsTableVersioning : DbMigration
    {
        public override void Up()
        {
            using (SReportsContext context = new SReportsContext())
            {
                context.SetSystemVersionedTables("dbo.PersonnelClinicalTrials");
                context.CreateIndexesOnCommonProperties("dbo.PersonnelClinicalTrials");
            }
        }

        public override void Down()
        {
            using (SReportsContext context = new SReportsContext())
            {
                context.DropIndexesOnCommonProperties("dbo.PersonnelClinicalTrials");
                context.UnsetSystemVersionedTables("dbo.PersonnelClinicalTrials");
            }
        }
    }
}
