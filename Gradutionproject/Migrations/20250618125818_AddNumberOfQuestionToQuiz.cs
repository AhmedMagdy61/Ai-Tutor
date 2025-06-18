using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gradutionproject.Migrations
{
    public partial class AddNumberOfQuestionToQuiz : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumberOfQuestion",
                table: "Quizzes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfQuestion",
                table: "Quizzes");
        }
    }
}
