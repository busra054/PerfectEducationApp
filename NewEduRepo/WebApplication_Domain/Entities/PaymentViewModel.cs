using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication_Domain.Entities
{
    public class PaymentViewModel
    {
        public int PackageId { get; set; }
        public string PackageName { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(16, MinimumLength = 16)]
        [Display(Name = "Kart Numarası")]
        public string CardNumber { get; set; }

        [Required]
        [Range(1, 12)]
        [Display(Name = "Son Kullanma Ayı")]
        public int ExpiryMonth { get; set; }

        [Required]
        [Range(2023, 2100)]
        [Display(Name = "Son Kullanma Yılı")]
        public int ExpiryYear { get; set; }

        [Required]
        [Range(100, 999)]
        [Display(Name = "CVC")]
        public int CVC { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Kart Sahibi Adı")]
        public string CardHolderName { get; set; }
    }
}
