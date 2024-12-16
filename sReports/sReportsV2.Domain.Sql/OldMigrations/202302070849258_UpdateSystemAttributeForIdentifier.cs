namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;

    public partial class UpdateSystemAttributeForIdentifier : DbMigration
    {
        public override void Up()
        {
            SReportsContext context = new SReportsContext();
            string script = @"
                  update Identifiers set System = (select code.CodeId from dbo.Codes code where code.ThesaurusEntryId = System and code.CodeSetId is not null) 
                ;
            ";
            context.Database.ExecuteSqlCommand(script);
        }

        public override void Down()
        {
            SReportsContext context = new SReportsContext();
            string script = @"
                update i set i.System = c.ThesaurusEntryId
                from dbo.Identifiers i
                inner join dbo.Codes c on c.CodeId = i.System
                where c.CodeSetId is not null
                ;
            ";
            context.Database.ExecuteSqlCommand(script);
        }
    }
}
