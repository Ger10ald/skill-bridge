using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Core.Models
{
    public sealed class PasswordCredential
    {
        public int Id { get; private set; }

        public int UserId { get; private set; }
        public User? User { get; private set; }

        // Plaintext isn't stored here 
        public byte[] Hash { get; private set; } = Array.Empty<byte>();
        public byte[] Salt { get; private set; } = Array.Empty<byte>();

        public int Iterations { get; private set; }
        public string Algorithm {  get; private set; }
        public int Version { get; private set; } // Version for updates
        
        public DateTime CreatedAt { get; private set; } 
        public DateTime UpdatedAt { get; private set; } 
        public DateTime? LastVerifiedAt { get; private set; } // Soft-disable
        public bool IsRevoked {  get; private set; }

        private PasswordCredential() { }

        public PasswordCredential(int userId, TimeProvider? time=null)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(userId);
            UserId = userId;

            var now = (time ?? TimeProvider.System).GetUtcNow().UtcDateTime;
            CreatedAt = now;
            UpdatedAt = now;
        }

        public void SetMaterial(byte[] hash, byte [] salt, int iterations, string algorithm, int version, TimeProvider? time = null)
        {
            if (hash is null || hash.Length < 16) throw new ArgumentException("Hash must be at least 16 bytes.", nameof(hash));
            if (salt is null || salt.Length < 8) throw new ArgumentException("Salt must be at least 8 bytes.", nameof(salt));
            if (iterations <= 0) throw new ArgumentOutOfRangeException(nameof(iterations), "Iterations must be positive.");
            if (string.IsNullOrWhiteSpace(algorithm)) throw new ArgumentException("Algorithm label is required.", nameof(algorithm));
            if (version <= 0) throw new ArgumentOutOfRangeException(nameof(version), "Version must be positive.");

            Hash = (byte[])hash.Clone();
            Salt = (byte[])salt.Clone();
            Iterations = iterations;
            Algorithm = algorithm.Trim();
            Version = version;

            var now = (time ?? TimeProvider.System).GetUtcNow().UtcDateTime;
            UpdatedAt = now;
            if (CreatedAt == default) CreatedAt = now;

            IsRevoked = false;
        }

        public void MarkVerified(TimeProvider? time = null)
        {
            LastVerifiedAt = (time ?? TimeProvider.System).GetUtcNow().UtcDateTime;
        }

        public void Revoke()
        {
            IsRevoked = true;
        }

        public bool NeedsRehash(int targetIterations, int targetVersion, string targetAlgorithm)
        {
            if (targetIterations <= 0) throw new ArgumentOutOfRangeException(nameof(targetIterations));
            if (targetVersion <= 0) throw new ArgumentOutOfRangeException(nameof(targetVersion));
            if (string.IsNullOrWhiteSpace(targetAlgorithm)) throw new ArgumentException("Target algorithm required.", nameof(targetAlgorithm));

            if (!string.Equals(Algorithm, targetAlgorithm, StringComparison.OrdinalIgnoreCase)) return true;
            if (Version != targetVersion) return true;
            if (Iterations < targetIterations) return true;

            return false;
        }
    }

}
