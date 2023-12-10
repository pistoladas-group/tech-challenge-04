using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TechNews.Common.Library.MessageBus.EventMessages.Base;

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
    
    public void Publish<T>(T message)
    {
        var eventName = typeof(T).Name;

        var serializedMessage = JsonSerializer.Serialize(message);
        var encodedMessage = Encoding.UTF8.GetBytes(serializedMessage);

        var deadLetterQueueName = $"{eventName}-DeadLetter";

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

    public void Consume<T>(string queueName, Action<T?> executeAfterConsumed) where T : IntegrationEvent
    {
        _channel.QueueDeclare(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );
        
        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += (sender, eventArgs) =>
        {
            var encodedMessage = eventArgs.Body.ToArray();
            var decodedMessage = Encoding.UTF8.GetString(encodedMessage);

            var resultMessage = JsonSerializer.Deserialize<T>(decodedMessage);
            
            executeAfterConsumed(resultMessage);
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
    // public void Test()
    // {
    //     Consume<Order>("teste", x =>
    //     {
    //         Console.WriteLine("teste");
    //     });
    // }
}