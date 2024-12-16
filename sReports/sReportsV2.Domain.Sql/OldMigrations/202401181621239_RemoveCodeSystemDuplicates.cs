namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveCodeSystemDuplicates : DbMigration
    {
        public override void Up()
        {
            Sql(@"
                declare @CodeSystemMappings table (IdToSet int, IdToRemove int);
                insert into @CodeSystemMappings
                select codeSystem1.CodeSystemId, codeSystem2.CodeSystemId
                  FROM [dbo].[CodeSystems] codeSystem1, [dbo].[CodeSystems] codeSystem2 
                  where codeSystem1.Label = codeSystem2.Label 
                  and codeSystem1.CodeSystemId != codeSystem2.CodeSystemId 
                  and codeSystem2.SAB is null 
                  and codeSystem1.SAB is not null
                  ;

                update code set code.CodeSystemId = mappings.IdToSet
	                from dbo.CodeSystems cs 
	                inner join dbo.O4CodeableConcepts code on code.CodeSystemId = cs.CodeSystemId
	                inner join @CodeSystemMappings mappings on cS.CodeSystemId = mappings.IdToRemove
	                where code.CodeSystemId in (select IdToRemove from @CodeSystemMappings);

                delete from dbo.CodeSystems where CodeSystemId in (select IdToRemove from @CodeSystemMappings);
            ");
        }
        
        public override void Down()
        {
        }
    }
}
