using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Uptime.Monitoring.Data.Migrations
{
    public partial class LastExecutor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Monitors_MonitoringServers_ServerId",
                table: "Monitors");

            migrationBuilder.DropIndex(
                name: "IX_Monitors_ServerId",
                table: "Monitors");

            migrationBuilder.DropColumn(
                name: "ServerId",
                table: "Monitors");

            migrationBuilder.DropColumn(
                name: "UptimeDay",
                table: "Monitors");

            migrationBuilder.DropColumn(
                name: "UptimeMonth",
                table: "Monitors");

            migrationBuilder.DropColumn(
                name: "UptimeWeek",
                table: "Monitors");

            migrationBuilder.AddColumn<Guid>(
                name: "LastExecutor",
                table: "Monitors",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastExecutor",
                table: "Monitors");

            migrationBuilder.AddColumn<long>(
                name: "ServerId",
                table: "Monitors",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UptimeDay",
                table: "Monitors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UptimeMonth",
                table: "Monitors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UptimeWeek",
                table: "Monitors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Monitors_ServerId",
                table: "Monitors",
                column: "ServerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Monitors_MonitoringServers_ServerId",
                table: "Monitors",
                column: "ServerId",
                principalTable: "MonitoringServers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
