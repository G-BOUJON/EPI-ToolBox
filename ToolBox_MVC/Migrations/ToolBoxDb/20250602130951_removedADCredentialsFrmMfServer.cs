using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToolBox_MVC.Migrations.ToolBoxDb
{
    /// <inheritdoc />
    public partial class removedADCredentialsFrmMfServer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ADCredential_Container",
                table: "MFilesServers");

            migrationBuilder.DropColumn(
                name: "ADCredential_Domain",
                table: "MFilesServers");

            migrationBuilder.DropColumn(
                name: "ADCredential_EncryptedPassword",
                table: "MFilesServers");

            migrationBuilder.DropColumn(
                name: "ADCredential_EncryptedUsername",
                table: "MFilesServers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ADCredential_Container",
                table: "MFilesServers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ADCredential_Domain",
                table: "MFilesServers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ADCredential_EncryptedPassword",
                table: "MFilesServers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ADCredential_EncryptedUsername",
                table: "MFilesServers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
