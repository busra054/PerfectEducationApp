using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication_Deneme.Migrations
{
    /// <inheritdoc />
    public partial class ChangePackageId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "BannerText", "CoverImagePath", "Description", "DiscountRate", "Feature1", "Feature2", "Feature3", "Name", "OriginalPrice", "Price" },
                values: new object[] { "Kampanyalı", "/img/6.jpg", "Dikey Geçiş Sınavı için tüm başlıkları kapsayan hazırlık paketi.", 20, "50 Deneme Sınavı", "PDF Özet Notlar", "Video Çözümler", "DGS Kapsamlı Paketi", 20000.00m, 16000.00m });

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "BannerText", "CoverImagePath", "Description", "DiscountRate", "Feature1", "Feature2", "Feature3", "Name", "OriginalPrice", "Price" },
                values: new object[] { "İndirimli", "/img/7.jpg", "Okuma, yazma, konuşma ve dinleme pratiği ile A1-A2 seviyesine hazırlık.", 15, "Canlı Konuşma Seansları", "PDF Eğitim Materyalleri", "Kısa Video Dersler", "İngilizce A1-A2 Temel Paketi", 15000.00m, 12750.00m });
        }
    }
}
