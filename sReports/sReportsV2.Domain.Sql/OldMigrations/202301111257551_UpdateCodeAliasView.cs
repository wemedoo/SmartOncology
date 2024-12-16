namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateCodeAliasView : DbMigration
    {
		public override void Up()
		{
			string script =
				@"CREATE OR ALTER VIEW dbo.CodeAliasViews
					as select
							inboundAliases.[AliasId]
							,inboundAliases.[CodeId]
							,inboundAliases.[System]
							,inboundAliases.[Alias] as InboundAlias
							,outboundAliases.[Alias] as OutboundAlias
							,inboundAliases.[ValidFrom]
							,inboundAliases.[ValidTo]
							,inboundAliases.[IsDeleted]
							,inboundAliases.[RowVersion]
							,inboundAliases.[EntryDatetime]
							,inboundAliases.[LastUpdate]
							,inboundAliases.[Active]
							,inboundAliases.[CreatedById]
					from dbo.[InboundAliases] inboundAliases
					inner join dbo.[OutboundAliases] outboundAliases
					on outboundAliases.[System] = inboundAliases.[System]
					where GETDATE() between inboundAliases.[ValidFrom] and inboundAliases.[ValidTo]
				";

			SReportsContext sReportsContext = new SReportsContext();
			sReportsContext.Database.ExecuteSqlCommand(script);

		}

		public override void Down()
		{
			DropForeignKey("dbo.CodeAliasViews", "CreatedById", "dbo.Users");
			DropIndex("dbo.CodeAliasViews", new[] { "CreatedById" });
			DropTable("dbo.CodeAliasViews");
		}
	}
}
