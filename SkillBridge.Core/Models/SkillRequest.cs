using SkillBridge.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Core.Models
{
    public class SkillRequest
    {
        public int Id { get; set; }

        public int RequesterId { get; set; }
        public User? Requester { get; set; }

        public int SkillId {  get; set; }
        public Skill? Skill { get; set; }
        
        // Concrete session window 
        public DateTime StartUtc { get; private set; }
        public DateTime EndUtc { get; private set; }
        //public DateTime? ActualStartedAt { get; private set; }
        //public DateTime? ActualEndedAt {  get; private set; }

        public decimal PriceAmount { get; private set; }
        public string PriceCurrency { get; private set; } = "USD";

        [Required, MaxLength(30)]
        public Status Status { get; private set; } = Status.Requested;

        [Required]
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? HoldUntil { get; private set; }
        public DateTime? AcceptedAt { get; private set; }
        public DateTime? CapturedAt { get; private set; }
        public DateTime? DeclinedAt { get; private set; }
        public DateTime? CanceledAt { get; private set; }
        public DateTime? ExpiredAt { get; private set; }

        public byte[]? RowVersion { get; set; }
        public string ReferenceCode { get; private set; } = $"REQ-{Guid.NewGuid():N}".ToUpperInvariant();
           
        private static void GuardTimeRange(DateTime startUtc, DateTime endUtc)
        {
            if (startUtc.Kind != DateTimeKind.Utc || endUtc.Kind != DateTimeKind.Utc)
                throw new ArgumentException("StartUtc/EndUtc must be UTC.");
            if (endUtc <= startUtc)
                throw new ArgumentException("EndUtc must be after StartUtc.");
        }

        public void SetScheduleUtc(DateTime startUtc, DateTime endUtc)
        {
            GuardTimeRange(startUtc, endUtc);
            if (Status != Status.Requested)
                throw new InvalidOperationException("Schedule can only be set while Requested");
            StartUtc = startUtc;
            EndUtc = endUtc;
        }

        public void SetPriceSnapshot(decimal amount, string currency)
        {
            if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount), "Price cannot be negative.");
            if (string.IsNullOrWhiteSpace(currency) || currency.Trim().Length != 3)
                throw new ArgumentException("Currency must be a 3-letter ISO code.", nameof(currency));

            PriceAmount = decimal.Round(amount, 2, MidpointRounding.AwayFromZero);
            PriceCurrency = currency.Trim().ToUpperInvariant();
        }
        public void PlaceHoldUntil(DateTime holdUntilUtc)
        {
            if (holdUntilUtc.Kind != DateTimeKind.Utc)
                throw new ArgumentException("HoldUntil must be UTC.");
            if (Status != Status.Requested)
                throw new InvalidOperationException("Only Requested can move to PendingHold.");
            if (holdUntilUtc <= DateTime.UtcNow)
                throw new ArgumentException("HoldUntil must be in the future.");

            Status = Status.PendingHold;
            HoldUntil = holdUntilUtc;
        }

        public void Accept()
        {
            if (Status != Status.PendingHold && Status != Status.Requested)
                throw new InvalidOperationException("Can only accept from Requested or PendingHold.");
            if (IsExpiredByTime())
                throw new InvalidOperationException("Cannot accept an expired request.");

            Status = Status.Accepted;
            AcceptedAt = DateTime.UtcNow;
        }

        public void Capture()
        {
            if (Status != Status.Accepted)
                throw new InvalidOperationException("Capture requires Accepted status.");
            if (DateTime.UtcNow < EndUtc)
                // This would capture the funds right after the service has been provided
                throw new InvalidOperationException("Cannot capture before end of session");
            Status = Status.Captured;
            CapturedAt = DateTime.UtcNow;
        }

        public void Decline()
        {
            if (Status == Status.Captured)
                throw new InvalidOperationException("Cannot decline after capture.");
            if (Status == Status.Declined)
                return; // idempotent
            Status = Status.Declined;
            DeclinedAt = DateTime.UtcNow;
        }

        public void CancelByRequester()
        {
            if (Status is Status.Captured or Status.Declined or Status.Expired)
                throw new InvalidOperationException("Cannot cancel after completion/decline/expiry.");
            Status = Status.Canceled;
            CanceledAt = DateTime.UtcNow;
        }

        public void ExpireIfPastHold()
        {
            if (Status is Status.Captured or Status.Declined or Status.Canceled or Status.Expired)
                return;

            if (IsExpiredByTime())
            {
                Status = Status.Expired;
                ExpiredAt = DateTime.UtcNow;
            }
        }

        private bool IsExpiredByTime()
        {
            if (HoldUntil.HasValue && DateTime.UtcNow > HoldUntil.Value) return true;
            if (DateTime.UtcNow > EndUtc) return true; // optional: also expire when session window passed
            return false;
        }
    }
}
