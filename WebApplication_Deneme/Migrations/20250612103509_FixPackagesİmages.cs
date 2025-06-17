using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication_Deneme.Migrations
{
    /// <inheritdoc />
    public partial class FixPackagesİmages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 1,
                column: "CoverImagePath",
                value: "/img/packages/1.jpg");

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 2,
                column: "CoverImagePath",
                value: "/img/packages/2.jpg");

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 3,
                column: "CoverImagePath",
                value: "/img/packages/3.jpg");

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 4,
                column: "CoverImagePath",
                value: "/img/packages/4.jpg");

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 5,
                column: "CoverImagePath",
                value: "/img/packages/5.jpg");

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 6,
                column: "CoverImagePath",
                value: "/img/packages/6.jpg");

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 7,
                column: "CoverImagePath",
                value: "/img/packages/7.jpg");

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 8,
                column: "CoverImagePath",
                value: "/img/packages/8.jpg");

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 9,
                column: "CoverImagePath",
                value: "/img/packages/9.jpg");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 1,
                column: "CoverImagePath",
                value: "/img/packages/tyt-ayt-2026.jpg");

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 2,
                column: "CoverImagePath",
                value: "/img/packages/tyt-express.jpg");

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 3,
                column: "CoverImagePath",
                value: "/img/packages/ayt-sayisal.jpg");

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 4,
                column: "CoverImagePath",
                value: "/img/packages/ayt-esitagirlik.jpg");

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 5,
                column: "CoverImagePath",
                value: "/img/packages/kpss.jpg");

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 6,
                column: "CoverImagePath",
                value: "/img/packages/dgs.jpg");

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 7,
                column: "CoverImagePath",
                value: "/img/packages/ingilizce-a1a2.jpg");

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 8,
                column: "CoverImagePath",
                value: "/img/packages/ispanyolca-a1a2.jpg");

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 9,
                column: "CoverImagePath",
                value: "/img/packages/korece-a1a2.jpg");
        }
    }
}
