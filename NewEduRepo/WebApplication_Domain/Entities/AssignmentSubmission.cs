using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication_Domain.Entities
{
    public class AssignmentSubmission
    {
        [Key]
        public int Id { get; set; }

        
        [Display(Name = "Dosya Yolu")]
        public string? FilePath { get; set; }

        [Display(Name = "Gönderim Tarihi")]
        public DateTime SubmissionDate { get; set; } = DateTime.Now;
        

        // İlişkiler
        public int AssignmentId { get; set; }

        [ForeignKey("AssignmentId")]
        public Assignment? Assignment { get; set; }

        public int StudentId { get; set; }

        [ForeignKey("StudentId")]
        public Student? Student { get; set; }


        [Display(Name = "Yorumlar")]
          public string? Comments { get; set; }

        [Display(Name = "Not")]
        [Range(0, 100)]
        public int? Grade { get; set; }

        [Display(Name = "Geri Bildirim")]
        public string? Feedback { get; set; }

        [Display(Name = "Notlandırma Tarihi")]
         public DateTime? GradedDate { get; set; }

    }
}
