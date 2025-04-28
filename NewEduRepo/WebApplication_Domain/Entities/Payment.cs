using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication_Domain.Entities
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }

        public int? StudentId { get; set; }
        [ForeignKey("StudentId")]
        public Student Student { get; set; }

        public int PackageId { get; set; }

        [ForeignKey("PackageId")]
        public Package Package { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Amount must be a positive value.")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(16, MinimumLength = 16, ErrorMessage = "Card number must be 16 digits.")]
        public string CardNumber { get; set; }

        [Required]
        [Range(1, 12, ErrorMessage = "Invalid month.")]
        public int ExpiryMonth { get; set; }

        [Required]
        [Range(2023, 2100, ErrorMessage = "Invalid year.")]
        public int ExpiryYear { get; set; }

        [Required]
        [Range(100, 999, ErrorMessage = "CVC must be 3 digits.")]
        public int CVC { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Card holder name cannot exceed 50 characters.")]
        public string CardHolderName { get; set; }
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    }

    public enum PaymentStatus
    {
        Pending,
        Completed,
        Failed
    }
}
