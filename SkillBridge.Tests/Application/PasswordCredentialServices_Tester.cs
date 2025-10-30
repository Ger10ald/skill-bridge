using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Time.Testing;
using SkillBridge.Application.Services;
using SkillBridge.Core.Models;
using SkillBridge.Core.Security;
using SkillBridge.Data;
using SkillBridge.Infrastructure.Security;
using SkillBridge.Tests.Data;
using Xunit;

namespace SkillBridge.Tests.Application
{
    public class PasswordCredentialService_Tests
    {
        private static User MakeUser()
        {
            return new User
            {
                UserName = $"user_{Guid.NewGuid():N}".Substring(0, 16),
                Email = $"{Guid.NewGuid():N}@example.com",
                PasswordHash = new byte[] { 1, 2, 3, 4 }, // satisfy Required on legacy columns
                PasswordSalt = new byte[] { 4, 3, 2, 1 }
            };
        }

        private static IPasswordHasher MakeHasher(string pepper = "test-pepper", int iterations = 200_000, int version = 1, int saltSize = 16, string algo = "PBKDF2-SHA256")
        {
            var opts = Options.Create(new PasswordHashOptions
            {
                Pepper = pepper,
                Iterations = iterations,
                Version = version,
                SaltSize = saltSize,
                Algorithm = algo
            });
            return new PasswordHasher(opts);
        }

        [Fact]
        public async Task Create_New_Credential()
        {
            using var scope = EfTestHelpers.CreateSqliteInMemory<SkillBridgeDbContext>(o => new SkillBridgeDbContext(o));
            var (_, db) = scope;

            var user = MakeUser();
            db.Users.Add(user);
            await db.SaveChangesAsync();

            var fake = new FakeTimeProvider(DateTimeOffset.Parse("2025-10-22T12:00:00Z"));
            var svc = new PasswordCredentialService(db, MakeHasher(), fake);

            await svc.SetOrReplaceAsync(user.Id, "Pa$$w0rd!");

            var creds = await db.Set<PasswordCredential>().Where(c => c.UserId == user.Id).ToListAsync();
            creds.Should().HaveCount(1);
            creds[0].IsRevoked.Should().BeFalse();
            creds[0].Hash.Should().HaveCount(32);
            creds[0].Salt.Should().HaveCount(16);
            creds[0].CreatedAt.Should().BeCloseTo(fake.GetUtcNow().UtcDateTime, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public async Task Update_Existing_Credential_Revokes_Old_And_Keeps_One_Active()
        {
            using var scope = EfTestHelpers.CreateSqliteInMemory<SkillBridgeDbContext>(o => new SkillBridgeDbContext(o));
            var (_, db) = scope;

            var user = MakeUser();
            db.Users.Add(user);
            await db.SaveChangesAsync();

            var fake = new FakeTimeProvider(DateTimeOffset.UtcNow);
            var svc = new PasswordCredentialService(db, MakeHasher(), fake);

            await svc.SetOrReplaceAsync(user.Id, "Old#Pass1");
            await svc.SetOrReplaceAsync(user.Id, "New#Pass2");

            var all = await db.Set<PasswordCredential>()
                              .Where(c => c.UserId == user.Id)
                              .OrderBy(c => c.Id)
                              .ToListAsync();

            all.Should().HaveCount(2);
            all.Count(c => !c.IsRevoked).Should().Be(1);
            all.First().IsRevoked.Should().BeTrue();
            all.Last().IsRevoked.Should().BeFalse();
        }

        [Fact]
        public async Task Verify_Success()
        {
            using var scope = EfTestHelpers.CreateSqliteInMemory<SkillBridgeDbContext>(o => new SkillBridgeDbContext(o));
            var (_, db) = scope;

            var user = MakeUser();
            db.Users.Add(user);
            await db.SaveChangesAsync();

            var fake = new FakeTimeProvider(DateTimeOffset.UtcNow);
            var svc = new PasswordCredentialService(db, MakeHasher(), fake);

            await svc.SetOrReplaceAsync(user.Id, "CorrectHorseBatteryStaple");

            var result = await svc.VerifyAsync(user.Id, "CorrectHorseBatteryStaple");
            result.Succeeded.Should().BeTrue();
            result.Rotated.Should().BeFalse();

            var active = await db.Set<PasswordCredential>().SingleAsync(c => c.UserId == user.Id && !c.IsRevoked);
            active.LastVerifiedAt.Should().NotBeNull();
        }

        [Fact]
        public async Task Verify_Failure_Wrong_Password()
        {
            using var scope = EfTestHelpers.CreateSqliteInMemory<SkillBridgeDbContext>(o => new SkillBridgeDbContext(o));
            var (_, db) = scope;

            var user = MakeUser();
            db.Users.Add(user);
            await db.SaveChangesAsync();

            var svc = new PasswordCredentialService(db, MakeHasher());
            await svc.SetOrReplaceAsync(user.Id, "Secret#123");

            var result = await svc.VerifyAsync(user.Id, "WRONG");
            result.Succeeded.Should().BeFalse();
            result.Rotated.Should().BeFalse();
        }

        [Fact]
        public async Task Verify_Rotates_When_Policy_Upgrades()
        {
            using var scope = EfTestHelpers.CreateSqliteInMemory<SkillBridgeDbContext>(o => new SkillBridgeDbContext(o));
            var (_, db) = scope;

            var user = MakeUser();
            db.Users.Add(user);
            await db.SaveChangesAsync();

            var fake = new FakeTimeProvider(DateTimeOffset.UtcNow);

            // Create credential with lower iterations (old policy)
            var oldSvc = new PasswordCredentialService(db, MakeHasher(iterations: 100_000), fake);
            await oldSvc.SetOrReplaceAsync(user.Id, "RotateMe!");

            // Verify under stronger policy -> should rotate
            var newSvc = new PasswordCredentialService(db, MakeHasher(iterations: 300_000), fake);
            var result = await newSvc.VerifyAsync(user.Id, "RotateMe!");

            result.Succeeded.Should().BeTrue();
            result.Rotated.Should().BeTrue();

            var active = await db.Set<PasswordCredential>().SingleAsync(c => c.UserId == user.Id && !c.IsRevoked);
            active.Iterations.Should().Be(300_000);
        }
    }
}
