using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication_Deneme.Migrations
{
    /// <inheritdoc />
    public partial class AddNewPackages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "BannerText", "CoverImagePath", "Description", "DiscountRate", "Feature1", "Feature2", "Feature3", "Feature4", "Name", "OriginalPrice", "Price" },
                values: new object[] { "Her Şey Dahil", "/img/packages/tyt-ayt-2026.jpg", "TYT ve AYT tüm konuları kapsayan, eğitim, canlı yayınlar ve deneme sınavlarıyla eksiksiz hazırlık.", 30, "100 Soru Sorma Hakkı", "PDF Özet Notlar", "Videolu Deneme Sınavları", "Yapay Zeka Asistanı", "TYT+AYT 2026 Paketi", 35000.00m, 24500.00m });

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "BannerText", "CoverImagePath", "Description", "DiscountRate", "Feature1", "Feature2", "Feature3", "Feature4", "Name", "OriginalPrice", "Price" },
                values: new object[] { "Kampanyalı", "/img/packages/tyt-express.jpg", "TYT konularına hızlı tekrar ve yoğun soru çözümü içeren pratik paket.", 20, "50 Canlı Soru Saati", "40 Test Denemesi", "PDF Özet Notlar", "Yapay Zeka Asistanı", "TYT 2026 Express Paketi", 18000.00m, 14400.00m });

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "BannerText", "CoverImagePath", "Description", "DiscountRate", "Feature1", "Feature2", "Feature3", "Feature4", "Name", "OriginalPrice", "Price" },
                values: new object[] { "İndirimli", "/img/packages/ayt-sayisal.jpg", "Matematik, Fizik, Kimya ve Biyoloji konularını içeren kapsamlı sayısal paket.", 25, "80 Deneme Sınavı", "Canlı Soru Çözümü", "PDF Özet Notlar", "Yapay Zeka Asistanı", "AYT Sayısal 2026 Paketi", 30000.00m, 22500.00m });

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "BannerText", "CoverImagePath", "Description", "DiscountRate", "Feature1", "Feature2", "Feature3", "Feature4", "Name", "OriginalPrice", "Price" },
                values: new object[] { "Kampanyalı", "/img/packages/ayt-esitagirlik.jpg", "Matematik, Edebiyat, Tarih ve Coğrafya derslerini kapsayan eşit ağırlık paketi.", 25, "75 Deneme Sınavı", "Video Çözümler", "PDF Özet Notlar", "Yapay Zeka Asistanı", "AYT Eşit Ağırlık 2026 Paketi", 28000.00m, 21000.00m });

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "BannerText", "CoverImagePath", "Description", "DiscountRate", "Feature1", "Feature2", "Feature3", "Feature4", "Name", "OriginalPrice", "Price" },
                values: new object[] { "Her Şey Dahil", "/img/packages/kpss.jpg", "Genel Yetenek ve Genel Kültür derslerinden oluşan KPSS hazırlık paketi.", 28, "60 Deneme Sınavı", "Video Anlatım", "PDF Özet Notlar", "Yapay Zeka Asistanı", "KPSS Kapsamlı Paketi", 25000.00m, 18000.00m });

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "BannerText", "CoverImagePath", "Description", "DiscountRate", "Feature1", "Feature2", "Feature3", "Feature4", "Name", "OriginalPrice", "Price" },
                values: new object[] { "Kampanyalı", "/img/packages/dgs.jpg", "Dikey Geçiş Sınavı için tüm başlıkları kapsayan hazırlık paketi.", 20, "50 Deneme Sınavı", "PDF Özet Notlar", "Video Çözümler", "Yapay Zeka Asistanı", "DGS Kapsamlı Paketi", 20000.00m, 16000.00m });

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "BannerText", "CoverImagePath", "Description", "DiscountRate", "Feature1", "Feature2", "Feature3", "Feature4", "Name", "OriginalPrice", "Price" },
                values: new object[] { "İndirimli", "/img/packages/ingilizce-a1a2.jpg", "Okuma, yazma, konuşma ve dinleme pratiği ile A1-A2 seviyesine hazırlık.", 15, "Canlı Konuşma Seansları", "PDF Eğitim Materyalleri", "Kısa Video Dersler", "Yapay Zeka Asistanı", "İngilizce A1-A2 Temel Paketi", 15000.00m, 12750.00m });

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "BannerText", "CoverImagePath", "Description", "DiscountRate", "Feature1", "Feature2", "Feature3", "Feature4", "Name", "OriginalPrice", "Price" },
                values: new object[] { "Kampanyalı", "/img/packages/ispanyolca-a1a2.jpg", "Temel İspanyolca becerilerini geliştiren A1-A2 paketi.", 15, "Konuşma Kulübü", "PDF Notlar", "Video Dersler", "Yapay Zeka Asistanı", "İspanyolca A1-A2 Temel Paketi", 14000.00m, 11900.00m });

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "BannerText", "CoverImagePath", "Description", "DiscountRate", "Feature1", "Feature2", "Feature3", "Feature4", "Name", "OriginalPrice", "Price" },
                values: new object[] { "Her Şey Dahil", "/img/packages/korece-a1a2.jpg", "İlk seviye Korece pratik ve temel dil bilgisi paketi.", 15, "Karakter Tanıma Dersleri", "PDF Eğitim Seti", "Video Anlatım", "Yapay Zeka Asistanı", "Korece A1-A2 Temel Paketi", 16000.00m, 13600.00m });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "BannerText", "CoverImagePath", "Description", "DiscountRate", "Feature1", "Feature2", "Feature3", "Feature4", "Name", "OriginalPrice", "Price" },
                values: new object[] { null, null, "TYT genel tüm dersler temelden gelişmişe adım adım Eğitim", null, null, null, null, null, "TYT-Eğitimi Full Paket", null, 14000.0m });

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "BannerText", "CoverImagePath", "Description", "DiscountRate", "Feature1", "Feature2", "Feature3", "Feature4", "Name", "OriginalPrice", "Price" },
                values: new object[] { null, null, "AYT temelden gelişmişe Matematik ,Fizik, Kimya, Biyoloji Dersleri Eğitimi", null, null, null, null, null, "AYT-Sayısal Eğitimi Full Paket", null, 18000.0m });

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "BannerText", "CoverImagePath", "Description", "DiscountRate", "Feature1", "Feature2", "Feature3", "Feature4", "Name", "OriginalPrice", "Price" },
                values: new object[] { null, null, "AYT temelden gelişmişe Matematik ,Tarih, Coğrafya, Edebiyat Dersleri eğitimi", null, null, null, null, null, "AYT-Eşit Ağırlık Eğitimi", null, 14000.0m });

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "BannerText", "CoverImagePath", "Description", "DiscountRate", "Feature1", "Feature2", "Feature3", "Feature4", "Name", "OriginalPrice", "Price" },
                values: new object[] { null, null, "KPSS temelden gelişmişe full hazırlık paketi dersleri", null, null, null, null, null, "KPSS Kapsamlı Eğitimi ", null, 23000.0m });

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "BannerText", "CoverImagePath", "Description", "DiscountRate", "Feature1", "Feature2", "Feature3", "Feature4", "Name", "OriginalPrice", "Price" },
                values: new object[] { null, null, "DGS temelden gelişmişe full hazırlık paketi dersleri", null, null, null, null, null, "DGS Kapsamlı Eğitimi", null, 13000.0m });

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "BannerText", "CoverImagePath", "Description", "DiscountRate", "Feature1", "Feature2", "Feature3", "Feature4", "Name", "OriginalPrice", "Price" },
                values: new object[] { null, null, "İngilizce temelden gelişmişe okuma,yazma,konuşma ve dinleme becerileri geliştirme dersleri", null, null, null, null, null, "İngilizce A1-A2 Temel Seviye Eğitim", null, 15000.0m });

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "BannerText", "CoverImagePath", "Description", "DiscountRate", "Feature1", "Feature2", "Feature3", "Feature4", "Name", "OriginalPrice", "Price" },
                values: new object[] { null, null, "İspanyolca temelden gelişmişe okuma,yazma,konuşma ve dinleme becerileri geliştirme dersleri", null, null, null, null, null, "İspanyolca A1-A2 Temel Seviye Eğitim", null, 12000.0m });

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "BannerText", "CoverImagePath", "Description", "DiscountRate", "Feature1", "Feature2", "Feature3", "Feature4", "Name", "OriginalPrice", "Price" },
                values: new object[] { null, null, "Korece temelden gelişmişe okuma,yazma,konuşma ve dinleme becerileri geliştirme dersleri", null, null, null, null, null, "Korece A1-A2 Temel Seviye Eğitim", null, 16000.0m });

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "BannerText", "CoverImagePath", "Description", "DiscountRate", "Feature1", "Feature2", "Feature3", "Feature4", "Name", "OriginalPrice", "Price" },
                values: new object[] { null, null, "Almanca temelden gelişmişe okuma,yazma,konuşma ve dinleme becerileri geliştirme dersleri", null, null, null, null, null, "Almanca A1-B2 Temelden Orta Düzeye(İntermediate) Seviye Eğitim", null, 17000.0m });
        }
    }
}
