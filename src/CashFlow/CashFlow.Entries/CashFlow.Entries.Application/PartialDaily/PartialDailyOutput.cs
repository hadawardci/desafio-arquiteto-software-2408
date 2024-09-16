namespace CashFlow.Entries.Application.PartialDaily
{
    public record PartialDailyOutput(decimal TotalAmount, DateTime RegisteredAt, long Timestamp);
}
