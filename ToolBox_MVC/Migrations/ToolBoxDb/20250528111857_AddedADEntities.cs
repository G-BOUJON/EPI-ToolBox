using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToolBox_MVC.Migrations.ToolBoxDb
{
    /// <inheritdoc />
    public partial class AddedADEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActiveDirectories",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContextType = table.Column<int>(type: "int", nullable: false),
                    Container = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EncryptedCredentials_Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EncryptedCredentials_Password = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActiveDirectories", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ADAccounts",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GUID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmailAdress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Enabled = table.Column<bool>(type: "bit", nullable: false),
                    ActiveDirectoryID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ADAccounts", x => x.ID);
                    table.UniqueConstraint("AK_ADAccounts_GUID", x => x.GUID);
                    table.ForeignKey(
                        name: "FK_ADAccounts_ActiveDirectories_ActiveDirectoryID",
                        column: x => x.ActiveDirectoryID,
                        principalTable: "ActiveDirectories",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ADGroups",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GUID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActiveDirectoryID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ADGroups", x => x.ID);
                    table.UniqueConstraint("AK_ADGroups_GUID", x => x.GUID);
                    table.ForeignKey(
                        name: "FK_ADGroups_ActiveDirectories_ActiveDirectoryID",
                        column: x => x.ActiveDirectoryID,
                        principalTable: "ActiveDirectories",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ADAccountADGroup",
                columns: table => new
                {
                    AccountsID = table.Column<int>(type: "int", nullable: false),
                    GroupsID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ADAccountADGroup", x => new { x.AccountsID, x.GroupsID });
                    table.ForeignKey(
                        name: "FK_ADAccountADGroup_ADAccounts_AccountsID",
                        column: x => x.AccountsID,
                        principalTable: "ADAccounts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ADAccountADGroup_ADGroups_GroupsID",
                        column: x => x.GroupsID,
                        principalTable: "ADGroups",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ADAccountADGroup_GroupsID",
                table: "ADAccountADGroup",
                column: "GroupsID");

            migrationBuilder.CreateIndex(
                name: "IX_ADAccounts_ActiveDirectoryID",
                table: "ADAccounts",
                column: "ActiveDirectoryID");

            migrationBuilder.CreateIndex(
                name: "IX_ADGroups_ActiveDirectoryID",
                table: "ADGroups",
                column: "ActiveDirectoryID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ADAccountADGroup");

            migrationBuilder.DropTable(
                name: "ADAccounts");

            migrationBuilder.DropTable(
                name: "ADGroups");

            migrationBuilder.DropTable(
                name: "ActiveDirectories");
        }
    }
}
