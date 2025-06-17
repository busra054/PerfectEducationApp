using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication_Domain.Entities
{
    public class ConversationViewModel
    {
        public User Partner { get; set; }
        public Message LastMessage { get; set; }
    }
}
