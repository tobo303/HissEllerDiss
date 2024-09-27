using System.Text;
using System.Text.Json;
using HissEllerDissApi.Models.HissEllerDiss;
using HissEllerDissService.Models;
using HissEllerDissService.RabbitMq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace HissEllerDissService.Services;

public class RabbitRpcService
{
    public void StartService(HissEllerDissContext context)
    {
        var connection = new RabbitConnection();
        var channel = connection.Connection.CreateModel();

        channel.QueueDeclare(queue: "rpc_rabbit_queue",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);
        channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

        var consumer = new EventingBasicConsumer(channel);
        channel.BasicConsume(queue: "rpc_rabbit_queue",
            autoAck: false,
            consumer: consumer);
        
        Console.WriteLine(" [x] Awaiting RPC requests");

        consumer.Received += (model, ea) =>
        {
            var response = new HissEllerDissCreateResponse();
            if (model is not EventingBasicConsumer local)
            {
                return;
            }
            
            var body = ea.Body.ToArray();
            var props = ea.BasicProperties;
            var replyProps = local.Model.CreateBasicProperties();
            replyProps.CorrelationId = props.CorrelationId;

            try
            {
                var message = Encoding.UTF8.GetString(body);
                var request = JsonSerializer.Deserialize<HissEllerDissCreateRequest>(message);

                var entry = new HissEllerDissEntry
                {
                    Name = request.Name,
                    Likes = request.Likes
                };

                context.Entries.Add(entry);
                context.SaveChanges();
                response.Id = entry.Id;

                Console.WriteLine($" [.] Create {request.Name}: {response.Id}");
            }
            catch (Exception e)
            {
                Console.WriteLine($" [.] {e.Message}");
                response.Id = 0;
            }
            finally
            {
                var responseBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response));
                local.Model.BasicPublish(exchange: string.Empty,
                    routingKey: props.ReplyTo,
                    basicProperties: replyProps,
                    body: responseBytes);
                local.Model.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            }
        };
    }
}