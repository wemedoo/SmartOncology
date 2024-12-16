namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveClinicalDomainIdFromOrgClinicalTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.OrganizationClinicalDomains", "ClinicalDomainId", "dbo.ClinicalDomains");
            DropIndex("dbo.OrganizationClinicalDomains", new[] { "ClinicalDomainId" });
            DropColumn("dbo.OrganizationClinicalDomains", "ClinicalDomainId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.OrganizationClinicalDomains", "ClinicalDomainId", c => c.Int(nullable: false));
            CreateIndex("dbo.OrganizationClinicalDomains", "ClinicalDomainId");
            AddForeignKey("dbo.OrganizationClinicalDomains", "ClinicalDomainId", "dbo.ClinicalDomains", "ClinicalDomainId", cascadeDelete: true);
        }
    }
}
