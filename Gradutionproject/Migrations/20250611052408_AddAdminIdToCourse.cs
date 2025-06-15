using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gradutionproject.Migrations
{
    public partial class AddAdminIdToCourse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AdminId",
                table: "Sections",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "AdminId",
                table: "Lectures",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "AdminId",
                table: "Courses",
                type: "int",
                nullable: false,
                defaultValue: 1);

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

            migrationBuilder.InsertData(
                table: "Admins",
                columns: new[] { "AdminId", "AdminPassword", "Email", "Username" },
                values: new object[] { 1, "123456", "Eslam@admin.com", "Eslam" });

            migrationBuilder.InsertData(
                table: "Courses",
                columns: new[] { "Id", "AdminId", "ImageName", "Title" },
                values: new object[] { 1, 1, "default.jpg", "Test Course" });

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
                principalColumn: "AdminId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Lectures_Admins_AdminId",
                table: "Lectures",
                column: "AdminId",
                principalTable: "Admins",
                principalColumn: "AdminId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sections_Admins_AdminId",
                table: "Sections",
                column: "AdminId",
                principalTable: "Admins",
                principalColumn: "AdminId",
                onDelete: ReferentialAction.Restrict);
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

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 1);

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
