using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillBridge.Core.Models;

namespace SkillBridge.Data.Configurations
{
    public sealed class CategoryConfig : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> b)
        {
            b.ToTable("Categories");
            b.HasKey(c => c.Id);

            b.Property(c => c.Name)
             .IsRequired()
             .HasMaxLength(80);

            b.HasIndex(c => c.Name).IsUnique();
        }
    }
}
