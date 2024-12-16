namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameUserIdColumns : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.PersonnelClinicalTrials", name: "UserId", newName: "PersonnelId");
            RenameColumn(table: "dbo.PersonnelOrganizations", name: "UserId", newName: "PersonnelId");
            RenameColumn(table: "dbo.GlobalThesaurusUserRoles", name: "UserId", newName: "GlobalThesaurusUserId");
            RenameColumn(table: "dbo.GlobalThesaurusUserRoles", name: "RoleId", newName: "GlobalThesaurusRoleId");
            RenameColumn(table: "dbo.Versions", name: "UserId", newName: "PersonnelId");
            RenameColumn(table: "dbo.Comments", name: "UserId", newName: "PersonnelId");
        }
        
        public override void Down()
        {
            RenameColumn(table: "dbo.Comments", name: "PersonnelId", newName: "UserId");
            RenameColumn(table: "dbo.Versions", name: "PersonnelId", newName: "UserId");
            RenameColumn(table: "dbo.GlobalThesaurusUserRoles", name: "GlobalThesaurusRoleId", newName: "RoleId");
            RenameColumn(table: "dbo.GlobalThesaurusUserRoles", name: "GlobalThesaurusUserId", newName: "UserId");
            RenameColumn(table: "dbo.PersonnelOrganizations", name: "PersonnelId", newName: "UserId");
            RenameColumn(table: "dbo.PersonnelClinicalTrials", name: "PersonnelId", newName: "UserId");
        }
    }
}
