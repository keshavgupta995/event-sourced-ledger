namespace LedgerApi.DTOs
{
    public class LedgerEventResponseDto
    {
        public int Id { get; set; }
        public string EventType { get; set; } = null!;
        public decimal Amount { get; set; }
        public string Payload { get; set; } = null!;
        public string? RelatedAccountId { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Version { get; set; }
    }
}
