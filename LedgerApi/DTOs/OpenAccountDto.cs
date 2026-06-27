namespace LedgerApi.DTOs
{
    public class OpenAccountDto
    {
        public string OwnerName { get; set; } = null!;
        public decimal InitialDeposit { get; set; }
    }
}
