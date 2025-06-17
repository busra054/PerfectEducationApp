using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication_Deneme.Migrations
{
    /// <inheritdoc />
    public partial class FixPackagesId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "TYT-Eğitimi Full Paket");

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "AYT-Sayısal Eğitimi Full Paket");

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "AYT-Eşit Ağırlık Eğitimi");

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "KPSS Kapsamlı Eğitimi");

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 5,
                column: "Name",
                value: "DGS Kapsamlı Eğitimi");

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "BannerText", "CoverImagePath", "Description", "DiscountRate", "Feature1", "Feature2", "Feature3", "Name", "OriginalPrice", "Price" },
                values: new object[] { "Kampanyalı", "/img/6.jpg", "Dikey Geçiş Sınavı için tüm başlıkları kapsayan hazırlık paketi.", 20, "50 Deneme Sınavı", "PDF Özet Notlar", "Video Çözümler", "İngilizce A1-A2 Temel Seviye Eğitim", 20000.00m, 16000.00m });

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "BannerText", "CoverImagePath", "Description", "DiscountRate", "Feature1", "Feature2", "Feature3", "Name", "OriginalPrice", "Price" },
                values: new object[] { "İndirimli", "/img/7.jpg", "Okuma, yazma, konuşma ve dinleme pratiği ile A1-A2 seviyesine hazırlık.", 15, "Canlı Konuşma Seansları", "PDF Eğitim Materyalleri", "Kısa Video Dersler", "İspanyolca A1-A2 Temel Seviye Eğitim", 15000.00m, 12750.00m });

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Temel Korece becerilerini geliştiren A1-A2 paketi.", "Korece A1-A2 Temel Seviye Eğitim" });

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 9,
                column: "Name",
                value: "Almanca A1-B2 Temelden Orta Düzeye(İntermediate) Seviye Eğitim");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "TYT+AYT 2026 Paketi");

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "TYT 2026 Express Paketi");

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "AYT Sayısal 2026 Paketi");

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "AYT Eşit Ağırlık 2026 Paketi");

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 5,
                column: "Name",
                value: "KPSS Kapsamlı Paketi");

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "BannerText", "CoverImagePath", "Description", "DiscountRate", "Feature1", "Feature2", "Feature3", "Name", "OriginalPrice", "Price" },
                values: new object[] { "İndirimli", "/img/7.jpg", "Okuma, yazma, konuşma ve dinleme pratiği ile A1-A2 seviyesine hazırlık.", 15, "Canlı Konuşma Seansları", "PDF Eğitim Materyalleri", "Kısa Video Dersler", "İngilizce A1-A2 Temel Paketi", 15000.00m, 12750.00m });

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "BannerText", "CoverImagePath", "Description", "DiscountRate", "Feature1", "Feature2", "Feature3", "Name", "OriginalPrice", "Price" },
                values: new object[] { "Kampanyalı", "/img/6.jpg", "Dikey Geçiş Sınavı için tüm başlıkları kapsayan hazırlık paketi.", 20, "50 Deneme Sınavı", "PDF Özet Notlar", "Video Çözümler", "DGS Kapsamlı Paketi", 20000.00m, 16000.00m });

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Temel İspanyolca becerilerini geliştiren A1-A2 paketi.", "İspanyolca A1-A2 Temel Paketi" });

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 9,
                column: "Name",
                value: "Korece A1-A2 Temel Paketi");
        }
    }
}
