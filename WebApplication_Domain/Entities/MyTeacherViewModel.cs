using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication_Domain.Entities
{
    public class MyTeacherViewModel
    {
        public int TeacherId { get; set; }
        public string Name { get; set; }
        public string Biography { get; set; }
        public List<Course> Courses { get; set; }
    }
}
