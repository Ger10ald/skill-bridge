using Microsoft.Extensions.Options;
using SkillBridge.Core.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Infrastructure.Security
{
    public sealed class PasswordHasher : IPasswordHasher
    {
        private readonly PasswordHashOptions _options;

        public PasswordHasher(IOptions<PasswordHashOptions> options)
        {
            _options = options.Value ?? throw new ArgumentNullException(nameof(options));
        }

        // Hash a new password 
        public HashPackage Hash(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password is required.", nameof(password));

            byte[] salt = RandomNumberGenerator.GetBytes(_options.SaltSize);
            byte[] hash = Derive(password, salt, _options.Iterations, _options.Pepper);

            return new HashPackage(hash, salt, _options.Iterations, _options.Algorithm, _options.Version);
        }

        // Verify a candidate password 
        public bool VerifyPassword(string password, in HashPackage package)
        {
            if (string.IsNullOrWhiteSpace(password)) return false;

            byte[] computed = Derive(password, package.Salt, package.Iterations, _options.Pepper);
            return CryptographicOperations.FixedTimeEquals(computed, package.Hash);
        }

        // Should we upgrade this hash?
        public bool NeedsRehash(in HashPackage package)
        {
            if (!string.Equals(package.Algorithm, _options.Algorithm, StringComparison.OrdinalIgnoreCase))
                return true;
            if (package.Version != _options.Version)
                return true;
            if (package.Iterations < _options.Iterations)
                return true;
            return false;
        }

        // Helper
        private static byte[] Derive(string password, byte[] salt, int iterations, string pepper)
        {
            var data = Encoding.UTF8.GetBytes(pepper + password);

            using var pbkdf2 = new Rfc2898DeriveBytes(data, salt, iterations, HashAlgorithmName.SHA256);
            return pbkdf2.GetBytes(32); // 256-bit hash
        }
    }

    // Options bound from configuration
    public sealed class PasswordHashOptions
    {
        public string Pepper { get; set; } = string.Empty;   
        public int Iterations { get; set; } = 310_000;
        public int SaltSize { get; set; } = 16;
        public string Algorithm { get; set; } = "PBKDF2-SHA256";
        public int Version { get; set; } = 1;
    }
}
