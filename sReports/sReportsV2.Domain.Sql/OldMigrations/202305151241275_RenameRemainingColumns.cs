namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameRemainingColumns : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.EpisodeOfCareWorkflows", name: "User", newName: "PersonnelId");
            RenameColumn(table: "dbo.Personnel", name: "UserConfigId", newName: "PersonnelConfigId");
            RenameIndex(table: "dbo.Personnel", name: "IX_UserConfigId", newName: "IX_PersonnelConfigId");
            RenameIndex(table: "dbo.GlobalThesaurusUserRoles", name: "IX_UserId", newName: "IX_GlobalThesaurusUserId");
            RenameIndex(table: "dbo.GlobalThesaurusUserRoles", name: "IX_RoleId", newName: "IX_GlobalThesaurusRoleId");
            RenameIndex(table: "dbo.PersonnelClinicalTrials", name: "IX_UserId", newName: "IX_PersonnelId");
            RenameIndex(table: "dbo.PersonnelOrganizations", name: "IX_UserId", newName: "IX_PersonnelId");
        }

        public override void Down()
        {
            RenameIndex(table: "dbo.PersonnelOrganizations", name: "IX_PersonnelId", newName: "IX_UserId");
            RenameIndex(table: "dbo.PersonnelClinicalTrials", name: "IX_PersonnelId", newName: "IX_UserId");
            RenameIndex(table: "dbo.GlobalThesaurusUserRoles", name: "IX_GlobalThesaurusRoleId", newName: "IX_RoleId");
            RenameIndex(table: "dbo.GlobalThesaurusUserRoles", name: "IX_GlobalThesaurusUserId", newName: "IX_UserId");
            RenameIndex(table: "dbo.Personnel", name: "IX_PersonnelConfigId", newName: "IX_UserConfigId");
            RenameColumn(table: "dbo.Personnel", name: "PersonnelConfigId", newName: "UserConfigId");
            RenameColumn(table: "dbo.EpisodeOfCareWorkflows", name: "PersonnelId", newName: "User");
        }
    }
}
