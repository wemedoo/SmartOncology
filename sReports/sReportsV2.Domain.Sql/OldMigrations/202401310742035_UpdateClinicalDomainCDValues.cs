namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.Common.Enums;
    using System;
    using System.Data.Entity.Migrations;

    public partial class UpdateClinicalDomainCDValues : DbMigration
    {
        public override void Up()
        {
            // Add the stored procedure for removing spaces in ClinicalDomain names, in existing enum from db
            Sql(@"
                CREATE FUNCTION dbo.InsertSpacesBetweenCapitalLetters(@inputString NVARCHAR(MAX))
                RETURNS NVARCHAR(MAX)
                AS
                BEGIN
                    DECLARE @outputString NVARCHAR(MAX) = '';
                    DECLARE @len INT = LEN(@inputString);
                    DECLARE @i INT = 1;

                    WHILE @i <= @len
                    BEGIN
                        IF UNICODE(SUBSTRING(@inputString, @i, 1)) BETWEEN UNICODE('A') AND UNICODE('Z')
                            AND @i > 1
                            AND UNICODE(SUBSTRING(@inputString, @i - 1, 1)) BETWEEN UNICODE('a') AND UNICODE('z')
                        BEGIN
                            SET @outputString += ' ';
                        END

                        SET @outputString += SUBSTRING(@inputString, @i, 1);
                        SET @i += 1;
                    END

                    RETURN @outputString;
                END;
            ");

            Sql($@"
                UPDATE dbo.[OrganizationClinicalDomains]
                SET [ClinicalDomainCD] = (
                    SELECT TOP 1 CodeId
                    FROM dbo.Codes code 
                    INNER JOIN dbo.ThesaurusEntryTranslations tranThCode ON tranThCode.ThesaurusEntryId = code.ThesaurusEntryId
                    WHERE 
                        code.CodeSetId = {(int)CodeSetList.ClinicalDomain} AND 
                        (
                            (clinicalDomain.Name = 'WomensHealth' AND tranThCode.PreferredTerm = 'Women''s Health')
                            OR (clinicalDomain.Name = 'Anaestetics' AND tranThCode.PreferredTerm = 'Anaesthetics')
                            OR (clinicalDomain.Name = 'ChildPsyhiatry' AND tranThCode.PreferredTerm = 'Child Psychiatry')
                            OR (clinicalDomain.Name = 'NeurologyWSpecialQualificationsInChildNeuro' AND tranThCode.PreferredTerm = 'Neurology W Special Qualifications In Child Neuro')
                            OR tranThCode.PreferredTerm = dbo.InsertSpacesBetweenCapitalLetters(clinicalDomain.Name)
                        )
                )
                FROM dbo.ClinicalDomains clinicalDomain
                WHERE clinicalDomain.ClinicalDomainId = [OrganizationClinicalDomains].[ClinicalDomainId];
            ");
        }

        public override void Down()
        {
        }
    }
}
