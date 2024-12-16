namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameCustomEnumsToCodes : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.CustomEnums", newName: "Codes");
            RenameTable(name: "dbo.CustomEnumsHistory", newName: "CodesHistory");
        }

        public override void Down()
        {
            RenameTable(name: "dbo.Codes", newName: "CustomEnums");
            RenameTable(name: "dbo.CodesHistory", newName: "CustomEnumsHistory");
        }
    }
}
