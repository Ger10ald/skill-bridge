using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillBridge.Core.Models;

namespace SkillBridge.Data.Configurations
{
    public sealed class ReviewConfig : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> b)
        {
            // Table & key
            b.ToTable("Reviews");
            b.HasKey(r => r.Id);

            b.Property(r => r.Title)
             .IsRequired()
             .HasMaxLength(120);

            b.Property(r => r.Comment)
             .IsRequired()
             .HasMaxLength(1500);

            b.Property(r => r.Rating)
             .IsRequired();

            // CreatedAt: ensure DB assigns UTC when app forgets
            b.Property(r => r.CreatedAt)
             .HasDefaultValueSql("GETUTCDATE()"); 

            b.HasOne(r => r.Reviewer)
             .WithMany(u => u.ReviewsWritten)
             .HasForeignKey(r => r.ReviewerId)
             .OnDelete(DeleteBehavior.Restrict)
             .IsRequired();

            b.HasOne(r => r.Reviewee)
             .WithMany(u => u.ReviewsReceived)
             .HasForeignKey(r => r.RevieweeId)
             .OnDelete(DeleteBehavior.Restrict)
             .IsRequired();

            b.HasIndex(r => new { r.RevieweeId, r.CreatedAt }); // show recent reviews for a user
            b.HasIndex(r => new { r.ReviewerId, r.CreatedAt }); // reviewer history

            // Postgres
            b.ToTable(tb => tb.HasCheckConstraint("CK_Reviews_Rating_1_5", "\"Rating\" BETWEEN 1 AND 5"));
            // SQL version would be: tb.HasCheckConstraint("CK_Reviews_Rating_1_5", "[Rating] BETWEEN 1 AND 5");
        }
    }
}
