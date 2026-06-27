namespace LedgerApi.Domain.Events
{
    public class AccountOpenedEvent
    {
        public string AccountId { get; set; } = null!;
        public string OwnerName { get; set; } = null!;
        public decimal InitialDeposit { get; set; }
        public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    }
}
