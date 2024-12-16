namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class RemoveOnlyOnePersonnelAddress : DbMigration
    {
        public override void Up()
        {
            string removeOnlyOnePersonnelAddress = @"
                alter table dbo.Personnel drop constraint if exists [FK_dbo.Users_dbo.Addresses_AddressId];
                drop index if exists dbo.Personnel.IX_AddressId;
                alter table dbo.Personnel drop column if exists AddressId;"
            ;
            Sql(removeOnlyOnePersonnelAddress);
        }

        public override void Down()
        {
            AddColumn("dbo.Personnel", "AddressId", c => c.Int());
            CreateIndex("dbo.Personnel", "AddressId");
            AddForeignKey("dbo.Personnel", "AddressId", "dbo.Addresses", "AddressId");
        }
    }
}
