using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace CashFlow.Entries.Infra.Data.Gateways
{
    public class RabbitMqProducerService(IConnection _connection, ILogger<RabbitMqProducerService> _logger) : IMessagePublisherGateway
    {
        public void PublishAsync<T>(T message)
        {
            var queue = typeof(T).Name;
            IModel channel = ChannelInstance<T>(_connection, queue);
            var messageBody = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            channel.BasicPublish(exchange: string.Empty, routingKey: queue, body: messageBody);
        }

        private const uint _max_messages = 3;
        public void PublishFollowUpAsync<T>(T message, uint maxMessages = _max_messages)
        {
            var queue = typeof(T).Name;
            var channel = ChannelInstance<T>(_connection, queue);           
            var messagesCount = channel.MessageCount(queue);
            if (messagesCount < _max_messages)
            {
                var messageBody = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
                channel.BasicPublish(exchange: string.Empty, routingKey: queue, body: messageBody);
            }
            channel.Close();
        }

        private IModel? _channel;
        private const string _single_active_consumer = "x-single-active-consumer";
        private const bool _single_consumer_value = true;

        private IModel ChannelInstance<T>(IConnection _connection, string queue)
        {

            if (!_connection.IsOpen)
            {
                _logger.LogCritical("Não é possível publicar em {queue}, conexão encerrada!", queue);
                throw new Exception("Conexão encerrada!");
            }
            if (_channel == null || _channel.IsClosed)
                _channel = _connection.CreateModel();

            var arguments = new Dictionary<string, object> {
                {
                    _single_active_consumer,
                    _single_consumer_value
                }
            };
            _channel.QueueDeclare(queue: queue, durable: true,
                            exclusive: false, autoDelete: false, arguments: arguments);
            return _channel;
        }
    }

}
