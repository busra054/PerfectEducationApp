using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication_Deneme.Migrations
{
    /// <inheritdoc />
    public partial class AddPackageFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_AspNetUsers_Admins_AdminProfileAdminId",
            //    table: "AspNetUsers");

            //migrationBuilder.DropIndex(
            //    name: "IX_AspNetUsers_AdminProfileAdminId",
            //    table: "AspNetUsers");

            //migrationBuilder.DropColumn(
            //    name: "AdminProfileAdminId",
            //    table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "BannerText",
                table: "Packages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CoverImagePath",
                table: "Packages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DiscountRate",
                table: "Packages",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Feature1",
                table: "Packages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Feature2",
                table: "Packages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Feature3",
                table: "Packages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Feature4",
                table: "Packages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OriginalPrice",
                table: "Packages",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "BannerText", "CoverImagePath", "DiscountRate", "Feature1", "Feature2", "Feature3", "Feature4", "OriginalPrice" },
                values: new object[] { null, null, null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "BannerText", "CoverImagePath", "DiscountRate", "Feature1", "Feature2", "Feature3", "Feature4", "OriginalPrice" },
                values: new object[] { null, null, null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "BannerText", "CoverImagePath", "DiscountRate", "Feature1", "Feature2", "Feature3", "Feature4", "OriginalPrice" },
                values: new object[] { null, null, null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "BannerText", "CoverImagePath", "DiscountRate", "Feature1", "Feature2", "Feature3", "Feature4", "OriginalPrice" },
                values: new object[] { null, null, null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "BannerText", "CoverImagePath", "DiscountRate", "Feature1", "Feature2", "Feature3", "Feature4", "OriginalPrice" },
                values: new object[] { null, null, null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "BannerText", "CoverImagePath", "DiscountRate", "Feature1", "Feature2", "Feature3", "Feature4", "OriginalPrice" },
                values: new object[] { null, null, null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "BannerText", "CoverImagePath", "DiscountRate", "Feature1", "Feature2", "Feature3", "Feature4", "OriginalPrice" },
                values: new object[] { null, null, null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "BannerText", "CoverImagePath", "DiscountRate", "Feature1", "Feature2", "Feature3", "Feature4", "OriginalPrice" },
                values: new object[] { null, null, null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "BannerText", "CoverImagePath", "DiscountRate", "Feature1", "Feature2", "Feature3", "Feature4", "OriginalPrice" },
                values: new object[] { null, null, null, null, null, null, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BannerText",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "CoverImagePath",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "DiscountRate",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "Feature1",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "Feature2",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "Feature3",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "Feature4",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "OriginalPrice",
                table: "Packages");

            migrationBuilder.AddColumn<int>(
                name: "AdminProfileAdminId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_AdminProfileAdminId",
                table: "AspNetUsers",
                column: "AdminProfileAdminId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Admins_AdminProfileAdminId",
                table: "AspNetUsers",
                column: "AdminProfileAdminId",
                principalTable: "Admins",
                principalColumn: "AdminId");
        }
    }
}
