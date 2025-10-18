using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SkillBridge.Core.Models
{
    public class Message
    {
        public int Id { get; set; }
        
        [Required, MaxLength(4000)]
        public string Content { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public int SenderId { get; set; }
        public User? Sender { get; set; }

        
        public int ReceiverId { get; set; }
        public User? Receiver  { get; set; }

        public bool IsDeleted { get; set; }
    }
}
