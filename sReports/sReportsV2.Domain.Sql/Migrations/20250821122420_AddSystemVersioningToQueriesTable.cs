using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sReportsV2.Domain.Sql.Migrations
{
    /// <inheritdoc />
    public partial class AddSystemVersioningToQueriesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            MigrationHelper.AddSystemVersioningToTables(migrationBuilder, "dbo.Queries");
            MigrationHelper.CreateIndexesOnCommonProperties(migrationBuilder, "dbo.Queries");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            MigrationHelper.DropIndexesOnCommonProperties(migrationBuilder, "dbo.Queries");
            MigrationHelper.UnsetSystemVersionedTables(migrationBuilder, "dbo.Queries");
        }
    }
}
