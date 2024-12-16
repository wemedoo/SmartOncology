namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ClinicalDomainCodeCDColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrganizationClinicalDomains", "ClinicalDomainCD", c => c.Int());
            CreateIndex("dbo.OrganizationClinicalDomains", "ClinicalDomainCD");
            AddForeignKey("dbo.OrganizationClinicalDomains", "ClinicalDomainCD", "dbo.Codes", "CodeId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrganizationClinicalDomains", "ClinicalDomainCD", "dbo.Codes");
            DropIndex("dbo.OrganizationClinicalDomains", new[] { "ClinicalDomainCD" });
            DropColumn("dbo.OrganizationClinicalDomains", "ClinicalDomainCD");
        }
    }
}
