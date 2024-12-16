namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;

    public partial class SaveExistingIdentifierData : DbMigration
    {
        public override void Up()
        {
            SReportsContext context = new SReportsContext();
            string updateValues = @"
                update dbo.Identifiers set IdentifierValue = [Value], IdentifierTypeCD = [System], IdentifierUseCD = [Use];
            ";
            string dropColumns = @"
                alter table dbo.Identifiers drop column [System], [Value], [Use], [Type];
            ";
            context.Database.ExecuteSqlCommand(updateValues);
            context.Database.ExecuteSqlCommand(dropColumns);
        }

        public override void Down()
        {
            SReportsContext context = new SReportsContext();
            string addColumns = @"
                alter table dbo.Identifiers 
                add [System] nvarchar(max),
                [Value] nvarchar(max),
                [Type] nvarchar(max),
                [Use] nvarchar(max);
            ";
            string updateValues = @"
                update dbo.Identifiers set [Value] = IdentifierValue, [System] = IdentifierTypeCD, [Use] = IdentifierUseCD;
            ";
            context.Database.ExecuteSqlCommand(addColumns);
            context.Database.ExecuteSqlCommand(updateValues);
        }
    }
}
