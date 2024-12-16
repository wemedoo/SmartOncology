namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveTrailingWhitespaceForPatientIdentifierValue : DbMigration
    {
        public override void Up()
        {
            SReportsContext sReportsContext = new SReportsContext();
            string removeTrailingWhitespaceFromPatientIdentifierValuie = 
                @"update dbo.PatientIdentifiers set IdentifierValue = TRIM(IdentifierValue);";
            sReportsContext.Database.ExecuteSqlCommand(removeTrailingWhitespaceFromPatientIdentifierValuie);
        }
        
        public override void Down()
        {
        }
    }
}
