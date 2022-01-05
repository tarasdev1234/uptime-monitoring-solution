using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Uptime.Monitoring.Data.Migrations
{
    public partial class init_monitoring : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AlertContacts",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<long>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    NotificationType = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlertContacts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MonitoringServers",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    AccessKey = table.Column<string>(nullable: true),
                    Host = table.Column<string>(nullable: false),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonitoringServers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserSettings",
                columns: table => new
                {
                    UserId = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    InformFeatures = table.Column<bool>(nullable: false),
                    InformDev = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSettings", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Monitors",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ServerId = table.Column<long>(nullable: true),
                    UserId = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Url = table.Column<string>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Interval = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    UptimeDay = table.Column<int>(nullable: false),
                    UptimeWeek = table.Column<int>(nullable: false),
                    UptimeMonth = table.Column<int>(nullable: false),
                    ContainsWord = table.Column<bool>(nullable: false),
                    Keyword = table.Column<string>(nullable: true),
                    AuthType = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Monitors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Monitors_MonitoringServers_ServerId",
                        column: x => x.ServerId,
                        principalTable: "MonitoringServers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MonitorAlertContacts",
                columns: table => new
                {
                    MonitorId = table.Column<long>(nullable: false),
                    AlertContactId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonitorAlertContacts", x => new { x.MonitorId, x.AlertContactId });
                    table.ForeignKey(
                        name: "FK_MonitorAlertContacts_AlertContacts_AlertContactId",
                        column: x => x.AlertContactId,
                        principalTable: "AlertContacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MonitorAlertContacts_Monitors_MonitorId",
                        column: x => x.MonitorId,
                        principalTable: "Monitors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MonitorAlertContacts_AlertContactId",
                table: "MonitorAlertContacts",
                column: "AlertContactId");

            migrationBuilder.CreateIndex(
                name: "IX_Monitors_ServerId",
                table: "Monitors",
                column: "ServerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MonitorAlertContacts");

            migrationBuilder.DropTable(
                name: "UserSettings");

            migrationBuilder.DropTable(
                name: "AlertContacts");

            migrationBuilder.DropTable(
                name: "Monitors");

            migrationBuilder.DropTable(
                name: "MonitoringServers");
        }
    }
}
