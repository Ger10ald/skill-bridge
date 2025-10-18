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

            b.Property(r => r.Status)
                .IsRequired()
                .HasMaxLength(30)
                .HasConversion<string>();

            b.Property(r => r.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            b.HasIndex(r => new {r.RequesterId, r.Status });
            b.HasIndex(r => new { r.SkillId, r.Status });
        
        }
    }
}
