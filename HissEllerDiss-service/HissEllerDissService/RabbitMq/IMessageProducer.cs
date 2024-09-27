using System.Text;
using System.Text.Json;

namespace HissEllerDissService.RabbitMq;

public interface IMessageProducer
{
    void SendMessage<T>(T message);
}

public class RabbitMessageProducer : IMessageProducer
{
    private readonly IRabbitConnection _connection;

    public RabbitMessageProducer(IRabbitConnection connection, string queueName)
    {
        _connection = connection;
    }

    public void SendMessage<T>(T message)
    {
        //using var channel = _connection.Connection.CreateModel();
        //channel.QueueDeclare("_queueName", exclusive: false);

        //var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
        //channel.BasicPublish("", _queueName, null, body);
    }
}