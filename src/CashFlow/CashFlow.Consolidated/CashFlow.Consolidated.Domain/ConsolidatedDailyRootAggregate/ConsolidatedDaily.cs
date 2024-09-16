namespace CashFlow.Consolidated.Domain.ConsolidatedDailyRootAggregate
{
    public class ConsolidatedDaily(decimal totalAmount, long timestamp, DateTime date, string id = null)
    {
        public decimal TotalAmount { get; private set; } = totalAmount;
        public DateTime Date { get; private set; } = date;
        public long Timestamp { get; private set; } = timestamp;
        public string Id { get; private set; } = id;

        public static ConsolidatedDaily Create(decimal totalAmount, long timestamp, DateTime registeredAt)
            => new(totalAmount, timestamp, registeredAt.Date);

        public bool Update(long timestamp, decimal partialAmount)
        {
            var isGreaterDate = timestamp > Timestamp;
            if (isGreaterDate)
            {
                TotalAmount += partialAmount;
                Timestamp = Timestamp;
            }
            return isGreaterDate;
        }
    }

}
