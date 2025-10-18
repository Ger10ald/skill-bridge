using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillBridge.Core.Models;

/// <summary>
/// EF Core mapping for the Skill model
/// </summary>
namespace SkillBridge.Data.Configurations
{
    public sealed class UserSkillConfig : IEntityTypeConfiguration<UserSkill>
    {
        public void Configure(EntityTypeBuilder<UserSkill> b)
        {
            b.ToTable("UserSkills");

            // Composite PK
            b.HasKey(us => new { us.UserId, us.SkillId, us.IsOffering});

            // Foreign Keys
            b.HasOne(us => us.User)
             .WithMany(u => u.UserSkills)
             .HasForeignKey(us => us.UserId)
             .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(us => us.Skill)
             .WithMany(s => s.UserSkills)
             .HasForeignKey(us => us.SkillId)
             .OnDelete(DeleteBehavior.Restrict);
            
            // User's Proficiency level
            b.Property(us => us.ProficiencyLevel)
             .HasConversion<string>()
             .HasMaxLength(30)
             .IsRequired();

            b.Property(us => us.Description)
                .HasMaxLength(500)
                .IsRequired();

            b.Property(us => us.IsOffering)
                .IsRequired();

            b.HasIndex(us => new {us.SkillId, us.IsOffering});
            b.HasIndex(us => new { us.UserId, us.IsOffering });
            b.HasIndex(us => us.UserId);
        }
    }
}
