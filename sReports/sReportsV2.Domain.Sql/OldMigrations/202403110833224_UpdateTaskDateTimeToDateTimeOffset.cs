namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.Common.Constants;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTaskDateTimeToDateTimeOffset : DbMigration
    {
        public override void Up()
        {
            string timezone = TimeZoneConstants.CEST;
            AlterColumn("dbo.Tasks", "TaskStartDateTime", c => c.DateTimeOffset(nullable: false, precision: 7));
            Sql($"UPDATE dbo.Tasks SET TaskStartDateTime = (SELECT TaskStartDateTime AT TIME ZONE '{timezone}')");
            AlterColumn("dbo.Tasks", "TaskEndDateTime", c => c.DateTimeOffset(precision: 7));
            Sql($"UPDATE dbo.Tasks SET TaskEndDateTime = (SELECT TaskEndDateTime AT TIME ZONE '{timezone}')");
            AlterColumn("dbo.Tasks", "ScheduledDateTime", c => c.DateTimeOffset(precision: 7));
            Sql($"UPDATE dbo.Tasks SET ScheduledDateTime = (SELECT ScheduledDateTime AT TIME ZONE '{timezone}')");
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Tasks", "ScheduledDateTime", c => c.DateTime());
            AlterColumn("dbo.Tasks", "TaskEndDateTime", c => c.DateTime());
            AlterColumn("dbo.Tasks", "TaskStartDateTime", c => c.DateTime(nullable: false));
        }
    }
}
