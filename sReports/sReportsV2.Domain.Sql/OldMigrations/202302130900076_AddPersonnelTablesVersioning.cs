namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPersonnelTablesVersioning : DbMigration
    {
        public override void Up()
        {
            using (SReportsContext context = new SReportsContext())
            {
                context.SetSystemVersionedTables("dbo.PersonnelAcademicPositions");
                context.CreateIndexesOnCommonProperties("dbo.PersonnelAcademicPositions");
                context.SetSystemVersionedTables("dbo.PatientIdentifiers");
                context.CreateIndexesOnCommonProperties("dbo.PatientIdentifiers");
                context.SetSystemVersionedTables("dbo.PersonnelAddresses");
                context.CreateIndexesOnCommonProperties("dbo.PersonnelAddresses");
                context.SetSystemVersionedTables("dbo.PersonnelIdentifiers");
                context.CreateIndexesOnCommonProperties("dbo.PersonnelIdentifiers");
                context.SetSystemVersionedTables("dbo.PersonnelPositions");
                context.CreateIndexesOnCommonProperties("dbo.PersonnelPositions");
                context.SetSystemVersionedTables("dbo.PositionPermissions");
                context.CreateIndexesOnCommonProperties("dbo.PositionPermissions");
            }
        }

        public override void Down()
        {
            using (SReportsContext context = new SReportsContext())
            {
                context.DropIndexesOnCommonProperties("dbo.PositionPermissions");
                context.UnsetSystemVersionedTables("dbo.PositionPermissions");
                context.DropIndexesOnCommonProperties("dbo.PersonnelPositions");
                context.UnsetSystemVersionedTables("dbo.PersonnelPositions");
                context.DropIndexesOnCommonProperties("dbo.PersonnelIdentifiers");
                context.UnsetSystemVersionedTables("dbo.PersonnelIdentifiers");
                context.DropIndexesOnCommonProperties("dbo.PersonnelAddresses");
                context.UnsetSystemVersionedTables("dbo.PersonnelAddresses");
                context.DropIndexesOnCommonProperties("dbo.PatientIdentifiers");
                context.UnsetSystemVersionedTables("dbo.PatientIdentifiers");
                context.DropIndexesOnCommonProperties("dbo.PersonnelAcademicPositions");
                context.UnsetSystemVersionedTables("dbo.PersonnelAcademicPositions");
            }
        }
    }
}
