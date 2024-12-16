namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddVersioningToPersonnelOccupations : DbMigration
    {
        public override void Up()
        {
            using (var context = new SReportsContext())
            {
                context.SetSystemVersionedTables("dbo.PersonnelOccupations");
                context.CreateIndexesOnCommonProperties("dbo.PersonnelOccupations");
            }
        }

        public override void Down()
        {
            using (var context = new SReportsContext())
            {
                context.DropIndexesOnCommonProperties("dbo.PersonnelOccupations");
                context.UnsetSystemVersionedTables("dbo.PersonnelOccupations");
            }
        }
    }
}
