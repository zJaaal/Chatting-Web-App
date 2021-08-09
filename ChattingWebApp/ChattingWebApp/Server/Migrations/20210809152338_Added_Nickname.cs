using Microsoft.EntityFrameworkCore.Migrations;

namespace ChattingWebApp.Server.Migrations
{
    public partial class Added_Nickname : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Nickname",
                table: "Profiles",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Nickname",
                table: "Profiles");
        }
    }
}
