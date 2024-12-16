using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sReportsV2.Domain.Sql.Migrations
{
    /// <inheritdoc />
    public partial class InsertSmartOncologyCodeSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var factory = new SReportsContextFactory();
            using (var context = factory.CreateDbContext(null))
            {
                SReportsContextSeeder.Seed(context);
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
