using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillBridge.Core.Models;

namespace SkillBridge.Data.Configurations
{
    /// <summary>
    /// EF Core mapping for the Skill model
    /// </summary>
    public sealed class SkillConfig : IEntityTypeConfiguration<Skill>
    {
        public void Configure(EntityTypeBuilder<Skill> b)
        {
            b.ToTable("Skills");
            
            b.HasKey(s => s.Id);
            
            b.Property(s => s.Title)
             .IsRequired()
             .HasMaxLength(80);
            
            b.Property(s => s.CategoryId)
             .IsRequired(false);

            // Relationship: Skill -> Category (many skills per category)
            b.HasOne(s => s.Category)
             .WithMany(c => c.Skills)          // ← change to .WithMany() if you don't have nav on Category
             .HasForeignKey(s => s.CategoryId)
             .OnDelete(DeleteBehavior.Restrict); // avoid cascading deletes wiping lots of skills by accident

            // Uniqueness: prevent duplicate titles within the same category.
            b.HasIndex(s => new { s.Title, s.CategoryId })
             .IsUnique()
             .HasFilter("[CategoryId] IS NOT NULL");
        }
    }
}

