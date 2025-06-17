using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication_Deneme.Migrations
{
    /// <inheritdoc />
    public partial class AddPackageCourseToAppointment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CourseId",
                table: "Appointments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PackageId",
                table: "Appointments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_CourseId",
                table: "Appointments",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_PackageId",
                table: "Appointments",
                column: "PackageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Course_CourseId",
                table: "Appointments",
                column: "CourseId",
                principalTable: "Course",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Packages_PackageId",
                table: "Appointments",
                column: "PackageId",
                principalTable: "Packages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Course_CourseId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Packages_PackageId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_CourseId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_PackageId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "PackageId",
                table: "Appointments");
        }
    }
}
