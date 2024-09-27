using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace HissEllerDissApi.RabbitMq;

public interface IMessageProducer
{
    void SendMessage<T>(string queue, T message);
}

public class RabbitMessageProducer : IMessageProducer
{
    private readonly IRabbitConnection _connection;

    public RabbitMessageProducer(IRabbitConnection connection)
    {
        _connection = connection;
    }

    public void SendMessage<T>(string queue, T message)
    {
        using var channel = _connection.Connection.CreateModel();
        channel.QueueDeclare(queue, exclusive: false, durable: true, autoDelete: false);

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
        channel.BasicPublish("", queue, null, body);
    }
}