namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.Common.Constants;
    using System;
    using System.Configuration;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeDateTimeToDateTimeOffset : DbMigration
    {
        public override void Up()
        {
            string timezone = TimeZoneConstants.CEST;

            AlterColumn("dbo.Versions", "CreatedOn", c => c.DateTimeOffset(nullable: false, precision: 7));
            Sql($"UPDATE dbo.Versions SET CreatedOn = (SELECT CreatedOn AT TIME ZONE '{timezone}')");
            AlterColumn("dbo.Versions", "RevokedOn", c => c.DateTimeOffset(precision: 7));
            Sql($"UPDATE dbo.Versions SET RevokedOn = (SELECT RevokedOn AT TIME ZONE '{timezone}')");

            AlterColumn("dbo.ApiRequestLogs", "RequestTimestamp", c => c.DateTimeOffset(nullable: false, precision: 7));
            Sql($"UPDATE dbo.ApiRequestLogs SET RequestTimestamp = (SELECT RequestTimestamp AT TIME ZONE '{timezone}')");
            AlterColumn("dbo.ApiRequestLogs", "ResponseTimestamp", c => c.DateTimeOffset(precision: 7));
            Sql($"UPDATE dbo.ApiRequestLogs SET ResponseTimestamp = (SELECT ResponseTimestamp AT TIME ZONE '{timezone}')");

            AlterColumn("dbo.O4CodeableConcepts", "EntryDateTime", c => c.DateTimeOffset(precision: 7));
            Sql($"UPDATE dbo.O4CodeableConcepts SET EntryDateTime = (SELECT EntryDateTime AT TIME ZONE '{timezone}')");

            AlterColumn("dbo.Projects", "ProjectStartDateTime", c => c.DateTimeOffset(precision: 7));
            Sql($"UPDATE dbo.Projects SET ProjectStartDateTime = (SELECT ProjectStartDateTime AT TIME ZONE '{timezone}')");
            AlterColumn("dbo.Projects", "ProjectEndDateTime", c => c.DateTimeOffset(precision: 7));
            Sql($"UPDATE dbo.Projects SET ProjectEndDateTime = (SELECT ProjectEndDateTime AT TIME ZONE '{timezone}')");

            AlterColumn("dbo.EpisodeOfCareWorkflows", "Submited", c => c.DateTimeOffset(nullable: false, precision: 7));
            Sql($"UPDATE dbo.EpisodeOfCareWorkflows SET Submited = (SELECT Submited AT TIME ZONE '{timezone}')");

            AlterColumn("dbo.ErrorMessageLogs", "TransactionDatetime", c => c.DateTimeOffset(precision: 7));
            Sql($"UPDATE dbo.ErrorMessageLogs SET TransactionDatetime = (SELECT TransactionDatetime AT TIME ZONE '{timezone}')");

            AlterColumn("dbo.Transactions", "TransactionDatetime", c => c.DateTimeOffset(nullable: false, precision: 7));
            Sql($"UPDATE dbo.Transactions SET TransactionDatetime = (SELECT TransactionDatetime AT TIME ZONE '{timezone}')");
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Transactions", "TransactionDatetime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.ErrorMessageLogs", "TransactionDatetime", c => c.DateTime());
            AlterColumn("dbo.EpisodeOfCareWorkflows", "Submited", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Projects", "ProjectEndDateTime", c => c.DateTime());
            AlterColumn("dbo.Projects", "ProjectStartDateTime", c => c.DateTime());
            AlterColumn("dbo.O4CodeableConcepts", "EntryDateTime", c => c.DateTime());
            AlterColumn("dbo.ApiRequestLogs", "ResponseTimestamp", c => c.DateTime());
            AlterColumn("dbo.ApiRequestLogs", "RequestTimestamp", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Versions", "RevokedOn", c => c.DateTime());
            AlterColumn("dbo.Versions", "CreatedOn", c => c.DateTime(nullable: false));
        }
    }
}
