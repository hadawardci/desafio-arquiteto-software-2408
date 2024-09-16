namespace CashFlow.Consolidated.Domain.Gateways
{
    public interface IMessageProducerGateway
    {
        void PublishAsync<T>(T message);
        void PublishFollowUpAsync<T>(T message, uint maxMessages = default);
    }
}
