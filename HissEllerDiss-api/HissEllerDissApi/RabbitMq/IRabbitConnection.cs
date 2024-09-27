using RabbitMQ.Client;

namespace HissEllerDissApi.RabbitMq
{
    public interface IRabbitConnection
    {
        IConnection Connection { get; }
    }
}
