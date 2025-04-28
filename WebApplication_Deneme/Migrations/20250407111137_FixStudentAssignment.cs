using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication_Deneme.Migrations
{
    /// <inheritdoc />
    public partial class FixStudentAssignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssignmentSubmissions_AspNetUsers_StudentId",
                table: "AssignmentSubmissions");

            migrationBuilder.AddColumn<string>(
                name: "Comments",
                table: "AssignmentSubmissions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Feedback",
                table: "AssignmentSubmissions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Grade",
                table: "AssignmentSubmissions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "GradedDate",
                table: "AssignmentSubmissions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AssignmentSubmissions_Students_StudentId",
                table: "AssignmentSubmissions",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssignmentSubmissions_Students_StudentId",
                table: "AssignmentSubmissions");

            migrationBuilder.DropColumn(
                name: "Comments",
                table: "AssignmentSubmissions");

            migrationBuilder.DropColumn(
                name: "Feedback",
                table: "AssignmentSubmissions");

            migrationBuilder.DropColumn(
                name: "Grade",
                table: "AssignmentSubmissions");

            migrationBuilder.DropColumn(
                name: "GradedDate",
                table: "AssignmentSubmissions");

            migrationBuilder.AddForeignKey(
                name: "FK_AssignmentSubmissions_AspNetUsers_StudentId",
                table: "AssignmentSubmissions",
                column: "StudentId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
