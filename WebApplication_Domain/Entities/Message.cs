using System.ComponentModel.DataAnnotations;

namespace WebApplication_Domain.Entities
{
    public class Message
    {
        [Key]
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string MessageText { get; set; }
        public DateTime SentDate { get; set; }

        // Relationships
        public User Sender { get; set; }
        public User Receiver { get; set; }
    }
}
