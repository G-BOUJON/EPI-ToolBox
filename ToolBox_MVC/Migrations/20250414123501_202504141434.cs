using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToolBox_MVC.Migrations
{
    /// <inheritdoc />
    public partial class _202504141434 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ServerRoleID",
                table: "Accounts",
                newName: "ServerRoles");

            migrationBuilder.RenameColumn(
                name: "LicenseID",
                table: "Accounts",
                newName: "LicenseType");

            migrationBuilder.RenameColumn(
                name: "EmailAdress",
                table: "Accounts",
                newName: "EmailAddress");

            migrationBuilder.RenameColumn(
                name: "AccountTypeID",
                table: "Accounts",
                newName: "AccountType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ServerRoles",
                table: "Accounts",
                newName: "ServerRoleID");

            migrationBuilder.RenameColumn(
                name: "LicenseType",
                table: "Accounts",
                newName: "LicenseID");

            migrationBuilder.RenameColumn(
                name: "EmailAddress",
                table: "Accounts",
                newName: "EmailAdress");

            migrationBuilder.RenameColumn(
                name: "AccountType",
                table: "Accounts",
                newName: "AccountTypeID");
        }
    }
}
