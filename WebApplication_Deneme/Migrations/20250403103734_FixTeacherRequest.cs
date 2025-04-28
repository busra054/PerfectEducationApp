using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication_Deneme.Migrations
{
    /// <inheritdoc />
    public partial class FixTeacherRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BranchId",
                table: "TeacherRequests",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeacherRequests_BranchId",
                table: "TeacherRequests",
                column: "BranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherRequests_Branches_BranchId",
                table: "TeacherRequests",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeacherRequests_Branches_BranchId",
                table: "TeacherRequests");

            migrationBuilder.DropIndex(
                name: "IX_TeacherRequests_BranchId",
                table: "TeacherRequests");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "TeacherRequests");
        }
    }
}
