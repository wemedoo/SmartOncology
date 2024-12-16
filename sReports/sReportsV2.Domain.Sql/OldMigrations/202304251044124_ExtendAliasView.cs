namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExtendAliasView : DbMigration
    {
        public override void Up()
        {
            string script = @"CREATE OR ALTER VIEW dbo.CodeAliasViews
                                AS 
                                SELECT
                                    inboundAliases.[AliasId],
                                    inboundAliases.[CodeId],
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
                                WHERE GETDATE() BETWEEN inboundAliases.[ActiveFrom] AND inboundAliases.[ActiveTo] AND inboundAliases.[EntityStateCD] != 2003";

            SReportsContext sReportsContext = new SReportsContext();
            sReportsContext.Database.ExecuteSqlCommand(script);
        }
        
        public override void Down()
        {
        }
    }
}
