namespace LedgerApi.Domain.Events
{
    public class TransferRecordedEvent
    {
        public string FromAccountId { get; set; } = null!;
        public string ToAccountId { get; set; } = null!;
        public decimal Amount { get; set; }
        public string Reason { get; set; } = null!;
        public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    }
}
