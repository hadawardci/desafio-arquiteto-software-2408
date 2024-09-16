using CashFlow.Entries.Domain.SharedKernel.Exceptions;

namespace CashFlow.Entries.Domain.EntryRootAggregation
{
    public class Entry(decimal amount, string? description, DateTime accountingDate, DateTime registeredAt, string id = null)
    {
        public decimal Amount { get; private set; } = amount;
        public string? Description { get; private set; } = description;
        public DateTime AccountingDate { get; private set; } = accountingDate;
        public DateTime RegisteredAt { get; private set; } = registeredAt;
        public string Id { get; private set; } = id;
        public long Timestamp { get; private set; } = registeredAt.Ticks;

        public static Entry Create(decimal amount, string? description, DateTime? accountingDate)
        {
            var value = Math.Round(amount, 2);
            if (value == 0)
            {
                const string message_invalid_amount = "Informe uma quantia válida";
                throw new UnprocessableContentException(message_invalid_amount);
            }
            var now = DateTime.Now;
            return new(value, description, (accountingDate ?? now).Date, now);
        }

        public static Entry Create(decimal amount, string? description, object accountingDate)
        {
            throw new NotImplementedException();
        }
    }
}
