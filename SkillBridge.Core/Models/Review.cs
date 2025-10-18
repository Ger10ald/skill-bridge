using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SkillBridge.Core.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Required, MaxLength(120)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        public int Rating {  get; set; }
        
        [Required, MaxLength(1500)]
        public string Comment { get; set; } = string.Empty;
        
        public int ReviewerId { get; set; }
        public User? Reviewer { get; set; }
        
        public int RevieweeId { get; set; }
        public User? Reviewee { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
