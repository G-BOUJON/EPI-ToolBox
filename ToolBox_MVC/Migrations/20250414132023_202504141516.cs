using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToolBox_MVC.Migrations
{
    /// <inheritdoc />
    public partial class _202504141516 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "MaintainedID",
                table: "Accounts",
                type: "INTEGER",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true,
                oldDefaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "MaintainedID",
                table: "Accounts",
                type: "INTEGER",
                nullable: true,
                defaultValue: 0,
                oldClrType: typeof(bool),
                oldType: "INTEGER");
        }
    }
}
