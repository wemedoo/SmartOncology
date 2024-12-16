namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PopulatePositionPermissionAndPersonnelPosition : DbMigration
    {
        public override void Up()
        {
            SReportsContext dbContext = new SReportsContext();
            string insertPositionPermission = @"
                insert into dbo.PositionPermissions (PositionCD, PermissionModuleId, Active, IsDeleted, EntryDatetime)
                    SELECT code.CodeId
	                ,pM.PermissionModuleId
	                ,1
	                ,0
	                ,GETDATE()
                FROM dbo.Codes code
                inner join dbo.ThesaurusEntryTranslations tranThCode on tranThCode.ThesaurusEntryId = code.ThesaurusEntryId
                inner join dbo.CodeSets cS on code.CodeSetId = cs.CodeSetId
                inner join dbo.ThesaurusEntryTranslations tranThCodeSet on tranThCodeSet.ThesaurusEntryId = cS.ThesaurusEntryId
                inner join dbo.Roles r on r.Name = tranThCode.PreferredTerm
                inner join dbo.PermissionRoles pR on pR.RoleId = r.RoleId
                inner join dbo.PermissionModules pM on pM.ModuleId = pR.ModuleId and pM.PermissionId = pR.PermissionId
                where 
                tranThCode.Language = 'en'
                and 
                tranThCodeSet.Language = 'en'
                and tranThCodeSet.PreferredTerm = 'Role'
                ; 
            ";

            string insertPersonnelPositions = @"
                insert into dbo.PersonnelPositions(PositionCD, PersonnelId, Active, IsDeleted, EntryDatetime)
                SELECT code.CodeId
	                ,uR.UserId
	                ,1
	                ,uR.IsDeleted
	                ,GETDATE()
                FROM dbo.Codes code
                inner join dbo.ThesaurusEntryTranslations tranThCode on tranThCode.ThesaurusEntryId = code.ThesaurusEntryId
                inner join dbo.CodeSets cS on code.CodeSetId = cs.CodeSetId
                inner join dbo.ThesaurusEntryTranslations tranThCodeSet on tranThCodeSet.ThesaurusEntryId = cS.ThesaurusEntryId
                inner join dbo.Roles r on r.Name = tranThCode.PreferredTerm
                inner join dbo.UserRoles uR on uR.RoleId = r.RoleId
                where 
                tranThCode.Language = 'en'
                and 
                tranThCodeSet.Language = 'en'
                and tranThCodeSet.PreferredTerm = 'Role'
                ;  
            ";

            dbContext.Database.ExecuteSqlCommand(insertPositionPermission);
            dbContext.Database.ExecuteSqlCommand(insertPersonnelPositions);
        }

        public override void Down()
        {
        }
    }
}
