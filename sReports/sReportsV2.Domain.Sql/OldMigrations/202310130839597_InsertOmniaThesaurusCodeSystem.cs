namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public partial class InsertOmniaThesaurusCodeSystem : DbMigration
    {
        public override void Up()
        {
            using (SReportsContext sReportsContext = new SReportsContext())
            {
                if (sReportsContext.CodeSystems.Any())
                {
                    string value = "Omnia-External-ID";
                    string label = "Omnia External ID";
                    string sab = "OMNIA";

                    string insertOomniaCodeSystem = $@"
                insert into [dbo].[CodeSystems] (Value, Label, SAB) 
                    select '{value}', '{label}', '{sab}'
                    where not exists (
                        SELECT * FROM [dbo].[CodeSystems] where Value = '{value}'
                    );";
                    sReportsContext.Database.ExecuteSqlCommand(insertOomniaCodeSystem);
                }
            }
        }
        
        public override void Down()
        {
            using (SReportsContext sReportsContext = new SReportsContext())
            {
                string removeOomniaCodeSystem = "delete from [dbo].[CodeSystems] where Value = 'Omnia-External-ID';";
                sReportsContext.Database.ExecuteSqlCommand(removeOomniaCodeSystem);
            }
        }
    }
}
