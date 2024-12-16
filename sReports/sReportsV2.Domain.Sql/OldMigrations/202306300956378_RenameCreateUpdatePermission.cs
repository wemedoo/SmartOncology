namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameCreateUpdatePermission : DbMigration
    {
        public override void Up()
        {
            SReportsContext context = new SReportsContext();
            string renameCreateUpdate = $@"update [dbo].[Permissions]
                              set Name='Update', Description='Update entity'
                              where Name='CreateUpdate'";
            string renameCreateUpdateCode = $@"update [dbo].[Permissions]
                                              set Name='UpdateCode', Description='Update Code Entity'
                                              where Name='CreateUpdateCode'";

            string renameCreateUpdateAlias = @"update [dbo].[Permissions]
                                              set Name='UpdateAlias', Description='Update Alias Entity'
                                              where Name='CreateUpdateAlias'";

            string renameCreateUpdateAssociation = @"update [dbo].[Permissions]
                                              set Name='UpdateAssociation', Description='Update Association Entity'
                                              where Name='CreateUpdateAssociation'";
            context.Database.ExecuteSqlCommand(renameCreateUpdate);
            context.Database.ExecuteSqlCommand(renameCreateUpdateCode);
            context.Database.ExecuteSqlCommand(renameCreateUpdateAlias);
            context.Database.ExecuteSqlCommand(renameCreateUpdateAssociation);
        }
        
        public override void Down()
        {
            SReportsContext context = new SReportsContext();
            string renameCreateUpdate = $@"update [dbo].[Permissions]
                              set Name='CreateUpdate', Description='Create or update entity'
                              where Name='Update'";
            string renameCreateUpdateCode = $@"update [dbo].[Permissions]
                                              set Name='CreateUpdateCode', Description='Create or Update Code Entity'
                                              where Name='UpdateCode'";

            string renameCreateUpdateAlias = @"update [dbo].[Permissions]
                                              set Name='CreateUpdateAlias', Description='Create or Update Alias Entity'
                                              where Name='UpdateAlias'";

            string renameCreateUpdateAssociation = @"update [dbo].[Permissions]
                                              set Name='CreateUpdateAssociation', Description='Create or Update Association Entity'
                                              where Name='UpdateAssociation'";
            context.Database.ExecuteSqlCommand(renameCreateUpdate);
            context.Database.ExecuteSqlCommand(renameCreateUpdateCode);
            context.Database.ExecuteSqlCommand(renameCreateUpdateAlias);
            context.Database.ExecuteSqlCommand(renameCreateUpdateAssociation);
        }
    }
}
