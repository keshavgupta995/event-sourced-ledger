namespace LedgerApi.Data.Models
{
    public class AccountBalance
    {
        public string AccountId { get; set; } = null!;
        public string OwnerName { get; set; } = null!;
        public decimal Balance { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}
