using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToolBox_MVC.Migrations.ToolBoxDb
{
    /// <inheritdoc />
    public partial class Add_Server_AdLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ADCredentialId",
                table: "MFilesServers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MFilesServers_ADCredentialId",
                table: "MFilesServers",
                column: "ADCredentialId");

            migrationBuilder.AddForeignKey(
                name: "FK_MFilesServers_ADCredentials_ADCredentialId",
                table: "MFilesServers",
                column: "ADCredentialId",
                principalTable: "ADCredentials",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MFilesServers_ADCredentials_ADCredentialId",
                table: "MFilesServers");

            migrationBuilder.DropIndex(
                name: "IX_MFilesServers_ADCredentialId",
                table: "MFilesServers");

            migrationBuilder.DropColumn(
                name: "ADCredentialId",
                table: "MFilesServers");
        }
    }
}
