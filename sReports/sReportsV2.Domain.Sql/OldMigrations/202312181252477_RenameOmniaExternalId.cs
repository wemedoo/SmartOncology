namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.Common.Constants;
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Security.AccessControl;

    public partial class RenameOmniaExternalId : DbMigration
    {
        public override void Up()
        {
            using (SReportsContext sReportsContext = new SReportsContext())
            {
                if (sReportsContext.CodeSystems.Any())
                {
                    string oldLabel = "Omnia External ID";
                    string newValue = "Oomnia-External-ID";
                    string newLabel = ResourceTypes.OomniaExternalId;
                    string newSAB = "OOMNIA";

                    string updateOomniaThesaurusCodeSystem = $@"
                    update dbo.CodeSystems set Value = '{newValue}'
                        , Label = '{newLabel}'
                        , SAB = '{newSAB}' 
                        where Label = '{oldLabel}';";
                    string updateOomniaThesaurusPrefTerm = $@"
                    update dbo.ThesaurusEntryTranslations set PreferredTerm = '{newLabel}' 
                        where PreferredTerm = '{oldLabel}';";
                    sReportsContext.Database.ExecuteSqlCommand(updateOomniaThesaurusCodeSystem);
                    sReportsContext.Database.ExecuteSqlCommand(updateOomniaThesaurusPrefTerm);
                }
            }
        }

        public override void Down()
        {
            using (SReportsContext sReportsContext = new SReportsContext())
            {
                string newLabel = ResourceTypes.OomniaExternalId;
                string oldValue = "Omnia-External-ID";
                string oldLabel = "Omnia External ID";
                string oldSAB = "OMNIA";

                string updateOomniaThesaurusCodeSystem = $@"
                    update dbo.CodeSystems set Value = '{oldValue}'
                        , Label = '{oldLabel}'
                        , SAB = '{oldSAB}' 
                        where Label = '{newLabel}';";
                string updateOomniaThesaurusPrefTerm = $@"
                    update dbo.ThesaurusEntryTranslations set PreferredTerm = '{oldLabel}' 
                        where PreferredTerm = '{newLabel}';";
                sReportsContext.Database.ExecuteSqlCommand(updateOomniaThesaurusCodeSystem);
                sReportsContext.Database.ExecuteSqlCommand(updateOomniaThesaurusPrefTerm);
            }
        }
    }
}
