using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace WebApplication_Domain.Entities
{
    public class Assignment
    {
        public Assignment() // Constructor ekle
        {
            Submissions = new HashSet<AssignmentSubmission>();
        }

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Ödev başlığı zorunludur")]
        [Display(Name = "Ödev Başlığı")]
        public string Title { get; set; }

        [Display(Name = "Açıklama")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Teslim tarihi zorunludur")]
        [Display(Name = "Teslim Tarihi")]
        public DateTime DueDate { get; set; }

        [Display(Name = "Ek Dosya")]
        public string? AttachmentPath { get; set; } // Nullable yap

        [Display(Name = "Oluşturulma Tarihi")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Kurs seçimi zorunludur")]
        public int CourseId { get; set; }

        [ForeignKey("CourseId")]
        [ValidateNever]
        public Course Course { get; set; }

        public ICollection<AssignmentSubmission> Submissions { get; set; }
    }
}
