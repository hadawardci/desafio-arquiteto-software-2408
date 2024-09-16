using CashFlow.Consolidated.Domain.SharedKernel.Events;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace CashFlow.Consolidated.Infra.Data.Gateways
{
    public class EntriesUpdateEventConsumer(IConnection _connection, ILogger<EntriesUpdateEventConsumer> _logger, IEntriesUpdateEventHandler _eventHandler) : BackgroundService, IEntriesUpdateEventGateway
    {
        private IModel? _channel;
        private const string _single_active_consumer = "x-single-active-consumer";
        private const bool _single_consumer_value = true;
        private IModel ChannelInstance(IConnection _connection, string queue)
        {
            if (!_connection.IsOpen)
            {
                _logger.LogCritical("Não é possível receber mensagens de {queue}, conexão encerrada!", queue);
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

        public void ReceiveMessages()
        {
            var queue = nameof(EntriesUpdateEvent);
            var channel = ChannelInstance(_connection, queue);
            if (channel == null) return;
            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                try
                {
                    byte[] body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var messageEvent = JsonConvert.DeserializeObject<EntriesUpdateEvent>(message)!;
                    _eventHandler.Handle(messageEvent).Wait();
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    _logger.LogCritical("Erro na queue {queue} ao processar {deliveryTag}: {@ex}", queue, ea.DeliveryTag, ex);
                    channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                }

            };
            channel.BasicConsume(queue: queue,
                                 autoAck: false,
                                 consumer: consumer);

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            ReceiveMessages();
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(100000, stoppingToken);
            }
            _logger.LogWarning("Disposing chanel.");
            _channel?.Dispose();
        }
    }

}
