namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SaveExistingAcademicPositionData : DbMigration
    {
        public override void Up()
        {
            SReportsContext context = new SReportsContext();
            string updateAcademicPositionTypesTable = @"
                update dbo.AcademicPositionTypes set [Name] = case [Name]
						when 'AssistantProfesssor' then 'Assistant professsor'
						when 'DoctorOfPhilosophy' then 'Doctor of philosophy'
						when 'Privatdozent' then 'Privatdozent'
                        else 'Professor'
						end;
            ";
            string insertIntoPersonnelAcademicPosition = @"
                insert into dbo.PersonnelAcademicPositions
                      (PersonnelId
	                  ,
                      Active,
                      IsDeleted,
                      EntryDatetime,
                      LastUpdate,
                      CreatedById,
	                  AcademicPositionCD
	                  )
                select pos.UserId,
                      pos.Active,
                      pos.IsDeleted,
                      pos.EntryDatetime,
                      pos.LastUpdate,
                      pos.CreatedById,
	                  code.CodeId
                  FROM dbo.UserAcademicPositions pos
                  inner join dbo.AcademicPositionTypes t on t.AcademicPositionTypeId = pos.AcademicPositionTypeId
                  inner join dbo.ThesaurusEntryTranslations tranThCode on tranThCode.PreferredTerm = t.[Name]
                  inner join dbo.Codes code on code.ThesaurusEntryId = tranThCode.ThesaurusEntryId
                  inner join dbo.CodeSets cS on code.CodeSetId = cs.CodeSetId
                  inner join dbo.ThesaurusEntryTranslations tranThCodeSet on tranThCodeSet.ThesaurusEntryId = cS.ThesaurusEntryId
                  where 
	                tranThCode.Language = 'en' and 
	                tranThCodeSet.Language = 'en'
	                and tranThCodeSet.PreferredTerm = 'Academic position'
                  ;
            ";

            context.Database.ExecuteSqlCommand(updateAcademicPositionTypesTable);
            context.Database.ExecuteSqlCommand(insertIntoPersonnelAcademicPosition);
        }

        public override void Down()
        {
        }
    }
}
