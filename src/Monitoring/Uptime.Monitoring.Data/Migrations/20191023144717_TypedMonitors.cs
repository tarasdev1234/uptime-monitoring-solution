using Microsoft.EntityFrameworkCore.Migrations;

namespace Uptime.Monitoring.Data.Migrations
{
    public partial class TypedMonitors : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "ContainsWord",
                table: "Monitors",
                nullable: true,
                oldClrType: typeof(bool));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "ContainsWord",
                table: "Monitors",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);
        }
    }
}
