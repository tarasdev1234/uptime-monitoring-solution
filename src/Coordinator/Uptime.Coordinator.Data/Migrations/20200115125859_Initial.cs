using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Uptime.Coordinator.Data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Activities",
                columns: table => new
                {
                    MonitorId = table.Column<long>(nullable: false),
                    ExecutorId = table.Column<Guid>(nullable: false),
                    CoordinatorId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    Modified = table.Column<DateTime>(nullable: false),
                    RowVersion = table.Column<int>(nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activities", x => x.MonitorId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Activities_CoordinatorId",
                table: "Activities",
                column: "CoordinatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_ExecutorId",
                table: "Activities",
                column: "ExecutorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Activities");
        }
    }
}
