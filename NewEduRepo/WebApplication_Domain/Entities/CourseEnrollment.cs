using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication_Domain.Entities
{
    public class CourseEnrollment
    {
        [Key]
        public int Id { get; set; }

        public int CourseId { get; set; }
        public int StudentId { get; set; }
        public DateTime EnrollmentDate { get; set; }


        // Relationships
        [ForeignKey("CourseId")]
        public Course Course { get; set; }

        [ForeignKey("StudentId")]
        public Student Student { get; set; } 
    }

}
