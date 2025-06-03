using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToolBox_MVC.Migrations.ToolBoxDb
{
    /// <inheritdoc />
    public partial class testLinkADMF : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ADGroupGUID",
                table: "MFilesGroups",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ADAccountGUID",
                table: "MFilesAccounts",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MFilesGroups_ADGroupGUID",
                table: "MFilesGroups",
                column: "ADGroupGUID");

            migrationBuilder.CreateIndex(
                name: "IX_MFilesAccounts_ADAccountGUID",
                table: "MFilesAccounts",
                column: "ADAccountGUID");

            migrationBuilder.AddForeignKey(
                name: "FK_MFilesAccounts_ADAccounts_ADAccountGUID",
                table: "MFilesAccounts",
                column: "ADAccountGUID",
                principalTable: "ADAccounts",
                principalColumn: "GUID");

            migrationBuilder.AddForeignKey(
                name: "FK_MFilesGroups_ADGroups_ADGroupGUID",
                table: "MFilesGroups",
                column: "ADGroupGUID",
                principalTable: "ADGroups",
                principalColumn: "GUID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MFilesAccounts_ADAccounts_ADAccountGUID",
                table: "MFilesAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_MFilesGroups_ADGroups_ADGroupGUID",
                table: "MFilesGroups");

            migrationBuilder.DropIndex(
                name: "IX_MFilesGroups_ADGroupGUID",
                table: "MFilesGroups");

            migrationBuilder.DropIndex(
                name: "IX_MFilesAccounts_ADAccountGUID",
                table: "MFilesAccounts");

            migrationBuilder.DropColumn(
                name: "ADGroupGUID",
                table: "MFilesGroups");

            migrationBuilder.DropColumn(
                name: "ADAccountGUID",
                table: "MFilesAccounts");
        }
    }
}
