namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SetOrganizationTimeZone : DbMigration
    {
        public override void Up()
        {
            string updateTimeZone =
                @"UPDATE [dbo].[Organizations]
                    SET TimeZone = CASE
                        WHEN (SELECT City FROM [dbo].[OrganizationAddresses] addr WHERE addr.OrganizationAddressId = [dbo].[Organizations].OrganizationAddressId) = 'Sydney' THEN '(UTC+10:00) Canberra, Melbourne, Sydney'
                        WHEN (SELECT City FROM [dbo].[OrganizationAddresses] addr WHERE addr.OrganizationAddressId = [dbo].[Organizations].OrganizationAddressId) = 'Novi Sad' THEN '(UTC+01:00) Sarajevo, Skopje, Warsaw, Zagreb'
                        WHEN (SELECT City FROM [dbo].[OrganizationAddresses] addr WHERE addr.OrganizationAddressId = [dbo].[Organizations].OrganizationAddressId) = 'London' THEN '(UTC+01:00) Sarajevo, Skopje, Warsaw, Zagreb'
                        WHEN (SELECT City FROM [dbo].[OrganizationAddresses] addr WHERE addr.OrganizationAddressId = [dbo].[Organizations].OrganizationAddressId) = 'Santa Barbara' THEN '(UTC-07:00) Arizona'
                        WHEN (SELECT City FROM [dbo].[OrganizationAddresses] addr WHERE addr.OrganizationAddressId = [dbo].[Organizations].OrganizationAddressId) = 'Mumbai' THEN '(UTC+05:30) Chennai, Kolkata, Mumbai, New Delhi'
                        WHEN (SELECT City FROM [dbo].[OrganizationAddresses] addr WHERE addr.OrganizationAddressId = [dbo].[Organizations].OrganizationAddressId) = 'Ahmedabad' THEN '(UTC+05:30) Chennai, Kolkata, Mumbai, New Delhi'

                        ELSE '(UTC+01:00) Amsterdam, Berlin, Bern, Rome, Stockholm, Vienna'
                    END;";

            string updateTimeZoneOffset =
                @"UPDATE [dbo].[Organizations]
                    SET TimeZoneOffset = CASE
                        WHEN (SELECT City FROM [dbo].[OrganizationAddresses] addr WHERE addr.OrganizationAddressId = [dbo].[Organizations].OrganizationAddressId) = 'Sydney' THEN '+10:00'
                        WHEN (SELECT City FROM [dbo].[OrganizationAddresses] addr WHERE addr.OrganizationAddressId = [dbo].[Organizations].OrganizationAddressId) = 'Novi Sad' THEN '+01:00'
                        WHEN (SELECT City FROM [dbo].[OrganizationAddresses] addr WHERE addr.OrganizationAddressId = [dbo].[Organizations].OrganizationAddressId) = 'London' THEN '+01:00'
                        WHEN (SELECT City FROM [dbo].[OrganizationAddresses] addr WHERE addr.OrganizationAddressId = [dbo].[Organizations].OrganizationAddressId) = 'Santa Barbara' THEN '-07:00'
                        WHEN (SELECT City FROM [dbo].[OrganizationAddresses] addr WHERE addr.OrganizationAddressId = [dbo].[Organizations].OrganizationAddressId) = 'Mumbai' THEN '+05:30'
                        WHEN (SELECT City FROM [dbo].[OrganizationAddresses] addr WHERE addr.OrganizationAddressId = [dbo].[Organizations].OrganizationAddressId) = 'Ahmedabad' THEN '+05:30'

                        ELSE '+01:00'
                    END;";

            using (var context = new SReportsContext())
            {
                context.Database.ExecuteSqlCommand(updateTimeZone);
                context.Database.ExecuteSqlCommand(updateTimeZoneOffset);
            }
        }
        
        public override void Down()
        {
        }
    }
}
