using Microsoft.EntityFrameworkCore.Migrations;

namespace Reliablesite.Authority.Data.Migrations
{
    public partial class AddedSignatureField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Signature",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Signature",
                table: "AspNetUsers");
        }
    }
}
