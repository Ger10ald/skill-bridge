using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Core.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int ReviewerId { get; set; }
        public int RevieweeId { get; set; }

        public int Rating {  get; set; }
        public string Comment { get; set; } = "";
        public User? Reviewer { get; set; }
        public User? Reviewee { get; set; }
    }
}
