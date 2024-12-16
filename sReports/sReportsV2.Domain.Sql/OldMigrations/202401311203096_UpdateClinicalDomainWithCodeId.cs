namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.Common.Enums;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateClinicalDomainWithCodeId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ClinicalDomains", "CodeId", c => c.Int(nullable: false));
            Sql($@"
                UPDATE clinicalDomain
                SET CodeId = (
                    SELECT TOP 1 code.CodeId
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
                            OR tranThCode.PreferredTerm = dbo.InsertSpacesBetweenCapitalLetters(clinicalDomain.Name)
                        )
                )
                FROM dbo.ClinicalDomains clinicalDomain;
            ");
        }
        
        public override void Down()
        {
            DropColumn("dbo.ClinicalDomains", "CodeId");
        }
    }
}
