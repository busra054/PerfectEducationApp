using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace WebApplication_Domain.Entities
{
    public class CourseMaterial
    {
        [Key]
        public int Id { get; set; }

        public int CourseId { get; set; }

        [DisplayName("Materyal Türü")]
        public string Type { get; set; } // Örneğin: "Video", "PDF", "Doküman"

        [DisplayName("Dosya Yolu")]
        public string FilePath { get; set; }

        public DateTime UploadDate { get; set; }

        // Relationships
        public Course Course { get; set; }
        public User UploadedBy { get; set; }
        public int UploadedById { get; set; } 
    }
}
