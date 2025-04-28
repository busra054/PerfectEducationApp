using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication_Domain.Entities
{
    public class TeacherRequest
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }

        [Required(ErrorMessage = "Biyografi alanı zorunludur")]
        [StringLength(2000)]
        public  string? Biography { get; set; }

        [Required(ErrorMessage = "Deneyim yılı zorunludur")]
        [Range(0, 50)]
        public int ExperienceYears { get; set; }

        public string? CertificationsPath { get; set; }

        public DateTime RequestDate { get; set; } = DateTime.UtcNow;

        [Required]
        public RequestStatus Status { get; set; } = RequestStatus.Pending;

        public DateTime? ReviewedDate { get; set; }

        [StringLength(500)] 
        public string? AdminNotes { get; set; }

 

        public int BranchId { get; set; }

        [ForeignKey("BranchId")]
        public Branch? Branch { get; set; }
    }

    public enum RequestStatus
    {
        Pending,
        Approved,
        Rejected
    }
}
