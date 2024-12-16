namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.Common.Constants;
    using System.Data.Entity.Migrations;
    
    public partial class MigrateDateTimeInFormCodeRelationTable : DbMigration
    {
        public override void Up()
        {
            string timezone = TimeZoneConstants.CEST;

            DropIndex("dbo.FormCodeRelations", "IX_LastUpdate");
            AlterColumn("dbo.FormCodeRelations", "EntryDatetime", c => c.DateTimeOffset(nullable: false, precision: 7));
            Sql($"UPDATE dbo.FormCodeRelations SET EntryDatetime = (SELECT EntryDatetime AT TIME ZONE '{timezone}')");
            AlterColumn("dbo.FormCodeRelations", "LastUpdate", c => c.DateTimeOffset(precision: 7));
            Sql($"UPDATE dbo.FormCodeRelations SET LastUpdate = (SELECT LastUpdate AT TIME ZONE '{timezone}')");
            AlterColumn("dbo.FormCodeRelations", "ActiveFrom", c => c.DateTimeOffset(nullable: false, precision: 7));
            Sql($"UPDATE dbo.FormCodeRelations SET ActiveFrom = (SELECT ActiveFrom AT TIME ZONE '{timezone}')");
            AlterColumn("dbo.FormCodeRelations", "ActiveTo", c => c.DateTimeOffset(nullable: false, precision: 7));
            Sql($"UPDATE dbo.FormCodeRelations SET ActiveTo = (SELECT ActiveTo AT TIME ZONE '{timezone}')");
            CreateIndex("dbo.FormCodeRelations", "LastUpdate", unique: false, name: "IX_LastUpdate");
        }

        public override void Down() 
        {
            AlterColumn("dbo.FormCodeRelations", "ActiveTo", c => c.DateTime(nullable: false));
            AlterColumn("dbo.FormCodeRelations", "ActiveFrom", c => c.DateTime(nullable: false));
            AlterColumn("dbo.FormCodeRelations", "LastUpdate", c => c.DateTime());
            AlterColumn("dbo.FormCodeRelations", "EntryDatetime", c => c.DateTime(nullable: false));
        }
    }
}
