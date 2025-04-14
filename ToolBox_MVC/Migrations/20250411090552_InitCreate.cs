using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToolBox_MVC.Migrations
{
    /// <inheritdoc />
    public partial class InitCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    AccountName = table.Column<string>(type: "TEXT", nullable: false),
                    Server = table.Column<int>(type: "INTEGER", nullable: false),
                    DomainName = table.Column<string>(type: "TEXT", nullable: false),
                    EmailAdress = table.Column<string>(type: "TEXT", nullable: false),
                    Enabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    FullName = table.Column<string>(type: "TEXT", nullable: false),
                    MaintainedID = table.Column<int>(type: "INTEGER", nullable: true, defaultValue: 0),
                    UserName = table.Column<string>(type: "TEXT", nullable: false),
                    LicenseID = table.Column<int>(type: "INTEGER", nullable: false),
                    AccountTypeID = table.Column<int>(type: "INTEGER", nullable: false),
                    ServerRoleID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => new { x.AccountName, x.Server });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Accounts");
        }
    }
}
