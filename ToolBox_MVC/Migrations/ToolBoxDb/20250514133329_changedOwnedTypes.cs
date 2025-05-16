using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToolBox_MVC.Migrations.ToolBoxDb
{
    /// <inheritdoc />
    public partial class changedOwnedTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MFilesServers_ADCredentials_ADCredentialId",
                table: "MFilesServers");

            migrationBuilder.DropTable(
                name: "ADCredentials");

            migrationBuilder.DropTable(
                name: "MFilesCredentials");

            migrationBuilder.DropIndex(
                name: "IX_MFilesServers_ADCredentialId",
                table: "MFilesServers");

            migrationBuilder.DropColumn(
                name: "ADCredentialId",
                table: "MFilesServers");

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

            migrationBuilder.AddColumn<string>(
                name: "Domain",
                table: "MFilesServers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MfCredential_EncryptedPassword",
                table: "MFilesServers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MfCredential_EncryptedUserName",
                table: "MFilesServers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "Domain",
                table: "MFilesServers");

            migrationBuilder.DropColumn(
                name: "MfCredential_EncryptedPassword",
                table: "MFilesServers");

            migrationBuilder.DropColumn(
                name: "MfCredential_EncryptedUserName",
                table: "MFilesServers");

            migrationBuilder.AddColumn<int>(
                name: "ADCredentialId",
                table: "MFilesServers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ADCredentials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Container = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Domain = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EncryptedPassword = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EncryptedUsername = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ADCredentials", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MFilesCredentials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServerId = table.Column<int>(type: "int", nullable: false),
                    Domain = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EncryptedPassword = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EncryptedUserName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MFilesCredentials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MFilesCredentials_MFilesServers_ServerId",
                        column: x => x.ServerId,
                        principalTable: "MFilesServers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MFilesServers_ADCredentialId",
                table: "MFilesServers",
                column: "ADCredentialId");

            migrationBuilder.CreateIndex(
                name: "IX_ADCredentials_Domain",
                table: "ADCredentials",
                column: "Domain",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MFilesCredentials_ServerId",
                table: "MFilesCredentials",
                column: "ServerId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MFilesServers_ADCredentials_ADCredentialId",
                table: "MFilesServers",
                column: "ADCredentialId",
                principalTable: "ADCredentials",
                principalColumn: "Id");
        }
    }
}
