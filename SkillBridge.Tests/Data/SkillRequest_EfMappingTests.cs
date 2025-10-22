using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Time.Testing;
using SkillBridge.Core.Enums; 
using SkillBridge.Core.Models;
using SkillBridge.Data;
using SkillBridge.Tests.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Xunit;

namespace SkillBridge.Tests.Data
{
    public class SkillRequest_EfMappingTests
    {
        private SkillBridgeDbContext NewContext(DbContextOptions<SkillBridgeDbContext> opts)
            => new SkillBridgeDbContext(opts);

        private static User MakeTestUser(string? tag = null)
        {
            var unique = tag ?? Guid.NewGuid().ToString("N").Substring(0, 8);

            return new User
            {
                UserName = $"test_user_{unique}",
                FirstName = "Test",
                LastName = "User",
                Email = $"test_{unique}@example.com",
             
                PasswordHash = new byte[] { 1, 2, 3, 4, 5 },
                PasswordSalt = new byte[] { 9, 8, 7, 6, 5 },
            };
        }

        private SkillRequest SeedOne(SkillBridgeDbContext db, FakeTimeProvider time)
        {
            var user = MakeTestUser("sr");
            db.Add(user);

            
            var skill = new Skill("Math Tutoring");   
            db.Add(skill);

            db.SaveChanges(); // get IDs

            var req = new SkillRequest(time)
            {
                RequesterId = user.Id,
                SkillId = skill.Id
            };

            var now = time.GetUtcNow().UtcDateTime;
            req.SetScheduleUtc(now.AddHours(1), now.AddHours(2));
            req.SetPriceSnapshot(99.99m, "USD");

            db.SkillRequests.Add(req);
            db.SaveChanges();

            return req;
        }

        [Fact]
        public void Roundtrip_Persists_Core_Fields()
        {
            using var scope = EfTestHelpers.CreateSqliteInMemory<SkillBridgeDbContext>(opts => new SkillBridgeDbContext(opts));
            var (conn, db) = scope;

            var fake = new FakeTimeProvider(DateTimeOffset.Parse("2025-10-21T10:00:00Z"));
            var r = SeedOne(db, fake);

            var loaded = db.SkillRequests.AsNoTracking().Single(x => x.Id == r.Id);

            loaded.PriceAmount.Should().Be(99.99m);
            loaded.PriceCurrency.Should().Be("USD");
            loaded.Status.Should().Be(Status.Requested);
            loaded.ReferenceCode.Should().NotBeNullOrWhiteSpace();
            loaded.RowVersion.Should().NotBeNull(); // rowversion is set after insert
            loaded.StartUtc.Should().Be(r.StartUtc);
            loaded.EndUtc.Should().Be(r.EndUtc);
        }

        [Fact]
        public async Task Concurrency_RowVersion_Conflicts_On_Race()
        {
            using var scope = EfTestHelpers.CreateSqliteInMemory<SkillBridgeDbContext>(opts => new SkillBridgeDbContext(opts)); ;
            var (conn, db) = scope;

            var fake = new FakeTimeProvider(DateTimeOffset.Parse("2025-10-21T10:00:00Z"));
            var r = SeedOne(db, fake);

            // Create two separate contexts sharing the same open connection
            var opts = new DbContextOptionsBuilder<SkillBridgeDbContext>().UseSqlite(conn).Options;
            await using var db1 = new SkillBridgeDbContext(opts);
            await using var db2 = new SkillBridgeDbContext(opts);

            var a = await db1.SkillRequests.FindAsync(r.Id);
            var b = await db2.SkillRequests.FindAsync(r.Id);

            a!.SetPriceSnapshot(120m, "USD");
            await db1.SaveChangesAsync(); // first writer wins, updates RowVersion

            b!.SetPriceSnapshot(80m, "USD");
            Func<Task> act = async () => await db2.SaveChangesAsync();

            await act.Should().ThrowAsync<DbUpdateConcurrencyException>();
        }
    }
}

