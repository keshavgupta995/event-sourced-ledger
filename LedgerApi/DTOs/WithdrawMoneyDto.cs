namespace LedgerApi.DTOs
{
    public class WithdrawMoneyDto
    {
        public string AccountId { get; set; } = null!;
        public decimal Amount { get; set; }
        public string Reason { get; set; } = null!;
    }
}
