namespace LedgerApi.Domain.Events
{
    public class MoneyWithdrawnEvent
    {
        public string AccountId { get; set; } = null!;
        public decimal Amount { get; set; }
        public string Reason { get; set; } = null!;
        public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    }
}
