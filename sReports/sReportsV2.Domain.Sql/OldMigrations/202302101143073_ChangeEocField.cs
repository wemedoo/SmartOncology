namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeEocField : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.EpisodeOfCares", name: "Type", newName: "TypeCD");
        }
        
        public override void Down()
        {
            RenameColumn(table: "dbo.EpisodeOfCares", name: "TypeCD", newName: "Type");
        }
    }
}
