using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication_Domain.Entities
{
    public class TeacherBranch
    {
        [Key]
        public int Id { get; set; }

        public int TeacherId { get; set; }
        public int BranchId { get; set; }

        [ForeignKey("BranchId")]
        public Branch Branch { get; set; }

        [ForeignKey("TeacherId")]
        public Teacher Teacher { get; set; } // User değil Teacher


    }
}
