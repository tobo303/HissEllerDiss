using RabbitMQ.Client;

namespace HissEllerDissApi.RabbitMq;

public class RabbitConnection : IRabbitConnection, IDisposable
{
    private readonly IConnection _connection;

    public RabbitConnection()
    {
        var factory = new ConnectionFactory
        {
            HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost",
            UserName = "hissellerdiss",
            Password = "rabbitmq"
        };

        Console.WriteLine($"RabbitMQ: {Environment.GetEnvironmentVariable("RABBITMQ_HOST")}");

        _connection = factory.CreateConnection();
    }

    public IConnection Connection => _connection;

    public void Dispose()
    {
        _connection?.Dispose();
    }
}