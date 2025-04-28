using System.ComponentModel.DataAnnotations;

namespace WebApplication_Domain.Entities
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Ad soyad alanı zorunludur.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "E-posta alanı zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre alanı zorunludur.")]
        [StringLength(100, ErrorMessage = "Şifre en az {2} karakter uzunluğunda olmalıdır.", MinimumLength = 6)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Şifreyi onaylayın.")]
        [Compare("Password", ErrorMessage = "Şifreler eşleşmiyor.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Rol seçimi zorunludur.")]
        public string Role { get; set; }
    }
}
