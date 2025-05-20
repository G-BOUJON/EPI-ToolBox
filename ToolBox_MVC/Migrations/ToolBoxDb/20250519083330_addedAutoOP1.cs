using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToolBox_MVC.Migrations.ToolBoxDb
{
    /// <inheritdoc />
    public partial class addedAutoOP1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AutomaticOP_AutoActivationHandling",
                table: "MFilesServers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AutomaticOP_AutoRemove",
                table: "MFilesServers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AutomaticOP_AutoRestore",
                table: "MFilesServers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AutomaticOP_AutoActivationHandling",
                table: "MFilesServers");

            migrationBuilder.DropColumn(
                name: "AutomaticOP_AutoRemove",
                table: "MFilesServers");

            migrationBuilder.DropColumn(
                name: "AutomaticOP_AutoRestore",
                table: "MFilesServers");
        }
    }
}
