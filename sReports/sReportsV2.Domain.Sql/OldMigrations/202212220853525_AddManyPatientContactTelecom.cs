namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddManyPatientContactTelecom : DbMigration
    {
        public override void Up()
        {
            string createPatientContactTelecomTable = @"
                CREATE TABLE [dbo].[PatientContactTelecoms] (
                [Id] [int] NOT NULL IDENTITY,
                [PatientContactId] [int] NOT NULL,
                [System] [nvarchar](max),
                [Value] [nvarchar](max),
                [Use] [nvarchar](max),
                [Active] [bit] NOT NULL,
                [IsDeleted] [bit] NOT NULL,
                [RowVersion] rowversion NOT NULL,
                [EntryDatetime] [datetime] NOT NULL,
                [LastUpdate] [datetime],
                [CreatedById] [int],
                CONSTRAINT [PK_dbo.PatientContactTelecoms] PRIMARY KEY ([Id])
            );
            CREATE INDEX [IX_PatientContactId] ON [dbo].[PatientContactTelecoms]([PatientContactId]);
            CREATE INDEX [IX_CreatedById] ON [dbo].[PatientContactTelecoms]([CreatedById]);
            ALTER TABLE [dbo].[PatientContactTelecoms] ADD CONSTRAINT [FK_dbo.PatientContactTelecoms_dbo.Users_CreatedById] FOREIGN KEY ([CreatedById]) REFERENCES [dbo].[Users] ([UserId]);
            ALTER TABLE [dbo].[PatientContactTelecoms] ADD CONSTRAINT [FK_dbo.PatientContactTelecoms_dbo.PatientContacts_PatientContactId] FOREIGN KEY ([PatientContactId]) REFERENCES [dbo].[PatientContacts] ([ContactId]) ON DELETE CASCADE;
            ";
            Sql(createPatientContactTelecomTable);

        }

        public override void Down()
        {
            DropForeignKey("dbo.PatientContactTelecoms", "PatientContactId", "dbo.PatientContacts");
            DropForeignKey("dbo.PatientContactTelecoms", "CreatedById", "dbo.Users");
            DropIndex("dbo.PatientContactTelecoms", new[] { "CreatedById" });
            DropIndex("dbo.PatientContactTelecoms", new[] { "PatientContactId" });
            DropTable("dbo.PatientContactTelecoms");
        }
    }
}
