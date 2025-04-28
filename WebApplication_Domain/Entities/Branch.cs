using System.ComponentModel.DataAnnotations;

namespace WebApplication_Domain.Entities
{
    public class Branch
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        // Relationships
        public ICollection<TeacherBranch> TeacherBranches { get; set; }
    }

}
