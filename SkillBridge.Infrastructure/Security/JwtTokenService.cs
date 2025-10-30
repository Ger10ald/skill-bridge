using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SkillBridge.Core.Models;
using SkillBridge.Core.Security;

namespace SkillBridge.Infrastructure.Security
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly JwtOptions _opt;
        private readonly SigningCredentials _signingCreds;

        public JwtTokenService(IOptions<JwtOptions> options)
        {
            _opt = options.Value ?? throw new ArgumentNullException(nameof(options));
            if (string.IsNullOrWhiteSpace(_opt.Key) || _opt.Key.Length < 32)
                throw new InvalidOperationException("JwtOptions.Key must be at least 32 characters.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opt.Key));
            _signingCreds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        }

        public JwtTokenResult Issue(User user, IEnumerable<Claim>? extraClaims = null, DateTimeOffset? now = null)
        {
            ArgumentNullException.ThrowIfNull(user);
            var t0 = (now ?? DateTimeOffset.UtcNow);

            // Standard identity claims
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.UniqueName, user.UserName ?? user.Email),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                new(JwtRegisteredClaimNames.Iat, t0.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            if (!string.IsNullOrWhiteSpace(user.Role))
            {
                // You can add multiple roles later; for now a single role string.
                claims.Add(new Claim(ClaimTypes.Role, user.Role));
            }

            if (extraClaims is not null)
                claims.AddRange(extraClaims);

            var expires = t0.AddMinutes(_opt.ExpiryMinutes);

            var token = new JwtSecurityToken(
                issuer: _opt.Issuer,
                audience: _opt.Audience,
                claims: claims,
                notBefore: t0.UtcDateTime,
                expires: expires.UtcDateTime,
                signingCredentials: _signingCreds
            );

            string jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return new JwtTokenResult(jwt, expires.UtcDateTime);
        }
    }
}
