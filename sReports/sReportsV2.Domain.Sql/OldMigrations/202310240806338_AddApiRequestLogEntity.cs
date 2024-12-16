namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddApiRequestLogEntity : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ApiRequestLogs",
                c => new
                    {
                        ApiRequestLogId = c.Int(nullable: false, identity: true),
                        ApiRequestDirection = c.Int(nullable: false),
                        RequestTimestamp = c.DateTime(nullable: false),
                        RequestPayload = c.String(),
                        RequestUriAbsolutePath = c.String(),
                        ResponseTimestamp = c.DateTime(),
                        ResponsePayload = c.String(),
                        HttpStatusCode = c.Short(),
                        ApiName = c.String(),
                    })
                .PrimaryKey(t => t.ApiRequestLogId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ApiRequestLogs");
        }
    }
}
