using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication_Domain.Entities
{
    public class Teacher
    {

        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }


        [ForeignKey("UserId")]
        public User User { get; set; }


        [Required]
        public string Biography { get; set; }

        [Required]
        public int ExperienceYears { get; set; }

        public string Certifications { get; set; }
        public ICollection<TeacherBranch> TeacherBranches { get; set; } = new List<TeacherBranch>();
        public ICollection<Course> Courses { get; set; } = new List<Course>();
        public ICollection<Appointment> AppointmentsAsTeacher { get; set; } = new List<Appointment>(); // Düzeltildi


    }
}
