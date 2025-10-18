using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillBridge.Core.Models;

namespace SkillBridge.Data.Configurations
{
    public sealed class MessageConfig : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> b)
        {
            b.ToTable("Messages");
            b.HasKey(m => m.Id);

            // Columns (keep it light; mirror typical fields if present)
            b.Property(m => m.Content)
             .IsRequired()
             .HasMaxLength(4000); // adjust later if you adopt attachments

            b.Property(m => m.CreatedAt)
             .HasDefaultValueSql("GETUTCDATE()"); // Postgres: now() at time zone 'utc'

            b.Property(m => m.IsDeleted)
             .HasDefaultValue(false);

            // Optional performance index (enable when you’re ready)
            // b.HasIndex(m => new { m.ReceiverId, m.CreatedAt });
            // b.HasIndex(m => new { m.SenderId, m.CreatedAt });
        }
    }
}
