using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(u => u.SkillsOffered)
                .WithOne(s => s.User)
                .HasForeignKey(s => s.UserId);

            modelBuilder.Entity<User>()
                .HasMany(u => u.SkillRequests)
                .WithOne(r => r.Requester)
                .HasForeignKey(r => r.RequesterId);

            modelBuilder.Entity<User>()
                .HasMany(u => u.ReviewsWritten)
                .WithOne(r => r.Reviewer)
                .HasForeignKey(r => r.ReviewerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.ReviewsReceived)
                .WithOne(r => r.Reviewee)
                .HasForeignKey(r => r.RevieweeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.MessagesSent)
                .WithOne(m => m.Sender)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.MessagesReceived)
                .WithOne(m => m.Receiver)
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Skill>()
                .HasOne(s => s.Category)
                .WithMany(c => c.Skills)
                .HasForeignKey(s => s.CategoryId);

            modelBuilder.Entity<SkillRequest>()
                .HasOne(sr => sr.Skill)
                .WithMany(s => s.MatchedRequests)
                .HasForeignKey(sr => sr.SkillId);
        }
    }
}
