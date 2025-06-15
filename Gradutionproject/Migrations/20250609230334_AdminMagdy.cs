using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gradutionproject.Migrations
{
    public partial class AdminMagdy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AdminId",
                table: "Sections",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AdminId",
                table: "Lectures",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AdminId",
                table: "Courses",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    AdminId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdminPassword = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.AdminId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sections_AdminId",
                table: "Sections",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_Lectures_AdminId",
                table: "Lectures",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_AdminId",
                table: "Courses",
                column: "AdminId");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Admins_AdminId",
                table: "Courses",
                column: "AdminId",
                principalTable: "Admins",
                principalColumn: "AdminId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lectures_Admins_AdminId",
                table: "Lectures",
                column: "AdminId",
                principalTable: "Admins",
                principalColumn: "AdminId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sections_Admins_AdminId",
                table: "Sections",
                column: "AdminId",
                principalTable: "Admins",
                principalColumn: "AdminId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Admins_AdminId",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_Lectures_Admins_AdminId",
                table: "Lectures");

            migrationBuilder.DropForeignKey(
                name: "FK_Sections_Admins_AdminId",
                table: "Sections");

            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropIndex(
                name: "IX_Sections_AdminId",
                table: "Sections");

            migrationBuilder.DropIndex(
                name: "IX_Lectures_AdminId",
                table: "Lectures");

            migrationBuilder.DropIndex(
                name: "IX_Courses_AdminId",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "AdminId",
                table: "Sections");

            migrationBuilder.DropColumn(
                name: "AdminId",
                table: "Lectures");

            migrationBuilder.DropColumn(
                name: "AdminId",
                table: "Courses");
        }
    }
}
