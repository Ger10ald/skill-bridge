using System;
using FluentAssertions;
using Microsoft.Extensions.Options;
using SkillBridge.Core.Enums;
using SkillBridge.Core.Security;
using SkillBridge.Infrastructure.Security;
using Xunit;


namespace SkillBridge.Tests.Security
{
    public class PasswordHasher_Tests
    {
        private static IPasswordHasher MakeHasher(string pepper = "test-pepper", int iterations = 200000, int version = 1, int saltSize = 16)
        {
            var opts = Options.Create(new PasswordHashOptions
            {
                Pepper = pepper,
                Iterations = iterations,
                Version = version,
                SaltSize = saltSize,
                Algorithm = "PBKDF2-SHA256"
            });
            return new PasswordHasher(opts);
        }

        [Fact]
        public void Hash_And_Verify_Succeeds()
        {
            var hasher = MakeHasher();
            var pkg = hasher.Hash("S3cret!");

            // Where would Hash even be stored in this case
            pkg.Hash.Should().NotBeNullOrEmpty();
            pkg.Salt.Should().NotBeNullOrEmpty();

            hasher.VerifyPassword("S3cret!", pkg).Should().BeTrue();
        }

        [Fact]
        public void Same_Password_Different_Salt()
        {
            var hasher = MakeHasher();

            var a = hasher.Hash("P@ssW1rd");
            var b = hasher.Hash("P@ssW1rd");

            a.Salt.Should().NotEqual(b.Salt);
            a.Hash.Should().NotEqual(b.Hash);
        }

        [Fact]
        public void Verify_Fails_For_WrongPassword()
        {
            var hasher = MakeHasher();
            var pkg = hasher.Hash("CorrectHorseBatteryStaple");
            hasher.VerifyPassword("wrong", pkg).Should().BeFalse();
        }

        [Fact]
        public void NeedsRehash_When_Iterations_Increase()
        {
            var oldHasher = MakeHasher(iterations: 100_000);
            var pkg = oldHasher.Hash("pw");

            var newHasher = MakeHasher(iterations: 300_000);
            newHasher.NeedsRehash(pkg).Should().BeTrue();
        }

        [Fact]
        public void NeedsRehash_When_Version_Changes()
        {
            var v1 = MakeHasher(version: 1);
            var pkg = v1.Hash("pw");

            var v2 = MakeHasher(version: 2);
            v2.NeedsRehash(pkg).Should().BeTrue();
        }

        [Fact]
        public void NeedsRehash_When_Algorithm_Changes()
        {
            var hasherA = MakeHasher();
            var pkg = hasherA.Hash("pw");

            // simulate switching algorithms by changing label in options
            var opts = Options.Create(new PasswordHashOptions
            {
                Pepper = "test-pepper",
                Iterations = 200_000,
                Version = 1,
                SaltSize = 16,
                Algorithm = "PBKDF2-SHA512" // different label than package
            });
            var hasherB = new PasswordHasher(opts);

            hasherB.NeedsRehash(pkg).Should().BeTrue();
        }
    }


}
