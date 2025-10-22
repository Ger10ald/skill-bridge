using System;               
using System.Linq;          
using Microsoft.EntityFrameworkCore;
using SkillBridge.Core.Models;

namespace SkillBridge.Data
{
    public class SkillBridgeDbContext : DbContext
    {
        public SkillBridgeDbContext(DbContextOptions<SkillBridgeDbContext> options)
            : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Skill> Skills => Set<Skill>();
        public DbSet<SkillRequest> SkillRequests => Set<SkillRequest>();
        public DbSet<Review> Reviews => Set<Review>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Message> Messages => Set<Message>();
        public DbSet<UserSkill> UserSkills => Set<UserSkill>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SkillBridgeDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            StampConcurrencyTokens();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            StampConcurrencyTokens();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void StampConcurrencyTokens()
        {
            // Any unique-changing value works; Guid bytes are simple and small
            var entries = ChangeTracker.Entries<SkillRequest>()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var e in entries)
            {
                e.Property(x => x.RowVersion).CurrentValue = Guid.NewGuid().ToByteArray();
            }
        }
    }
}
