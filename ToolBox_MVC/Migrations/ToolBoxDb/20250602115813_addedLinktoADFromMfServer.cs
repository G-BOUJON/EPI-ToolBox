using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToolBox_MVC.Migrations.ToolBoxDb
{
    /// <inheritdoc />
    public partial class addedLinktoADFromMfServer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ActiveDirectoryID",
                table: "MFilesServers",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_MFilesServers_ActiveDirectoryID",
                table: "MFilesServers",
                column: "ActiveDirectoryID");

            migrationBuilder.AddForeignKey(
                name: "FK_MFilesServers_ActiveDirectories_ActiveDirectoryID",
                table: "MFilesServers",
                column: "ActiveDirectoryID",
                principalTable: "ActiveDirectories",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MFilesServers_ActiveDirectories_ActiveDirectoryID",
                table: "MFilesServers");

            migrationBuilder.DropIndex(
                name: "IX_MFilesServers_ActiveDirectoryID",
                table: "MFilesServers");

            migrationBuilder.DropColumn(
                name: "ActiveDirectoryID",
                table: "MFilesServers");
        }
    }
}
