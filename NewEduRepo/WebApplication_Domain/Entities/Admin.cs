using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebApplication_Domain.Entities
{
    public class Admin : IdentityUser<int>
    {
        [Key]
        public int AdminId { get; set; }

        [DisplayName("Kullanıcı Adı")]
        [Required(ErrorMessage = "Bu alan boş geçilemez!")]
        public string AdminName { get; set; }

        [DisplayName("Şifre")]
        [Required(ErrorMessage = "Bu alan boş geçilemez!")]
        [MinLength(8, ErrorMessage = "Şifre en az 8 karakter uzunluğunda olmalıdır.")]
        public string AdminPassword { get; set; }
    }
}
