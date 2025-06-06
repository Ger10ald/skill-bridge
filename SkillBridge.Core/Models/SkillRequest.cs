using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Core.Models
{
    public class SkillRequest
    {
        public int Id { get; set; }
        public int RequesterId { get; set; }
        public int SkillId {  get; set; }

        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Pending";

        public User? Requester { get; set; }
        public Skill? Skill { get; set; }
    }
}
