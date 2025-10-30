using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SkillBridge.Infrastructure.Security
{
    public sealed class JwtOptions
    {
        [Required] public string Issuer { get; set; } = "SkillBridge";
        [Required] public string Audience { get; set; } = "SkillBridge.Web";
        [Required, MinLength(32)] public string Key { get; set; } = string.Empty;
        [Range(1, 2 * 60)] public int ExpiryMinutes { get; set; } = 60;
        [Range(0, 10)] public int ClockSkewMinutes { get; set; } = 2;
    }
}
