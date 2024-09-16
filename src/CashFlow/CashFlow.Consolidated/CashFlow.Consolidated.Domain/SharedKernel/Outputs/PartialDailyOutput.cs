namespace CashFlow.Consolidated.Domain.SharedKernel.Outputs
{
    public record PartialDailyOutput(decimal TotalAmount, DateTime RegisteredAt, long Timestamp)
    {
        public DateTime Date => RegisteredAt.Date;
    }
}
