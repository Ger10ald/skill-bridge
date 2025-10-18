using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SkillBridge.Core.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required, MaxLength(80)] // Unique
        public string Name { get; set; } = string.Empty;

        public ICollection<Skill> Skills { get; set; } = new List<Skill>();
    }
}
