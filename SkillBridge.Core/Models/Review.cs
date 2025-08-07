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
        public int ReviewerId { get; set; }
        public int RevieweeId { get; set; }

        [Required]
        public int Rating {  get; set; }
        [Required]
        public string Comment { get; set; } = string.Empty;
        public User? Reviewer { get; set; }
        public User? Reviewee { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
