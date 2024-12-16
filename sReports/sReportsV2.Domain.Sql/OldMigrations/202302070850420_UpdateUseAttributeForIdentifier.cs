namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;

    public partial class UpdateUseAttributeForIdentifier : DbMigration
    {
        public override void Up()
        {
            SReportsContext context = new SReportsContext();
            string script = @"
                update Identifiers set [Use] = (select code.CodeId from  dbo.ThesaurusEntryTranslations tranThCode
                inner join dbo.Codes code on tranThCode.ThesaurusEntryId = code.ThesaurusEntryId
                inner join dbo.CodeSets cS on code.CodeSetId = cs.CodeSetId
                inner join dbo.ThesaurusEntryTranslations tranThCodeSet on tranThCodeSet.ThesaurusEntryId = cS.ThesaurusEntryId
                where 
                tranThCode.Language = 'en'
                and 
                tranThCodeSet.Language = 'en'
                and tranThCodeSet.PreferredTerm = 'Identifier use Type'
				and tranThCode.PreferredTerm = [Use]
				) 
                ; 
            ";
            context.Database.ExecuteSqlCommand(script);
        }

        public override void Down()
        {
            SReportsContext context = new SReportsContext();
            string script = @"
                update i set i.[Use] = tranThCode.PreferredTerm
                from dbo.Identifiers i  
                inner join dbo.Codes code on i.[Use] = code.CodeId
                inner join dbo.ThesaurusEntryTranslations tranThCode on tranThCode.ThesaurusEntryId = code.ThesaurusEntryId
                inner join dbo.CodeSets cS on code.CodeSetId = cs.CodeSetId
                inner join dbo.ThesaurusEntryTranslations tranThCodeSet on tranThCodeSet.ThesaurusEntryId = cS.ThesaurusEntryId
                where 
                tranThCode.Language = 'en'
                and 
                tranThCodeSet.Language = 'en'
                and tranThCodeSet.PreferredTerm = 'Identifier use Type'
                ; 
            ";
            context.Database.ExecuteSqlCommand(script);
        }
    }
}