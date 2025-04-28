using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;


namespace WebApplication_Domain.Entities
{
    using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

    public class Course
    {
        public Course()
        {
            CourseMaterials = new HashSet<CourseMaterial>();
            EnrolledStudents = new HashSet<CourseEnrollment>();
            Assignments = new HashSet<Assignment>();
        }

        [Key]
        public int Id { get; set; }

        [DisplayName("Ders Adı")]
        [Required(ErrorMessage = "Ders adı boş geçilemez!")]
        public string Name { get; set; }

        [DisplayName("Ders Açıklaması")]
        public string Description { get; set; }

        [Required]
        public int TeacherId { get; set; }

        [ForeignKey("TeacherId")]
        [ValidateNever] // Validasyonu kapat
        public virtual Teacher Teacher { get; set; }

        [Required]
        public int PackageId { get; set; }

        [ForeignKey("PackageId")]
        [ValidateNever] // Validasyonu kapat
        public virtual Package Package { get; set; }

        public virtual ICollection<CourseMaterial> CourseMaterials { get; set; }
        public virtual ICollection<CourseEnrollment> EnrolledStudents { get; set; }
        public virtual ICollection<Assignment> Assignments { get; set; }
    }
}
