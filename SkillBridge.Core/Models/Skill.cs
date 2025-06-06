using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Core.Models
{
    public class Skill
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";

        public int UserId { get; set; }
        public User? User { get; set; }
        
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }

        public ICollection<SkillRequest> MatchedRequests { get; set; } = new List<SkillRequest>();

    }
}
