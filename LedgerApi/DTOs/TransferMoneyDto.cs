namespace LedgerApi.DTOs
{
    public class TransferMoneyDto
    {
        public string FromAccountId { get; set; } = null!;
        public string ToAccountId { get; set; } = null!;
        public decimal Amount { get; set; }
        public string Reason { get; set; } = null!;
    }
}
