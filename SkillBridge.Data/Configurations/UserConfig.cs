using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillBridge.Core.Models;

namespace SkillBridge.Data.Configurations
{
    public sealed class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> b)
        {
            b.ToTable("Users");
            b.HasKey(u => u.Id);

            b.Property(u => u.UserName).IsRequired().HasMaxLength(80);
            b.Property(u => u.Email).IsRequired().HasMaxLength(254); 
            b.Property(u => u.Bio).HasMaxLength(700);
            b.Property(u => u.Role).HasMaxLength(40);
            b.Property(u => u.CreatedAt)
             .HasDefaultValueSql("GETUTCDATE()");

            b.HasIndex(u => u.Email).IsUnique();
            b.HasIndex(u => u.UserName).IsUnique();

            b.HasMany(u => u.SkillRequests)
             .WithOne(r => r.Requester)
             .HasForeignKey(r => r.RequesterId);

            b.HasMany(u => u.ReviewsWritten)
             .WithOne(r => r.Reviewer)
             .HasForeignKey(r => r.ReviewerId)
             .OnDelete(DeleteBehavior.Restrict);

            b.HasMany(u => u.ReviewsReceived)
             .WithOne(r => r.Reviewee)
             .HasForeignKey(r => r.RevieweeId)
             .OnDelete(DeleteBehavior.Restrict);

            b.HasMany(u => u.MessagesSent)
             .WithOne(m => m.Sender)
             .HasForeignKey(m => m.SenderId)
             .OnDelete(DeleteBehavior.Restrict);

            b.HasMany(u => u.MessagesReceived)
             .WithOne(m => m.Receiver)
             .HasForeignKey(m => m.ReceiverId)
             .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
