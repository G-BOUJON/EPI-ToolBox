using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToolBox_MVC.Migrations.ToolBoxDb
{
    /// <inheritdoc />
    public partial class addSyncTime_INTHERIGHTPLACE : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SyncTime",
                table: "MFilesCredentials");

            migrationBuilder.AddColumn<TimeOnly>(
                name: "SyncTime",
                table: "MFilesServers",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SyncTime",
                table: "MFilesServers");

            migrationBuilder.AddColumn<TimeOnly>(
                name: "SyncTime",
                table: "MFilesCredentials",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));
        }
    }
}
