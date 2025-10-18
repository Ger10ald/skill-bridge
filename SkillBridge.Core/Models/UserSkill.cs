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
        public User User { get; set; } = null!;

        public int SkillId { get; set; }
        public Skill Skill { get; set; } = null!;

        [Required, MaxLength(500)]
        public string Description { get; set; } = string.Empty;
        public ProficiencyLevel ProficiencyLevel { get; set; }
        public bool IsOffering { get; set; }
    }
}
