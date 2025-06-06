using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Core.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";

        public ICollection<Skill> Skills { get; set; } = new List<Skill>();
    }
}
