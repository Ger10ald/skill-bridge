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
    }
}
