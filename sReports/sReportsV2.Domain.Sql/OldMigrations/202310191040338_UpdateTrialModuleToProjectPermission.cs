namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public partial class UpdateTrialModuleToProjectPermission : DbMigration
    {
        public override void Up()
        {
            string sqlCommand = @"
                UPDATE Modules 
                SET Name = 'ProjectManagement', 
                    Description = 'Project Management module has functionalities that are related to viewing, creating and editing Projects, as well as its Personnels and Documentation.' 
                WHERE Name = 'TrialManagement';
            ";

            Sql(sqlCommand);
        }
        
        public override void Down()
        {
        }
    }
}
