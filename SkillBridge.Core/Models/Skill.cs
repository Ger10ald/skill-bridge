using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using SkillBridge.Core.Enums;

namespace SkillBridge.Core.Models
{
    public class Skill
    {
        public int Id { get; set; }
        
        [Required]
        public string Title { get; set; } = string.Empty;

        public int? CategoryId { get; set; }
        public Category? Category { get; set; }

        public ICollection<UserSkill> UserSkills { get; set; } = new List<UserSkill>();

    }
}
