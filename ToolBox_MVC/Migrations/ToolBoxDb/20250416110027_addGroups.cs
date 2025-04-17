using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToolBox_MVC.Migrations.ToolBoxDb
{
    /// <inheritdoc />
    public partial class addGroups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Actived",
                table: "MFilesAccounts",
                newName: "Active");

            migrationBuilder.CreateTable(
                name: "MFilesGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MFilesGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MFilesAccountMFilesGroup",
                columns: table => new
                {
                    GroupsId = table.Column<int>(type: "int", nullable: false),
                    AccountsAccountName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AccountsServerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MFilesAccountMFilesGroup", x => new { x.GroupsId, x.AccountsAccountName, x.AccountsServerId });
                    table.ForeignKey(
                        name: "FK_MFilesAccountMFilesGroup_MFilesAccounts_AccountsAccountName_AccountsServerId",
                        columns: x => new { x.AccountsAccountName, x.AccountsServerId },
                        principalTable: "MFilesAccounts",
                        principalColumns: new[] { "AccountName", "ServerId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MFilesAccountMFilesGroup_MFilesGroups_GroupsId",
                        column: x => x.GroupsId,
                        principalTable: "MFilesGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MFilesAccountMFilesGroup_AccountsAccountName_AccountsServerId",
                table: "MFilesAccountMFilesGroup",
                columns: new[] { "AccountsAccountName", "AccountsServerId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MFilesAccountMFilesGroup");

            migrationBuilder.DropTable(
                name: "MFilesGroups");

            migrationBuilder.RenameColumn(
                name: "Active",
                table: "MFilesAccounts",
                newName: "Actived");
        }
    }
}
