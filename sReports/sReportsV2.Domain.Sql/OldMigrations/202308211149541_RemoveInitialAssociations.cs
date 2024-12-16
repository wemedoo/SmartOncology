namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveInitialAssociations : DbMigration
    {
        public override void Up()
        {
            using (var context = new SReportsContext())
            {
                CodeMigrationHelper codeMigrationHelper = new CodeMigrationHelper(context);
                codeMigrationHelper.RemoveInitialAssociations();
            }
        }
        
        public override void Down()
        {
        }
    }
}
