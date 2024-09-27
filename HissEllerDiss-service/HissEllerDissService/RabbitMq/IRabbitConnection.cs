using RabbitMQ.Client;

namespace HissEllerDissService.RabbitMq
{
    public interface IRabbitConnection
    {
        IConnection Connection { get; }
    }
}
