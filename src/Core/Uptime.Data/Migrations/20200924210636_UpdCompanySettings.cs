using Microsoft.EntityFrameworkCore.Migrations;

namespace Uptime.Data.Migrations
{
    public partial class UpdCompanySettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanySettings_Users_UserId",
                table: "CompanySettings");

            migrationBuilder.DropForeignKey(
                name: "FK_UserDepartments_Users_UserId",
                table: "UserDepartments");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_CompanySettings_Users_UserId",
                table: "CompanySettings",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserDepartments_Users_UserId",
                table: "UserDepartments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
