using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Time.Testing;
using SkillBridge.Core.Models;
using SkillBridge.Data;
using SkillBridge.Tests.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SkillBridge.Tests.Data
{
    public class PasswordCredential_EfTest
    {
        private static User MakeTestUser(string tag = "pc")
        {
            return new User
            {
                UserName = $"user_{tag}_{Guid.NewGuid():N}".Substring(0, 20),
                FirstName = "Test",
                LastName = "User",
                Email = $"test_{Guid.NewGuid():N}@example.com",
                PasswordHash = new byte[] { 1, 2, 3, 4 },
                PasswordSalt = new byte[] { 9, 8, 7, 6 }
            };
        }

        private static PasswordCredential MakeActiveCred(int userId, FakeTimeProvider? time = null)
        {
            var tp = time ?? new FakeTimeProvider(DateTimeOffset.UtcNow);
            var pc = new PasswordCredential(userId, tp);
            // Minimal but valid material: 32-byte hash, 16-byte salt
            pc.SetMaterial(
                hash: new byte[32] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                salt: new byte[16] { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 },
                iterations: 310_000,
                algorithm: "PBKDF2-SHA256",
                version: 1,
                time: tp
            );
            return pc; // IsRevoked defaults to false (active)
        }

        [Fact]
        public async Task Roundtrip_Persists_Basics()
        {
            using var scope = EfTestHelpers.CreateSqliteInMemory<SkillBridgeDbContext>(opts => new SkillBridgeDbContext(opts));
            var (_, db) = scope;

            var user = MakeTestUser("rt");
            db.Users.Add(user);
            await db.SaveChangesAsync();

            var fake = new FakeTimeProvider(DateTimeOffset.Parse("2025-10-22T12:00:00Z"));
            var t0 = fake.GetUtcNow().UtcDateTime;

            var cred = MakeActiveCred(user.Id, fake);
            db.Set<PasswordCredential>().Add(cred);
            await db.SaveChangesAsync();

            var loaded = await db.Set<PasswordCredential>().AsNoTracking().SingleAsync(x => x.Id == cred.Id);
            loaded.UserId.Should().Be(user.Id);
            loaded.Hash.Should().HaveCount(32);
            loaded.Salt.Should().HaveCount(16);
            loaded.Algorithm.Should().Be("PBKDF2-SHA256");
            loaded.Iterations.Should().Be(310_000);
            loaded.Version.Should().Be(1);
            loaded.IsRevoked.Should().BeFalse();

            loaded.CreatedAt.Should().BeCloseTo(t0, TimeSpan.FromSeconds(1));
            loaded.UpdatedAt.Should().BeOnOrAfter(loaded.CreatedAt);
        }

        [Fact]
        public async Task Enforces_One_Active_Credential_Per_User()
        {
            using var scope = EfTestHelpers.CreateSqliteInMemory<SkillBridgeDbContext>(opts => new SkillBridgeDbContext(opts));
            var (_, db) = scope;

            var user = MakeTestUser("one");
            db.Users.Add(user);
            await db.SaveChangesAsync();

            // First active credential
            db.Set<PasswordCredential>().Add(MakeActiveCred(user.Id));
            await db.SaveChangesAsync();

            // Second active credential for the same user should violate the filtered unique index
            db.Set<PasswordCredential>().Add(MakeActiveCred(user.Id));
            Func<Task> act = async () => await db.SaveChangesAsync();

            await act.Should().ThrowAsync<DbUpdateException>();
        }

        [Fact]
        public async Task Allows_Another_Credential_After_Revoking_Current()
        {
            using var scope = EfTestHelpers.CreateSqliteInMemory<SkillBridgeDbContext>(opts => new SkillBridgeDbContext(opts));
            var (_, db) = scope;

            var user = MakeTestUser("rev");
            db.Users.Add(user);
            await db.SaveChangesAsync();

            var active = MakeActiveCred(user.Id);
            db.Set<PasswordCredential>().Add(active);
            await db.SaveChangesAsync();
          
            active.Revoke();
            await db.SaveChangesAsync(); 
        
            var replacement = MakeActiveCred(user.Id);
            db.Set<PasswordCredential>().Add(replacement);
            await db.SaveChangesAsync(); 

            var total = await db.Set<PasswordCredential>().CountAsync(x => x.UserId == user.Id);
            var activeCount = await db.Set<PasswordCredential>().CountAsync(x => x.UserId == user.Id && !x.IsRevoked);
            total.Should().Be(2);
            activeCount.Should().Be(1);
        }
    }
}

