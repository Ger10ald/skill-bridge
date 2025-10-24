using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Core.Security
{
    public interface IPasswordHasher
    {
        // Hash the plaintext password using the current policy
        HashPackage Hash(string password);

        // Verify plaintext password against an existing hash package
        bool VerifyPassword(string password, in HashPackage package);

        // Returns true if the stored package should be upgraded
        bool NeedsRehash(in HashPackage package);
    }
    
    public readonly record struct HashPackage(
        byte[] Hash,
        byte[] Salt,
        int Iterations,
        string Algorithm,
        int Version
    )

    {
        public void Deconstruct(out byte[] hash, out byte[] salt, out int iterations, out string algorithm, out int version)
        {
            hash = Hash;
            salt = Salt;
            iterations = Iterations;
            algorithm = Algorithm;
            version = Version;
        }
    }
}
