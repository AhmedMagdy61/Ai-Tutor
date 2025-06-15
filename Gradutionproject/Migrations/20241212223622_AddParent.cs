using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gradutionproject.Migrations
{
    public partial class AddParent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmailParent",
                table: "User",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhonePaent",
                table: "User",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EmailParent",
                table: "registers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhonePaent",
                table: "registers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailParent",
                table: "User");

            migrationBuilder.DropColumn(
                name: "PhonePaent",
                table: "User");

            migrationBuilder.DropColumn(
                name: "EmailParent",
                table: "registers");

            migrationBuilder.DropColumn(
                name: "PhonePaent",
                table: "registers");
        }
    }
}
