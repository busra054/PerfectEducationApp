using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication_Domain.Entities
{
    public class CourseMaterialViewModel
    {
        [Required]
        public int CourseId { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public IFormFile File { get; set; }
    }
}
