using SkillBridge.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Core.Models
{
    public class UserSkill
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int SkillId { get; set; }
        public Skill Skill { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;
        [Required]
        public ProficiencyLevel ProficiencyLevel { get; set; }
        [Required]
        public bool IsOffering { get; set; }
    }
}
