namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CodedFieldStandardCodeset : DbMigration
    {
        public override void Up()
        {
            int CodeSetId = 88; // CodedField Codeset
            Sql($"DELETE FROM dbo.Codes WHERE CodeSetId = {CodeSetId}"); // Deletes Codes and associated CodeAssociations

            Sql($"DELETE FROM dbo.CodeSets WHERE CodeSetId = {CodeSetId}");
        }
        
        public override void Down()
        {
        }
    }
}
