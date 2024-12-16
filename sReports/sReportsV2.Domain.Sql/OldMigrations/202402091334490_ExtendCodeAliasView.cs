namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExtendCodeAliasView : DbMigration
    {
        public override void Up()
        {
            Sql(@"CREATE OR ALTER VIEW dbo.CodeAliasViews
                AS 
                SELECT
                    inboundAliases.[AliasId],
                    inboundAliases.[CodeId],
					codes.[CodeSetId],
                    inboundAliases.[System],
                    inboundAliases.[Alias] as InboundAlias,
                    outboundAliases.[Alias] as OutboundAlias,
                    inboundAliases.[EntityStateCD],
                    inboundAliases.[RowVersion],
                    inboundAliases.[EntryDatetime],
                    inboundAliases.[LastUpdate],
                    inboundAliases.[ActiveFrom],
                    inboundAliases.[ActiveTo],
                    inboundAliases.[CreatedById],
	                inboundAliases.[AliasId] as InboundAliasId,
                    outboundAliases.[AliasId] as OutboundAliasId
                FROM dbo.[InboundAliases] inboundAliases
                LEFT JOIN dbo.[OutboundAliases] outboundAliases
                    ON outboundAliases.[System] = inboundAliases.[System] 
                    AND outboundAliases.[CodeId] = inboundAliases.[CodeId]  AND outboundAliases.[AliasId] = inboundAliases.[OutboundAliasId]
                    AND GETDATE() between outboundAliases.[ActiveFrom] and outboundAliases.[ActiveTo]
				INNER JOIN dbo.[Codes] codes on codes.CodeId = inboundAliases.CodeId
                WHERE GETDATE() BETWEEN inboundAliases.[ActiveFrom] AND inboundAliases.[ActiveTo]
			;");
        }
        
        public override void Down()
        {
            
        }
    }
}
