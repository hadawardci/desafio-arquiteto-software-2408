namespace CashFlow.Consolidated.Domain.Gateways
{
    public interface IEntriesUpdateEventGateway
    {
        //void Consume<T>(MessageHandler<T> onMessageHandler) where T : class;
        void ReceiveMessages();
    }

    //public delegate void MessageHandler<T>(T message) where T : class;

    

}
