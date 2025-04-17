using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToolBox_MVC.Migrations.ToolBoxDb
{
    /// <inheritdoc />
    public partial class NewBDD : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MFilesServers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NetworkAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EndPoint = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProtocolSequence = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MFilesServers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MFilesAccounts",
                columns: table => new
                {
                    AccountName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ServerID = table.Column<int>(type: "int", nullable: false),
                    Domain = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmailAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccountType = table.Column<int>(type: "int", nullable: false),
                    License = table.Column<int>(type: "int", nullable: false),
                    ServerRole = table.Column<int>(type: "int", nullable: false),
                    Enabled = table.Column<bool>(type: "bit", nullable: false),
                    Maintained = table.Column<bool>(type: "bit", nullable: false),
                    Actived = table.Column<bool>(type: "bit", nullable: false),
                    MFilesServerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MFilesAccounts", x => new { x.AccountName, x.ServerID });
                    table.ForeignKey(
                        name: "FK_MFilesAccounts_MFilesServers_MFilesServerId",
                        column: x => x.MFilesServerId,
                        principalTable: "MFilesServers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MFilesCredentials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServerId = table.Column<int>(type: "int", nullable: false),
                    EncryptedUserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EncryptedPassword = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Domain = table.Column<string>(type: "nvarchar(max)", nullable: false)
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
                name: "IX_MFilesAccounts_MFilesServerId",
                table: "MFilesAccounts",
                column: "MFilesServerId");

            migrationBuilder.CreateIndex(
                name: "IX_MFilesCredentials_ServerId",
                table: "MFilesCredentials",
                column: "ServerId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MFilesAccounts");

            migrationBuilder.DropTable(
                name: "MFilesCredentials");

            migrationBuilder.DropTable(
                name: "MFilesServers");
        }
    }
}
