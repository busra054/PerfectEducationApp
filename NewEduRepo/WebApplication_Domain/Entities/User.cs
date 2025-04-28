using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace WebApplication_Domain.Entities
{
    public class User: IdentityUser<int>
    {

        [Required(ErrorMessage = "Ad zorunludur")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Rol zorunludur")]
        public string Role { get; set; }



        // Relationships
        public Teacher? TeacherProfile { get; set; }
        public Student? StudentProfile { get; set; }
        public ICollection<AssignmentSubmission>? Submissions { get; set; }
        public ICollection<TeacherBranch> TeacherBranches { get; set; } = new List<TeacherBranch>();
        public ICollection<Appointment> AppointmentsAsStudent { get; set; } = new List<Appointment>();
        public ICollection<Appointment> AppointmentsAsTeacher { get; set; } = new List<Appointment>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public ICollection<Message> SentMessages { get; set; } = new List<Message>();
        public ICollection<Message> ReceivedMessages { get; set; } = new List<Message>();
        public ICollection<Course> CoursesAsTeacher { get; set; } = new List<Course>();
        public ICollection<CourseEnrollment> EnrolledCourses { get; set; } = new List<CourseEnrollment>();
        public ICollection<CourseMaterial> UploadedMaterials { get; set; } = new List<CourseMaterial>();
        public ICollection<Package> PurchasedPackages { get; set; } = new List<Package>();

    }

}

