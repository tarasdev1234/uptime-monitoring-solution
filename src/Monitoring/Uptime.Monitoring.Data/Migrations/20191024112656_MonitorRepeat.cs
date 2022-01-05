using Microsoft.EntityFrameworkCore.Migrations;

namespace Uptime.Monitoring.Data.Migrations
{
    public partial class MonitorRepeat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Repeat",
                table: "Monitors",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Repeat",
                table: "Monitors");
        }
    }
}
