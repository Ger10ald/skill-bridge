using SkillBridge.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Core.Models
{
    public class SkillRequest
    {
        public int Id { get; set; }

        public int RequesterId { get; set; }
        public User? Requester { get; set; }

        public int SkillId {  get; set; }
        public Skill? Skill { get; set; }
        
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Required, MaxLength(30)]
        public Status Status { get; set; }
    }
}
