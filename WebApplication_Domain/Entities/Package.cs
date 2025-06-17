using System.ComponentModel.DataAnnotations;

namespace WebApplication_Domain.Entities
{
    public class Package
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }


        // Yeni Alanlar:
        public int? DiscountRate { get; set; } // Örneğin: %31 indirim
        public decimal? OriginalPrice { get; set; } // İndirim öncesi fiyat
        public string? BannerText { get; set; } // Örn: "Her Şey Dahil", "Kampanyalı"
        public string? Feature1 { get; set; } // Örn: "100 Soru Sorma Hakkı"
        public string? Feature2 { get; set; } // Örn: "PDF Özet Notlar"
        public string? Feature3 { get; set; } // Örn: "Videolu Deneme Sınavları"
        public string? Feature4 { get; set; } // ...
        public string? CoverImagePath { get; set; } // Kapak görseli


        // Relationships
        public ICollection<Course> Courses { get; set; } // Paket içindeki kurslar
        public ICollection<Payment> Payments { get; set; } // Satın alma bilgileri
    }
}
