namespace LedgerApi.DTOs
{
    public class AccountResponseDto
    {
        public string AccountId { get; set; } = null!;
        public string OwnerName { get; set; } = null!;
        public decimal Balance { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
