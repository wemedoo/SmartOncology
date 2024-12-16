namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddManyPatientContactAddresses : DbMigration
    {
        public override void Up()
        {
            string createPatientContactAddressTable = @"
                CREATE TABLE [dbo].[PatientContactAddresses] (
                [Id] [int] NOT NULL IDENTITY,
                [PatientContactId] [int] NOT NULL,
                [City] [nvarchar](100),
                [State] [nvarchar](50),
                [PostalCode] [nvarchar](10),
                [CountryId] [int],
                [Street] [nvarchar](200),
                [StreetNumber] [int],
                [AddressTypeId] [int],
                [Active] [bit] NOT NULL,
                [IsDeleted] [bit] NOT NULL,
                [RowVersion] rowversion NOT NULL,
                [EntryDatetime] [datetime] NOT NULL,
                [LastUpdate] [datetime],
                [CreatedById] [int],
                CONSTRAINT [PK_dbo.PatientContactAddresses] PRIMARY KEY ([Id])
            );
            CREATE INDEX [IX_PatientContactId] ON [dbo].[PatientContactAddresses]([PatientContactId]);
            CREATE INDEX [IX_CountryId] ON [dbo].[PatientContactAddresses]([CountryId]);
            CREATE INDEX [IX_AddressTypeId] ON [dbo].[PatientContactAddresses]([AddressTypeId]);
            CREATE INDEX [IX_CreatedById] ON [dbo].[PatientContactAddresses]([CreatedById]);
            ALTER TABLE [dbo].[PatientContactAddresses] ADD CONSTRAINT [FK_dbo.PatientContactAddresses_dbo.CustomEnums_AddressTypeId] FOREIGN KEY ([AddressTypeId]) REFERENCES [dbo].[CustomEnums] ([CustomEnumId]);
            ALTER TABLE [dbo].[PatientContactAddresses] ADD CONSTRAINT [FK_dbo.PatientContactAddresses_dbo.CustomEnums_CountryId] FOREIGN KEY ([CountryId]) REFERENCES [dbo].[CustomEnums] ([CustomEnumId]);
            ALTER TABLE [dbo].[PatientContactAddresses] ADD CONSTRAINT [FK_dbo.PatientContactAddresses_dbo.Users_CreatedById] FOREIGN KEY ([CreatedById]) REFERENCES [dbo].[Users] ([UserId]);
            ALTER TABLE [dbo].[PatientContactAddresses] ADD CONSTRAINT [FK_dbo.PatientContactAddresses_dbo.PatientContacts_PatientContactId] 
            FOREIGN KEY ([PatientContactId]) REFERENCES [dbo].[PatientContacts] ([ContactId]) ON DELETE CASCADE;
            ";
            Sql(createPatientContactAddressTable);

        }

        public override void Down()
        {
            DropForeignKey("dbo.PatientContactAddresses", "PatientContactId", "dbo.PatientContacts");
            DropForeignKey("dbo.PatientContactAddresses", "CreatedById", "dbo.Users");
            DropForeignKey("dbo.PatientContactAddresses", "CountryId", "dbo.CustomEnums");
            DropForeignKey("dbo.PatientContactAddresses", "AddressTypeId", "dbo.CustomEnums");
            DropIndex("dbo.PatientContactAddresses", new[] { "CreatedById" });
            DropIndex("dbo.PatientContactAddresses", new[] { "AddressTypeId" });
            DropIndex("dbo.PatientContactAddresses", new[] { "CountryId" });
            DropIndex("dbo.PatientContactAddresses", new[] { "PatientContactId" });
            DropTable("dbo.PatientContactAddresses");
        }
    }
}
