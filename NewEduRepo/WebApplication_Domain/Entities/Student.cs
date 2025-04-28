using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication_Domain.Entities
{
    public class Student
    {

        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [NotMapped] // Veritabanına kaydedilmesin
        public string Name => User?.Name; // User üzerinden name'e erişim
        public ICollection<Payment> PurchasedPackages { get; set; } // Yeni eklenen


        public DateTime EnrollmentDate { get; set; }
        public ICollection<CourseEnrollment> Enrollments { get; set; } = new List<CourseEnrollment>();
        public ICollection<Appointment> AppointmentsAsStudent { get; set; } = new List<Appointment>(); // Düzeltildi
    }
}
