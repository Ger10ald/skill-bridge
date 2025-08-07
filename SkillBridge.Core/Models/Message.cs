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
        public int SenderId {  get; set; }
        public int ReceiverId { get; set; }
        
        [Required]
        public string Content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        public User? Sender { get; set; }
        public User? Receiver  { get; set; }
    }
}
