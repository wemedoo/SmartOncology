namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MigrateCodes : DbMigration
    {
        public override void Up()
        {
            SReportsContext dbContext = new SReportsContext();

            string migrateCitizenship = $@"
                    UPDATE dbo.Patients
                    SET CitizenshipCD = (
                        SELECT CodeId
                        FROM dbo.Codes
                        WHERE ThesaurusEntryId = 9923 AND TypeCD = 0 and CodeSetId=10
                    )
                    WHERE CitizenshipCD = (
                        SELECT CodeId
                        FROM dbo.Codes
                        WHERE ThesaurusEntryId = 9923 AND TypeCD != 0
                    );
                    UPDATE dbo.Patients
                    SET CitizenshipCD = (
                        SELECT CodeId
                        FROM dbo.Codes
                        WHERE ThesaurusEntryId = 9924 AND TypeCD = 0 and CodeSetId=10
                    )
                    WHERE CitizenshipCD = (
                        SELECT CodeId
                        FROM dbo.Codes
                        WHERE ThesaurusEntryId = 9924 AND TypeCD != 0
                    );
                    UPDATE dbo.Patients
                    SET CitizenshipCD = (
                        SELECT CodeId
                        FROM dbo.Codes
                        WHERE ThesaurusEntryId = 9925 AND TypeCD = 0 and CodeSetId=10
                    )
                    WHERE CitizenshipCD = (
                        SELECT CodeId
                        FROM dbo.Codes
                        WHERE ThesaurusEntryId = 9925 AND TypeCD != 0
                    );";

            string migrateAddressType = $@"
                    UPDATE dbo.OrganizationAddresses
                    SET AddressTypeCD = (
                        SELECT CodeId
                        FROM dbo.Codes
                        WHERE ThesaurusEntryId = 9919 AND TypeCD = 0 and CodeSetId=9
                    )
                    WHERE AddressTypeCD = (
                        SELECT CodeId
                        FROM dbo.Codes
                        WHERE ThesaurusEntryId = 9919 AND TypeCD != 0
                    );
                    UPDATE dbo.OrganizationAddresses
                    SET AddressTypeCD = (
                        SELECT CodeId
                        FROM dbo.Codes
                        WHERE ThesaurusEntryId = 9920 AND TypeCD = 0 and CodeSetId=9
                    )
                    WHERE AddressTypeCD = (
                        SELECT CodeId
                        FROM dbo.Codes
                        WHERE ThesaurusEntryId = 9920 AND TypeCD != 0
                    );
                    UPDATE dbo.OrganizationAddresses
                    SET AddressTypeCD = (
                        SELECT CodeId
                        FROM dbo.Codes
                        WHERE ThesaurusEntryId = 9921 AND TypeCD = 0 and CodeSetId=9
                    )
                    WHERE AddressTypeCD = (
                        SELECT CodeId
                        FROM dbo.Codes
                        WHERE ThesaurusEntryId = 9921 AND TypeCD != 0
                    );
                    UPDATE dbo.OrganizationAddresses
                    SET AddressTypeCD = (
                        SELECT CodeId
                        FROM dbo.Codes
                        WHERE ThesaurusEntryId = 9922 AND TypeCD = 0 and CodeSetId=9
                    )
                    WHERE AddressTypeCD = (
                        SELECT CodeId
                        FROM dbo.Codes
                        WHERE ThesaurusEntryId = 9922 AND TypeCD != 0
                    );

                    UPDATE dbo.OutsideUserAddresses
                    SET AddressTypeCD = (
                        SELECT CodeId
                        FROM dbo.Codes
                        WHERE ThesaurusEntryId = 9919 AND TypeCD = 0 and CodeSetId=9
                    )
                    WHERE AddressTypeCD = (
                        SELECT CodeId
                        FROM dbo.Codes
                        WHERE ThesaurusEntryId = 9919 AND TypeCD != 0
                    );
                    UPDATE dbo.OutsideUserAddresses
                    SET AddressTypeCD = (
                        SELECT CodeId
                        FROM dbo.Codes
                        WHERE ThesaurusEntryId = 9920 AND TypeCD = 0 and CodeSetId=9
                    )
                    WHERE AddressTypeCD = (
                        SELECT CodeId
                        FROM dbo.Codes
                        WHERE ThesaurusEntryId = 9920 AND TypeCD != 0
                    );
                    UPDATE dbo.OutsideUserAddresses
                    SET AddressTypeCD = (
                        SELECT CodeId
                        FROM dbo.Codes
                        WHERE ThesaurusEntryId = 9921 AND TypeCD = 0 and CodeSetId=9
                    )
                    WHERE AddressTypeCD = (
                        SELECT CodeId
                        FROM dbo.Codes
                        WHERE ThesaurusEntryId = 9921 AND TypeCD != 0
                    );
                    UPDATE dbo.OutsideUserAddresses
                    SET AddressTypeCD = (
                        SELECT CodeId
                        FROM dbo.Codes
                        WHERE ThesaurusEntryId = 9922 AND TypeCD = 0 and CodeSetId=9
                    )
                    WHERE AddressTypeCD = (
                        SELECT CodeId
                        FROM dbo.Codes
                        WHERE ThesaurusEntryId = 9922 AND TypeCD != 0
                    );



                    UPDATE dbo.PatientAddresses
                    SET AddressTypeCD = (
                        SELECT CodeId
                        FROM dbo.Codes
                        WHERE ThesaurusEntryId = 9919 AND TypeCD = 0 and CodeSetId=9
                    )
                    WHERE AddressTypeCD = (
                        SELECT CodeId
                        FROM dbo.Codes
                        WHERE ThesaurusEntryId = 9919 AND TypeCD != 0
                    ); 
                    UPDATE dbo.PatientAddresses
                    SET AddressTypeCD = (
                        SELECT CodeId
                        FROM dbo.Codes
                        WHERE ThesaurusEntryId = 9920 AND TypeCD = 0 and CodeSetId=9
                    )
                    WHERE AddressTypeCD = (
                        SELECT CodeId
                        FROM dbo.Codes
                        WHERE ThesaurusEntryId = 9920 AND TypeCD != 0
                    );
                    UPDATE dbo.PatientAddresses
                    SET AddressTypeCD = (
                        SELECT CodeId
                        FROM dbo.Codes
                        WHERE ThesaurusEntryId = 9921 AND TypeCD = 0 and CodeSetId=9
                    )
                    WHERE AddressTypeCD = (
                        SELECT CodeId
                        FROM dbo.Codes
                        WHERE ThesaurusEntryId = 9921 AND TypeCD != 0
                    );
                    UPDATE dbo.PatientAddresses
                    SET AddressTypeCD = (
                        SELECT CodeId
                        FROM dbo.Codes
                        WHERE ThesaurusEntryId = 9922 AND TypeCD = 0 and CodeSetId=9
                    )
                    WHERE AddressTypeCD = (
                        SELECT CodeId
                        FROM dbo.Codes
                        WHERE ThesaurusEntryId = 9922 AND TypeCD != 0
                    );



                    UPDATE dbo.PatientContactAddresses
                    SET AddressTypeCD = (
                        SELECT CodeId
                        FROM dbo.Codes
                        WHERE ThesaurusEntryId = 9919 AND TypeCD = 0 and CodeSetId=9
                    )
                    WHERE AddressTypeCD = (
                        SELECT CodeId
                        FROM dbo.Codes
                        WHERE ThesaurusEntryId = 9919 AND TypeCD != 0
                    );
                    UPDATE dbo.PatientContactAddresses
                    SET AddressTypeCD = (
                        SELECT CodeId
                        FROM dbo.Codes
                        WHERE ThesaurusEntryId = 9920 AND TypeCD = 0 and CodeSetId=9
                    )
                    WHERE AddressTypeCD = (
                        SELECT CodeId
                        FROM dbo.Codes
                        WHERE ThesaurusEntryId = 9920 AND TypeCD != 0
                    );
                    UPDATE dbo.PatientContactAddresses
                    SET AddressTypeCD = (
                        SELECT CodeId
                        FROM dbo.Codes
                        WHERE ThesaurusEntryId = 9921 AND TypeCD = 0 and CodeSetId=9
                    )
                    WHERE AddressTypeCD = (
                        SELECT CodeId
                        FROM dbo.Codes
                        WHERE ThesaurusEntryId = 9921 AND TypeCD != 0
                    );
                    UPDATE dbo.PatientContactAddresses
                    SET AddressTypeCD = (
                        SELECT CodeId
                        FROM dbo.Codes
                        WHERE ThesaurusEntryId = 9922 AND TypeCD = 0 and CodeSetId=9
                    )
                    WHERE AddressTypeCD = (
                        SELECT CodeId
                        FROM dbo.Codes
                        WHERE ThesaurusEntryId = 9922 AND TypeCD != 0
                    );



                    UPDATE dbo.PersonnelAddresses
                    SET AddressTypeCD = (
                        SELECT CodeId
                        FROM dbo.Codes
                        WHERE ThesaurusEntryId = 9919 AND TypeCD = 0 and CodeSetId=9
                    )
                    WHERE AddressTypeCD = (
                        SELECT CodeId
                        FROM dbo.Codes
                        WHERE ThesaurusEntryId = 9919 AND TypeCD != 0
                    );
                    UPDATE dbo.PersonnelAddresses
                    SET AddressTypeCD = (
                        SELECT CodeId
                        FROM dbo.Codes
                        WHERE ThesaurusEntryId = 9920 AND TypeCD = 0 and CodeSetId=9
                    )
                    WHERE AddressTypeCD = (
                        SELECT CodeId
                        FROM dbo.Codes
                        WHERE ThesaurusEntryId = 9920 AND TypeCD != 0
                    );
                    UPDATE dbo.PersonnelAddresses
                    SET AddressTypeCD = (
                        SELECT CodeId
                        FROM dbo.Codes
                        WHERE ThesaurusEntryId = 9921 AND TypeCD = 0 and CodeSetId=9
                    )
                    WHERE AddressTypeCD = (
                        SELECT CodeId
                        FROM dbo.Codes
                        WHERE ThesaurusEntryId = 9921 AND TypeCD != 0
                    );
                    UPDATE dbo.PersonnelAddresses
                    SET AddressTypeCD = (
                        SELECT CodeId
                        FROM dbo.Codes
                        WHERE ThesaurusEntryId = 9922 AND TypeCD = 0 and CodeSetId=9
                    )
                    WHERE AddressTypeCD = (
                        SELECT CodeId
                        FROM dbo.Codes
                        WHERE ThesaurusEntryId = 9922 AND TypeCD != 0
                    );";

            dbContext.Database.ExecuteSqlCommand(migrateCitizenship);
            dbContext.Database.ExecuteSqlCommand(migrateAddressType);
        }

        public override void Down()
        {
        }
    }
}
