using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gradutionproject.Migrations
{
    public partial class Update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SectionPdf",
                table: "Sections");

            migrationBuilder.DropColumn(
                name: "ContentPdf",
                table: "Lectures");

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "Sections",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "Lectures",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "Sections");

            migrationBuilder.DropColumn(
                name: "FileName",
                table: "Lectures");

            migrationBuilder.AddColumn<byte[]>(
                name: "SectionPdf",
                table: "Sections",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "ContentPdf",
                table: "Lectures",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }
    }
}
