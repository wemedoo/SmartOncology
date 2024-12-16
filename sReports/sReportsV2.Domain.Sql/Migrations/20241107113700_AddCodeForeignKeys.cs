using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sReportsV2.Domain.Sql.Migrations
{
    /// <inheritdoc />
    public partial class AddCodeForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TypeCD",
                table: "Versions",
                newName: "Type"
            );

            migrationBuilder.RenameColumn(
                name: "StateCD",
                table: "Versions",
                newName: "State"
            );

            migrationBuilder.RenameColumn(
                name: "StateCD",
                table: "ThesaurusMerges",
                newName: "State"
            );

            migrationBuilder.RenameColumn(
                name: "StateCD",
                table: "ThesaurusEntries",
                newName: "State"
            );

            migrationBuilder.RenameColumn(
                name: "StateCD",
                table: "PersonnelOrganizations",
                newName: "State"
            );

            migrationBuilder.RenameColumn(
               name: "SourceCD",
               table: "GlobalThesaurusUsers",
               newName: "Source"
           );

            migrationBuilder.RenameColumn(
               name: "StatusCD",
               table: "GlobalThesaurusUsers",
               newName: "Status"
           );

            migrationBuilder.RenameColumn(
               name: "CommentStateCD",
               table: "Comments",
               newName: "CommentState"
           );

            migrationBuilder.DropForeignKey(
                name: "FK_Codes_ThesaurusEntries_ThesaurusEntryId",
                table: "Codes");

            migrationBuilder.DropIndex(
                name: "IX_Codes_ThesaurusEntryId",
                table: "Codes");

            migrationBuilder.AddColumn<int>(
                name: "TypeCD",
                table: "Versions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StateCD",
                table: "Versions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StateCD",
                table: "ThesaurusMerges",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StateCD",
                table: "ThesaurusEntries",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StateCD",
                table: "PersonnelOrganizations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SourceCD",
                table: "GlobalThesaurusUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StatusCD",
                table: "GlobalThesaurusUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CommentStateCD",
                table: "Comments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Versions_StateCD",
                table: "Versions",
                column: "StateCD");

            migrationBuilder.CreateIndex(
                name: "IX_Versions_TypeCD",
                table: "Versions",
                column: "TypeCD");

            migrationBuilder.CreateIndex(
                name: "IX_ThesaurusMerges_StateCD",
                table: "ThesaurusMerges",
                column: "StateCD");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelOrganizations_StateCD",
                table: "PersonnelOrganizations",
                column: "StateCD");

            migrationBuilder.CreateIndex(
                name: "IX_GlobalThesaurusUsers_SourceCD",
                table: "GlobalThesaurusUsers",
                column: "SourceCD");

            migrationBuilder.CreateIndex(
                name: "IX_GlobalThesaurusUsers_StatusCD",
                table: "GlobalThesaurusUsers",
                column: "StatusCD");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_CommentStateCD",
                table: "Comments",
                column: "CommentStateCD");

            migrationBuilder.CreateIndex(
                name: "IX_Codes_ThesaurusEntryId",
                table: "Codes",
                column: "ThesaurusEntryId",
                unique: false);

            migrationBuilder.CreateIndex(
               name: "IX_ThesaurusEntries_StateCD",
               table: "ThesaurusEntries",
               column: "StateCD");

            migrationBuilder.AddForeignKey(
                name: "FK_Codes_ThesaurusEntries_ThesaurusEntryId",
                table: "Codes",
                column: "ThesaurusEntryId",
                principalTable: "ThesaurusEntries",
                principalColumn: "ThesaurusEntryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Codes_CommentStateCD",
                table: "Comments",
                column: "CommentStateCD",
                principalTable: "Codes",
                principalColumn: "CodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_GlobalThesaurusUsers_Codes_SourceCD",
                table: "GlobalThesaurusUsers",
                column: "SourceCD",
                principalTable: "Codes",
                principalColumn: "CodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_GlobalThesaurusUsers_Codes_StatusCD",
                table: "GlobalThesaurusUsers",
                column: "StatusCD",
                principalTable: "Codes",
                principalColumn: "CodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_PersonnelOrganizations_Codes_StateCD",
                table: "PersonnelOrganizations",
                column: "StateCD",
                principalTable: "Codes",
                principalColumn: "CodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ThesaurusMerges_Codes_StateCD",
                table: "ThesaurusMerges",
                column: "StateCD",
                principalTable: "Codes",
                principalColumn: "CodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Versions_Codes_StateCD",
                table: "Versions",
                column: "StateCD",
                principalTable: "Codes",
                principalColumn: "CodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Versions_Codes_TypeCD",
                table: "Versions",
                column: "TypeCD",
                principalTable: "Codes",
                principalColumn: "CodeId");

            migrationBuilder.AddForeignKey(
               name: "FK_ThesaurusEntries_Codes_StateCD",
               table: "ThesaurusEntries",
               column: "StateCD",
               principalTable: "Codes",
               principalColumn: "CodeId",
               onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
               name: "Type",
               table: "Versions",
               newName: "TypeCD"
           );

            migrationBuilder.RenameColumn(
                name: "State",
                table: "Versions",
                newName: "StateCD"
            );

            migrationBuilder.RenameColumn(
                name: "State",
                table: "ThesaurusMerges",
                newName: "StateCD"
            );

            migrationBuilder.RenameColumn(
                name: "State",
                table: "ThesaurusEntries",
                newName: "StateCD"
            );

            migrationBuilder.RenameColumn(
                name: "State",
                table: "PersonnelOrganizations",
                newName: "StateCD"
            );

            migrationBuilder.RenameColumn(
                name: "Source",
                table: "GlobalThesaurusUsers",
                newName: "SourceCD"
            );

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "GlobalThesaurusUsers",
                newName: "StatusCD"
            );

            migrationBuilder.RenameColumn(
                name: "CommentState",
                table: "Comments",
                newName: "CommentStateCD"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_Codes_ThesaurusEntries_ThesaurusEntryId",
                table: "Codes");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Codes_CommentStateCD",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_GlobalThesaurusUsers_Codes_SourceCD",
                table: "GlobalThesaurusUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_GlobalThesaurusUsers_Codes_StatusCD",
                table: "GlobalThesaurusUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonnelOrganizations_Codes_StateCD",
                table: "PersonnelOrganizations");

            migrationBuilder.DropForeignKey(
                name: "FK_ThesaurusMerges_Codes_StateCD",
                table: "ThesaurusMerges");

            migrationBuilder.DropForeignKey(
                name: "FK_Versions_Codes_StateCD",
                table: "Versions");

            migrationBuilder.DropForeignKey(
                name: "FK_Versions_Codes_TypeCD",
                table: "Versions");

            migrationBuilder.DropForeignKey(
                name: "FK_ThesaurusEntries_Codes_StateCD",
                table: "ThesaurusEntries");

            migrationBuilder.DropIndex(
                name: "IX_Versions_StateCD",
                table: "Versions");

            migrationBuilder.DropIndex(
                name: "IX_Versions_TypeCD",
                table: "Versions");

            migrationBuilder.DropIndex(
                name: "IX_ThesaurusMerges_StateCD",
                table: "ThesaurusMerges");

            migrationBuilder.DropIndex(
                name: "IX_PersonnelOrganizations_StateCD",
                table: "PersonnelOrganizations");

            migrationBuilder.DropIndex(
                name: "IX_GlobalThesaurusUsers_SourceCD",
                table: "GlobalThesaurusUsers");

            migrationBuilder.DropIndex(
                name: "IX_GlobalThesaurusUsers_StatusCD",
                table: "GlobalThesaurusUsers");

            migrationBuilder.DropIndex(
                name: "IX_Comments_CommentStateCD",
                table: "Comments");

            migrationBuilder.DropIndex(
              name: "IX_ThesaurusEntries_StateCD",
              table: "ThesaurusEntries");

            migrationBuilder.DropColumn(
                name: "TypeCD",
                table: "Versions");

            migrationBuilder.DropColumn(
                name: "StateCD",
                table: "Versions");

            migrationBuilder.DropColumn(
                name: "StateCD",
                table: "ThesaurusMerges");

            migrationBuilder.DropColumn(
                name: "StateCD",
                table: "ThesaurusEntries");

            migrationBuilder.DropColumn(
                name: "StateCD",
                table: "PersonnelOrganizations");

            migrationBuilder.DropColumn(
                name: "SourceCD",
                table: "GlobalThesaurusUsers");

            migrationBuilder.DropColumn(
                name: "StatusCD",
                table: "GlobalThesaurusUsers");

            migrationBuilder.DropColumn(
                name: "CommentStateCD",
                table: "Comments");

            migrationBuilder.CreateIndex(
                name: "IX_Codes_ThesaurusEntryId",
                table: "Codes",
                column: "ThesaurusEntryId",
                unique: true);
        }
    }
}
