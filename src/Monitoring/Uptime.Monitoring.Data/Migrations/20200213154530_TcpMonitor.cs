using Microsoft.EntityFrameworkCore.Migrations;

namespace Uptime.Monitoring.Data.Migrations
{
    public partial class TcpMonitor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Port",
                table: "Monitors",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Port",
                table: "Monitors");
        }
    }
}
