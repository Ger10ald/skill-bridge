using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillBridge.Core.Models;

namespace SkillBridge.Data.Configurations
{
    public sealed class SkillsRequestConfig : IEntityTypeConfiguration<SkillRequest>
    {
        public void Configure(EntityTypeBuilder<SkillRequest> b) 
        {
            b.ToTable("SkillsRequests");
            b.HasKey(r => r.Id);

            b.Property(x => x.ReferenceCode)
             .HasMaxLength(64)
             .IsRequired();
            b.HasIndex(x => x.ReferenceCode).IsUnique();

            // Price snapshot
            // or: .HasPrecision(18, 2)
            b.Property(x => x.PriceAmount)
             .HasColumnType("decimal(18,2)")  
             .IsRequired();

            b.Property(x => x.PriceCurrency)
             .HasMaxLength(3)
             .IsRequired();

            b.Property(r => r.Status)
                .IsRequired()
                .HasMaxLength(30)
                .HasConversion<int>();

            b.Property(r => r.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            b.Property(x => x.StartUtc).IsRequired();
            b.Property(x => x.EndUtc).IsRequired();

            b.Property(x => x.HoldUntil);
            b.Property(x => x.AcceptedAt);
            b.Property(x => x.CapturedAt);
            b.Property(x => x.DeclinedAt);
            b.Property(x => x.CanceledAt);
            b.Property(x => x.ExpiredAt);

            // Optimistic concurrency
            b.Property(x => x.RowVersion).IsRowVersion();

            b.HasOne(x => x.Requester)
             .WithMany()
             .HasForeignKey(x => x.RequesterId)
             .OnDelete(DeleteBehavior.Restrict);   // keep history if a user is removed

            b.HasOne(x => x.Skill)
             .WithMany()
             .HasForeignKey(x => x.SkillId)
             .OnDelete(DeleteBehavior.Restrict);

            b.HasIndex(x => x.RequesterId);
            b.HasIndex(x => x.SkillId);
            b.HasIndex(x => new { x.Status, x.StartUtc }); // dashboards: upcoming by status
            b.HasIndex(x => x.CreatedAt);

            b.HasIndex(r => new {r.RequesterId, r.Status });
            b.HasIndex(r => new { r.SkillId, r.Status });
        }
    }
}
