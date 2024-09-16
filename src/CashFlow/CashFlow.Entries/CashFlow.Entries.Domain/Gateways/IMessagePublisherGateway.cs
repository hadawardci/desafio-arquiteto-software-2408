namespace CashFlow.Entries.Domain.Gateways
{
    public interface IMessagePublisherGateway
    {
        void PublishAsync<T>(T message);
        void PublishFollowUpAsync<T>(T message, uint maxMessages = default);
    }
}
