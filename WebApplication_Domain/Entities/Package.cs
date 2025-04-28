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

        // Relationships
        public ICollection<Course> Courses { get; set; } // Paket içindeki kurslar
        public ICollection<Payment> Payments { get; set; } // Satın alma bilgileri
    }
}
