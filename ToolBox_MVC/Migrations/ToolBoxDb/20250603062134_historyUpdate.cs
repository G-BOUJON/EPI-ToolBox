using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToolBox_MVC.Migrations.ToolBoxDb
{
    /// <inheritdoc />
    public partial class historyUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HistoryOperations_MFilesAccounts_AccountId",
                table: "HistoryOperations");

            migrationBuilder.RenameColumn(
                name: "AccountId",
                table: "HistoryOperations",
                newName: "AccountID");

            migrationBuilder.RenameIndex(
                name: "IX_HistoryOperations_AccountId",
                table: "HistoryOperations",
                newName: "IX_HistoryOperations_AccountID");

            migrationBuilder.AddForeignKey(
                name: "FK_HistoryOperations_MFilesAccounts_AccountID",
                table: "HistoryOperations",
                column: "AccountID",
                principalTable: "MFilesAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HistoryOperations_MFilesAccounts_AccountID",
                table: "HistoryOperations");

            migrationBuilder.RenameColumn(
                name: "AccountID",
                table: "HistoryOperations",
                newName: "AccountId");

            migrationBuilder.RenameIndex(
                name: "IX_HistoryOperations_AccountID",
                table: "HistoryOperations",
                newName: "IX_HistoryOperations_AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_HistoryOperations_MFilesAccounts_AccountId",
                table: "HistoryOperations",
                column: "AccountId",
                principalTable: "MFilesAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
