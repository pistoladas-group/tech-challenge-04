using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TechNews.Common.Library.Messages;
using TechNews.Common.Library.Messages.Events;

namespace TechNews.Common.Library.MessageBus.Brokers.RabbitMQ;

public class RabbitMQMessageBus : IMessageBus, IDisposable
{
    private IModel _channel { get; }
    private IConnection _connection { get; }

    public RabbitMQMessageBus(RabbitMQMessageBusParameters parameters)
    {
        var factory = new ConnectionFactory
        {
            HostName = parameters.HostName,
            UserName = parameters.UserName,
            Password = parameters.Password,
            VirtualHost = parameters.VirtualHost
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    public void Publish<T>(T message) where T : IEvent
    {
        var eventName = typeof(T).Name;

        var serializedMessage = JsonSerializer.Serialize(message);
        var encodedMessage = Encoding.UTF8.GetBytes(serializedMessage);

        var deadLetterQueueName = $"{eventName}-DeadLetter";

        // TODO: Fazer os declares e binds apenas uma vez

        _channel.ExchangeDeclare(
            exchange: eventName,
            type: ExchangeType.Fanout,
            durable: true,
            autoDelete: false,
            arguments: null
        );

        _channel.QueueDeclare(
            queue: deadLetterQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        _channel.QueueBind(
            queue: deadLetterQueueName,
            exchange: eventName,
            routingKey: string.Empty,
            arguments: null
        );

        _channel.BasicPublish(
            exchange: eventName,
            routingKey: string.Empty,
            basicProperties: null,
            body: encodedMessage
        );
    }

    // UserRegisteredEvent
    public void Consume<T>(string queueName, Action<T?> executeAfterConsumed) where T : IEvent
    {
        var eventName = typeof(T).Name;
        var deadLetterQueueName = $"{eventName}-DeadLetter";

        // TODO: Fazer os declares e binds apenas uma vez

        _channel.QueueDeclare(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        _channel.QueueBind(
            queue: queueName,
            exchange: eventName,
            routingKey: string.Empty,
            arguments: null
        );

        // TODO: Mover mensagens de deadletter para a fila principal

        _channel.QueueUnbind(
            queue: deadLetterQueueName,
            exchange: eventName,
            routingKey: string.Empty,
            arguments: null);

        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += (sender, eventArgs) =>
        {
            var encodedBody = eventArgs.Body.ToArray();
            var decodedBody = Encoding.UTF8.GetString(encodedBody);
            var message = JsonSerializer.Deserialize<T>(decodedBody);

            try
            {
                executeAfterConsumed(message);
            }
            catch (Exception ex)
            {
                _channel.QueueDeclare(
                    queue: $"{queueName}-Error",
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );

                var serializedMessage = JsonSerializer.Serialize(new ErrorMessage()
                {
                    Description = ex.Message,
                    StackTrace = ex.StackTrace,
                    Message = decodedBody
                });
                
                var encodedMessage = Encoding.UTF8.GetBytes(serializedMessage);

                _channel.BasicPublish(
                    exchange: string.Empty,
                    routingKey: $"{queueName}-Error",
                    basicProperties: null,
                    body: encodedMessage
                );
            }
        };

        _channel.BasicConsume(
            queue: queueName,
            autoAck: true, //automatically remove message from queue when processed
            consumer: consumer
        );
    }

    public void Dispose()
    {
        _connection.Dispose();
        _channel.Dispose();
    }
}