namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    
    public partial class SaveOrgAndOutsideUserAddresses : DbMigration
    {
        public override void Up()
        {
			Sql(GenerateSqlScript("Organization"));
			Sql(GenerateSqlScript("OutsideUser"));
        }
        
        public override void Down()
        {
        }

        private string GenerateSqlScript(string tableName)
        {
            return $@"
				CREATE TABLE dbo.{tableName}AddressesTemp(
					[{tableName}AddressId][int] IDENTITY(1, 1) NOT NULL,
					[{tableName}Id] [int] NULL,
					[AddressTypeCD] [int] NULL,
					[City] [nvarchar](100) NULL,
					[State] [nvarchar](50) NULL,
					[PostalCode] [nvarchar](10) NULL,
					[Street] [nvarchar](200) NULL,
					[StreetNumber] [int] NULL,
					[CountryCD] [int] NULL,
					[CreatedById] [int] NULL,
					[EntryDatetime] [datetime] NOT NULL,
					[LastUpdate] [datetime] NULL,
					[ActiveFrom] [datetime] NOT NULL,
					[ActiveTo] [datetime] NOT NULL,
					[EntityStateCD] [int] NULL
				);

				insert into dbo.{tableName}AddressesTemp
					(
						{tableName}Id,
						AddressTypeCD,
						City,
						[State],
						PostalCode,
						Street,
						StreetNumber,
						CountryCD,
						CreatedById,
						EntryDatetime,
						LastUpdate,
						ActiveFrom,
						ActiveTo,
						EntityStateCD
						)
					select
						entityTable.{tableName}Id,
						adr.AddressTypeCD,
						adr.City,
						adr.[State],
						adr.PostalCode,
						adr.Street,
						adr.StreetNumber,
						adr.CountryCD,
						adr.CreatedById,
						adr.EntryDatetime,
						adr.LastUpdate,
						adr.ActiveFrom,
						adr.ActiveTo,
						adr.EntityStateCD
					from dbo.Addresses adr
				inner join dbo.{tableName}s entityTable on entityTable.AddressId = adr.AddressId;

				insert into dbo.{tableName}Addresses
					(
						AddressTypeCD,
						City,
						[State],
						PostalCode,
						Street,
						StreetNumber,
						CountryCD,
						CreatedById,
						EntryDatetime,
						LastUpdate,
						ActiveFrom,
						ActiveTo,
						EntityStateCD
					)
					select

							adr.AddressTypeCD,
							adr.City,
							adr.[State],
							adr.PostalCode,
							adr.Street,
							adr.StreetNumber,
							adr.CountryCD,
							adr.CreatedById,
							adr.EntryDatetime,
							adr.LastUpdate,
							adr.ActiveFrom,
							adr.ActiveTo,
							adr.EntityStateCD
						from dbo.{tableName}AddressesTemp adr;

				update entityTable set entityTable.{tableName}AddressId = tempTable.{tableName}AddressId
					from dbo.{tableName}s entityTable
					inner join dbo.{tableName}AddressesTemp tempTable on tempTable.{tableName}Id = entityTable.{tableName}Id;

				drop table if exists dbo.{tableName}AddressesTemp;

				update entityTable set entityTable.AddressId = null
					from dbo.{tableName}s entityTable;
			";
        }
    }
}
