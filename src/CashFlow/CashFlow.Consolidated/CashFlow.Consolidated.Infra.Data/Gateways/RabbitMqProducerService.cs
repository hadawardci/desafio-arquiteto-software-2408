using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace CashFlow.Consolidated.Infra.Data.Gateways
{
    public class RabbitMqProducerService(IConnection _connection) : IMessageProducerGateway
    {
        public void PublishAsync<T>(T message)
        {
            _connection.CreateModel().QueueDeclare(queue: "queue_name", durable: false,
                exclusive: false, autoDelete: false, arguments: null);


            if (!_connection.IsOpen)
                return;
            using var channel = _connection.CreateModel();
            var messageBody = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            channel.BasicPublish(exchange: "exchange_name", routingKey: "route_key", body: messageBody);
        }

        private const uint _max_messages = 3;
        public void PublishFollowUpAsync<T>(T message, uint maxMessages = _max_messages)
        {

            var queue = typeof(T).Name;
            _connection.CreateModel().QueueDeclare(queue: queue, durable: true,
                exclusive: false, autoDelete: false, arguments: null);

            if (!_connection.IsOpen)
                return;
            using var channel = _connection.CreateModel();
            var messagesCount = channel.MessageCount(queue);
            Console.WriteLine(messagesCount);
            if (messagesCount < _max_messages)
            {
                var messageBody = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
                channel.BasicPublish(exchange: string.Empty, routingKey: queue, body: messageBody);
            }
        }
    }

}
