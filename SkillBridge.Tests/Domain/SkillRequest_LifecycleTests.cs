using FluentAssertions;
using SkillBridge.Core.Enums;
using SkillBridge.Core.Models;
using Microsoft.Extensions.Time.Testing;
using System;
using Xunit;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace SkillBridge.Tests.Domain
{
    public class SkillRequest_LifecycleTests
    {
        
        // Helper that creates a valid new request with schedule + price
        private SkillRequest NewRequest(TimeProvider? tp = null)
        {
            var r = new SkillRequest(tp ?? TimeProvider.System);
            var now = (tp ?? TimeProvider.System).GetUtcNow().UtcDateTime;

            r.SetScheduleUtc(now.AddHours(1), now.AddHours(2));
            r.SetPriceSnapshot(50m, "USD");
            return r;
        }

        [Fact]
        public void HappyPath_Request_To_Captured()
        {
            var fake = new FakeTimeProvider(DateTimeOffset.Parse("2025-10-21T12:00:00Z"));
            var r = NewRequest(fake);
            var now = fake.GetUtcNow().UtcDateTime;

            r.PlaceHoldUntil(now.AddMinutes(30));
            r.Status.Should().Be(Core.Enums.Status.PendingHold, "PlaceHoldUntil must set status to PendingHold");

            r.Accept();
            r.Status.Should().Be(Core.Enums.Status.Accepted);
            r.AcceptedAt.Should().NotBeNull();

            fake.Advance(TimeSpan.FromHours(3));

            r.Capture();
            r.Status.Should().Be(Core.Enums.Status.Captured);
            r.CapturedAt.Should().NotBeNull();
        }

        [Fact]
        public void Cannot_Capture_Without_Accept()
        {
            var fake = new FakeTimeProvider(DateTimeOffset.Parse("2025-10-21T12:00:00Z"));
            var r = NewRequest(fake);
            var now = fake.GetUtcNow().UtcDateTime;

            Action act = () => r.Capture();
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("*Accepted*");
        }

        [Fact]
        public void Expire_When_Hold_Window_Passed()
        {
            var fake = new FakeTimeProvider(DateTimeOffset.Parse("2025-10-21T12:00:00Z"));
            var r = NewRequest(fake);
            var now = fake.GetUtcNow().UtcDateTime;

            r.PlaceHoldUntil(now.AddMinutes(5));
            r.Status.Should().Be(Status.PendingHold);

            // Advance time past hold
            fake.Advance(TimeSpan.FromMinutes(10));
           
            r.ExpireIfPastHold();

            r.Status.Should().Be(Core.Enums.Status.Expired);
            r.ExpiredAt.Should().NotBeNull();
        }

        [Fact]
        public void Cancel_Disallowed_After_Captured()
        {
            var fake = new FakeTimeProvider(DateTimeOffset.Parse("2025-10-21T12:00:00Z"));
            var r = NewRequest(fake);
            var now = fake.GetUtcNow().UtcDateTime;

            r.PlaceHoldUntil(now.AddMinutes(5));
            r.Accept();

            fake.Advance(TimeSpan.FromHours(3));

            r.Capture();

            Action act = () => r.CancelByRequester();
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("*after completion*");
        }

        [Fact]
        public void Accept_Disallowed_After_Expiry()
        {
            var fake = new FakeTimeProvider(DateTimeOffset.Parse("2025-10-21T12:00:00Z"));
            var r = NewRequest(fake);
            var now = fake.GetUtcNow().UtcDateTime;

            r.PlaceHoldUntil(now.AddMinutes(5));
            r.Status.Should().Be(Status.PendingHold);
            
            fake.Advance(TimeSpan.FromHours(1));

            Action act = () => r.Accept();
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("*expired*");
        }
    }
}
