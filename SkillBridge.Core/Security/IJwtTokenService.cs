using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using SkillBridge.Core.Models;

namespace SkillBridge.Core.Security
{
    public interface IJwtTokenService
    {
        JwtTokenResult Issue(User user, IEnumerable<Claim>? extraClaims = null, DateTimeOffset? now = null);
    }
    public readonly record struct JwtTokenResult(string AccessToken, DateTime ExpiresAtUtc);
}
