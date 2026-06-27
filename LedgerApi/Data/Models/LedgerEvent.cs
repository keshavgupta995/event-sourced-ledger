namespace LedgerApi.Data.Models
{
    public class LedgerEvent
    {
        public int Id { get; set; }
        public string AccountId { get; set; } = null!;
        public string EventType { get; set; } = null!;  // "MoneyDeposited", "MoneyWithdrawn" etc.
        public decimal Amount { get; set; }
        public string Payload { get; set; } = null!;     // full event data as JSON
        public string? RelatedAccountId { get; set; }    // used for transfers
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int Version { get; set; }                 // order of events per account
    }
}
