using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToolBox_MVC.Migrations.ToolBoxDb
{
    /// <inheritdoc />
    public partial class _202504161049 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MFilesAccounts_MFilesServers_MFilesServerId",
                table: "MFilesAccounts");

            migrationBuilder.DropIndex(
                name: "IX_MFilesAccounts_MFilesServerId",
                table: "MFilesAccounts");

            migrationBuilder.DropColumn(
                name: "MFilesServerId",
                table: "MFilesAccounts");

            migrationBuilder.RenameColumn(
                name: "ServerID",
                table: "MFilesAccounts",
                newName: "ServerId");

            migrationBuilder.CreateIndex(
                name: "IX_MFilesAccounts_ServerId",
                table: "MFilesAccounts",
                column: "ServerId");

            migrationBuilder.AddForeignKey(
                name: "FK_MFilesAccounts_MFilesServers_ServerId",
                table: "MFilesAccounts",
                column: "ServerId",
                principalTable: "MFilesServers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MFilesAccounts_MFilesServers_ServerId",
                table: "MFilesAccounts");

            migrationBuilder.DropIndex(
                name: "IX_MFilesAccounts_ServerId",
                table: "MFilesAccounts");

            migrationBuilder.RenameColumn(
                name: "ServerId",
                table: "MFilesAccounts",
                newName: "ServerID");

            migrationBuilder.AddColumn<int>(
                name: "MFilesServerId",
                table: "MFilesAccounts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_MFilesAccounts_MFilesServerId",
                table: "MFilesAccounts",
                column: "MFilesServerId");

            migrationBuilder.AddForeignKey(
                name: "FK_MFilesAccounts_MFilesServers_MFilesServerId",
                table: "MFilesAccounts",
                column: "MFilesServerId",
                principalTable: "MFilesServers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
