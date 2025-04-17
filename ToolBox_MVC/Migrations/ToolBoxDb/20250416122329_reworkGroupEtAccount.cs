using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToolBox_MVC.Migrations.ToolBoxDb
{
    /// <inheritdoc />
    public partial class reworkGroupEtAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MFilesAccountMFilesGroup_MFilesAccounts_AccountsAccountName_AccountsServerId",
                table: "MFilesAccountMFilesGroup");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MFilesAccounts",
                table: "MFilesAccounts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MFilesAccountMFilesGroup",
                table: "MFilesAccountMFilesGroup");

            migrationBuilder.DropIndex(
                name: "IX_MFilesAccountMFilesGroup_AccountsAccountName_AccountsServerId",
                table: "MFilesAccountMFilesGroup");

            migrationBuilder.DropColumn(
                name: "AccountsAccountName",
                table: "MFilesAccountMFilesGroup");

            migrationBuilder.RenameColumn(
                name: "AccountsServerId",
                table: "MFilesAccountMFilesGroup",
                newName: "AccountsId");

            migrationBuilder.AddColumn<int>(
                name: "MFilesId",
                table: "MFilesGroups",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ServerId",
                table: "MFilesGroups",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "MFilesAccounts",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MFilesAccounts",
                table: "MFilesAccounts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MFilesAccountMFilesGroup",
                table: "MFilesAccountMFilesGroup",
                columns: new[] { "AccountsId", "GroupsId" });

            migrationBuilder.CreateIndex(
                name: "IX_MFilesGroups_MFilesId_ServerId",
                table: "MFilesGroups",
                columns: new[] { "MFilesId", "ServerId" },
                unique: true,
                filter: "[ServerId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MFilesGroups_ServerId",
                table: "MFilesGroups",
                column: "ServerId");

            migrationBuilder.CreateIndex(
                name: "IX_MFilesAccounts_AccountName_ServerId",
                table: "MFilesAccounts",
                columns: new[] { "AccountName", "ServerId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MFilesAccountMFilesGroup_GroupsId",
                table: "MFilesAccountMFilesGroup",
                column: "GroupsId");

            migrationBuilder.AddForeignKey(
                name: "FK_MFilesAccountMFilesGroup_MFilesAccounts_AccountsId",
                table: "MFilesAccountMFilesGroup",
                column: "AccountsId",
                principalTable: "MFilesAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MFilesGroups_MFilesServers_ServerId",
                table: "MFilesGroups",
                column: "ServerId",
                principalTable: "MFilesServers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MFilesAccountMFilesGroup_MFilesAccounts_AccountsId",
                table: "MFilesAccountMFilesGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_MFilesGroups_MFilesServers_ServerId",
                table: "MFilesGroups");

            migrationBuilder.DropIndex(
                name: "IX_MFilesGroups_MFilesId_ServerId",
                table: "MFilesGroups");

            migrationBuilder.DropIndex(
                name: "IX_MFilesGroups_ServerId",
                table: "MFilesGroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MFilesAccounts",
                table: "MFilesAccounts");

            migrationBuilder.DropIndex(
                name: "IX_MFilesAccounts_AccountName_ServerId",
                table: "MFilesAccounts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MFilesAccountMFilesGroup",
                table: "MFilesAccountMFilesGroup");

            migrationBuilder.DropIndex(
                name: "IX_MFilesAccountMFilesGroup_GroupsId",
                table: "MFilesAccountMFilesGroup");

            migrationBuilder.DropColumn(
                name: "MFilesId",
                table: "MFilesGroups");

            migrationBuilder.DropColumn(
                name: "ServerId",
                table: "MFilesGroups");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "MFilesAccounts");

            migrationBuilder.RenameColumn(
                name: "AccountsId",
                table: "MFilesAccountMFilesGroup",
                newName: "AccountsServerId");

            migrationBuilder.AddColumn<string>(
                name: "AccountsAccountName",
                table: "MFilesAccountMFilesGroup",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MFilesAccounts",
                table: "MFilesAccounts",
                columns: new[] { "AccountName", "ServerId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_MFilesAccountMFilesGroup",
                table: "MFilesAccountMFilesGroup",
                columns: new[] { "GroupsId", "AccountsAccountName", "AccountsServerId" });

            migrationBuilder.CreateIndex(
                name: "IX_MFilesAccountMFilesGroup_AccountsAccountName_AccountsServerId",
                table: "MFilesAccountMFilesGroup",
                columns: new[] { "AccountsAccountName", "AccountsServerId" });

            migrationBuilder.AddForeignKey(
                name: "FK_MFilesAccountMFilesGroup_MFilesAccounts_AccountsAccountName_AccountsServerId",
                table: "MFilesAccountMFilesGroup",
                columns: new[] { "AccountsAccountName", "AccountsServerId" },
                principalTable: "MFilesAccounts",
                principalColumns: new[] { "AccountName", "ServerId" },
                onDelete: ReferentialAction.Cascade);
        }
    }
}
