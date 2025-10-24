using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillBridge.Core.Models;

namespace SkillBridge.Data.Configurations
{
    public sealed class PasswordCredentialConfig : IEntityTypeConfiguration<PasswordCredential>
    {
        public void Configure(EntityTypeBuilder<PasswordCredential> b)
        {
            b.ToTable("PasswordCredential");
            b.HasKey(p => p.Id);

            b.Property(p => p.Hash)
                .IsRequired()
                .HasMaxLength(32);

            b.Property(p => p.Salt)
                .IsRequired()
                .HasMaxLength(16);

            b.Property(p => p.Iterations).IsRequired();
            b.Property(p => p.Algorithm).IsRequired().HasMaxLength(40);
            b.Property(p => p.Version).IsRequired();

            b.Property(p => p.CreatedAt)
                .IsRequired();

            b.Property(p => p.UpdatedAt)
                .IsRequired();
            
            b.Property(p => p.LastVerifiedAt);
            b.Property(p => p.IsRevoked).IsRequired();

            b.HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasIndex(p => p.UserId);
            b.HasIndex(x => new { x.UserId, x.IsRevoked })
                .IsUnique()
                .HasFilter("IsRevoked = 0");
        }
    }
}
