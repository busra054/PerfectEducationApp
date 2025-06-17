using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication_Domain.Entities
{

    public class Appointment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PackageId { get; set; }
        [ForeignKey("PackageId")]
        public Package Package { get; set; }

        // Kurs ilişkisi
        [Required]
        public int CourseId { get; set; }
        [ForeignKey("CourseId")]
        public Course Course { get; set; }


        public int StudentId { get; set; }
        [ForeignKey("StudentId")]
        public Student Student { get; set; }

        public int TeacherId { get; set; }

        [ForeignKey("TeacherId")]
        public Teacher Teacher { get; set; } // User değil Teacher

        public DateTime Date { get; set; }
        public string Type { get; set; }

        [StringLength(500)]
        public string? ZoomLink { get; set; }
    }
}
