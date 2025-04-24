using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToolBox_MVC.Migrations.ToolBoxDb
{
    /// <inheritdoc />
    public partial class s1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Username",
                table: "ADCredentials",
                newName: "EncryptedUsername");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "ADCredentials",
                newName: "EncryptedPassword");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EncryptedUsername",
                table: "ADCredentials",
                newName: "Username");

            migrationBuilder.RenameColumn(
                name: "EncryptedPassword",
                table: "ADCredentials",
                newName: "Password");
        }
    }
}
