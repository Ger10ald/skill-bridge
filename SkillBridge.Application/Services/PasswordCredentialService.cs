using Microsoft.EntityFrameworkCore;
using SkillBridge.Core.Models;
using SkillBridge.Core.Security;
using SkillBridge.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Services
{
    public sealed class PasswordCredentialService
    {
        private readonly SkillBridgeDbContext _db;
        private readonly IPasswordHasher _hasher;
        private readonly TimeProvider _time;

        public PasswordCredentialService(
                SkillBridgeDbContext db,
                IPasswordHasher hasher,
                TimeProvider? time = null
            )
        {
            _db = db;
            _hasher = hasher;
            _time = time ?? TimeProvider.System;
        }

        public async Task SetOrReplaceAsync(int userId, string password, CancellationToken ct = default)
        {
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId));
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Password required.", nameof(password));

            var userExists = await _db.Users.AsNoTracking().AnyAsync(u => u.Id == userId, ct);
            if (!userExists) throw new InvalidOperationException($"User {userId} not found.");

            var active = await _db.Set<PasswordCredential>()
                                  .Where(p => p.UserId == userId && !p.IsRevoked)
                                  .SingleOrDefaultAsync(ct);
            if (active is not null)
                active.Revoke();

            var pkg = _hasher.Hash(password);

            var cred = new PasswordCredential(userId, _time);
            cred.SetMaterial(pkg.Hash, pkg.Salt, pkg.Iterations, pkg.Algorithm, pkg.Version, _time);

            _db.Add(cred);
            await _db.SaveChangesAsync(ct);
        }

        public async Task<VerifyPasswordResult> VerifyAsync(int userId, string password, CancellationToken ct = default)
        {
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId));
            if (string.IsNullOrWhiteSpace(password)) return VerifyPasswordResult.Fail();

            var cred = await _db.Set<PasswordCredential>()
                                .Where(p => p.UserId == userId && !p.IsRevoked)
                                .SingleOrDefaultAsync(ct);

            if (cred is null) return VerifyPasswordResult.Fail();

            var package = new HashPackage(cred.Hash, cred.Salt, cred.Iterations, cred.Algorithm, cred.Version);

            // 1) Verify
            if (!_hasher.VerifyPassword(password, package))
                return VerifyPasswordResult.Fail();

            // 2) Mark success time
            cred.MarkVerified(_time);

            // 3) Rotate if policy upgraded
            bool rotated = false;
            if (_hasher.NeedsRehash(package))
            {
                var upgraded = _hasher.Hash(password);
                cred.SetMaterial(upgraded.Hash, upgraded.Salt, upgraded.Iterations, upgraded.Algorithm, upgraded.Version, _time);
                rotated = true;
            }

            await _db.SaveChangesAsync(ct);
            return VerifyPasswordResult.Success(rotated);
        }

        public async Task RevokeActiveAsync(int userId, CancellationToken ct = default)
        {
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId));

            var cred = await _db.Set<PasswordCredential>()
                                .Where(p => p.UserId == userId && !p.IsRevoked)
                                .SingleOrDefaultAsync(ct);
            if (cred is null) return;

            cred.Revoke();
            await _db.SaveChangesAsync(ct);
        }
    }

    public readonly record struct VerifyPasswordResult(bool Succeeded, bool Rotated)
    {
        public static VerifyPasswordResult Fail() => new(false, false);
        public static VerifyPasswordResult Success(bool rotated) => new(true, rotated);
    }
}
