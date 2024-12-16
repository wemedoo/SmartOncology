namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameUserTable : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Users", newName: "Personnel");
            RenameTable(name: "dbo.UsersHistory", newName: "PersonnelHistory");
        }

        public override void Down()
        {
            RenameTable(name: "dbo.PersonnelHistory", newName: "UsersHistory");
            RenameTable(name: "dbo.Personnel", newName: "Users");
        }
    }
}
