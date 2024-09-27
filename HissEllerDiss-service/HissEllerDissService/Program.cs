using System.Text;
using System.Text.Json;
using HissEllerDissApi.Database;
using HissEllerDissApi.Models.HissEllerDiss;
using HissEllerDissService.Models;
using HissEllerDissService.RabbitMq;
using HissEllerDissService.Services;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<HostOptions>(hostOptions =>
{
    hostOptions.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<HissEllerDissContext>(options =>
{
    options.UseInMemoryDatabase(builder.Configuration.GetConnectionString("HissEllerDissDatabase") ?? string.Empty);
});
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
//builder.Services.AddHostedService<HissEllerDissCreateRequestService>();

var  MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy  =>
        {
            policy.WithOrigins("*");
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<HissEllerDissContext>();
    context.Database.EnsureCreated();
    DbInitializer.SeedDatabase(context);
}

app.UseCors(MyAllowSpecificOrigins);

// Not used in this example project
//app.UseAuthorization();

app.MapControllers();

var connection = new RabbitConnection();
using var channel = connection.Connection.CreateModel();
channel.QueueDeclare("createQueue", true, false, false, new Dictionary<string, object>());

using var rpcScope = app.Services.CreateScope();
var rpcContext = rpcScope.ServiceProvider.GetRequiredService<HissEllerDissContext>();

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);

    if (message.Contains("PATCH"))
    {
        var parts = message.Replace('"', ' ').Trim().Replace('\\', ' ').Split(' ');
        var idLikes = parts[1];
        var idLikesParts = idLikes.Split(':');
        var id = int.Parse(idLikesParts[0]);
        var likes = int.Parse(idLikesParts[1]);

        Console.WriteLine($"Received PATCH request for id: {id}, likes: {likes}");
        var patchEntry = rpcContext.Entries.Find(id);
        if (patchEntry == null)
        {
            Console.WriteLine($"Entry with id {id} not found");
            return;
        }

        patchEntry.Likes += likes;
        rpcContext.SaveChanges();
        return;
    }

    var request = JsonSerializer.Deserialize<HissEllerDissCreateRequest>(message);

    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<HissEllerDissContext>();

    var entry = new HissEllerDissEntry
    {
        Name = request.Name,
        Likes = request.Likes
    };

    context.Entries.Add(entry);
    context.SaveChanges();

    var response = new HissEllerDissCreateResponse
    {
        Id = entry.Id
    };

    //var producer = new RabbitMessageProducer(connection, "createResponseQueue");
    //producer.SendMessage(response);
};

channel.BasicConsume("createQueue", true, "", false, false, new Dictionary<string, object>(), consumer);


channel.QueueDeclare(queue: "rpc_rabbit_queue",
    durable: false,
    exclusive: false,
    autoDelete: false,
    arguments: null);
channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

var rpcConsumer = new EventingBasicConsumer(channel);
channel.BasicConsume(queue: "rpc_rabbit_queue",
    autoAck: false,
    consumer: rpcConsumer);

Console.WriteLine(" [x] Awaiting RPC requests");

rpcConsumer.Received += (model, ea) =>
{
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
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<HissEllerDissContext>();

        if (message.Contains("GETALL"))
        {
            var response = context.Entries.ToList();
            
            Console.WriteLine($" [.] Return all entries");

            var responseBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response));
            local.Model.BasicPublish(exchange: string.Empty,
                routingKey: props.ReplyTo,
                basicProperties: replyProps,
                body: responseBytes);
            local.Model.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        }
        else if (message.Contains("GET"))
        {
            var id = Convert.ToInt32(message.Substring(4).Trim());
            var entry = context.Entries.Find(id);
            var response = entry;
            var responseBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response));
            local.Model.BasicPublish(exchange: string.Empty,
                routingKey: props.ReplyTo,
                basicProperties: replyProps,
                body: responseBytes);
            local.Model.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        }
        else if (message.Contains("PATCH"))
        {
            // Extract the id and likes from the message
            var parts = message.Split(' ');
            var idLikes = parts[1];
            var idLikesParts = idLikes.Split(':');
            var id = int.Parse(idLikesParts[0]);
            var likes = int.Parse(idLikesParts[1]);

            Console.WriteLine($"Received PATCH request for id: {id}, likes: {likes}");
            var entry = context.Entries.Find(id);
            if (entry == null)
            {
                Console.WriteLine($"Entry with id {id} not found");
                local.Model.BasicPublish(exchange: string.Empty,
                    routingKey: props.ReplyTo,
                    basicProperties: replyProps,
                    body: Array.Empty<byte>());
                local.Model.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);

                return;
            }

            entry.Likes += likes;
            context.SaveChanges();
            local.Model.BasicPublish(exchange: string.Empty,
                routingKey: props.ReplyTo,
                basicProperties: replyProps,
                body: Array.Empty<byte>());
            local.Model.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);

        }
        else if (JsonSerializer.Deserialize<HissEllerDissCreateRequest>(message) is { } request)
        {
            var entry = new HissEllerDissEntry
            {
                Name = request.Name,
                Likes = request.Likes
            };


            var response = new HissEllerDissCreateResponse();
            context.Entries.Add(entry);
            context.SaveChanges();
            response.Id = entry.Id;

            Console.WriteLine($" [.] Create {request.Name}: {response.Id}");

            var responseBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response));
            local.Model.BasicPublish(exchange: string.Empty,
                routingKey: props.ReplyTo,
                basicProperties: replyProps,
                body: responseBytes);
            local.Model.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        }
    }
    catch (Exception e)
    {
        Console.WriteLine($" [.] {e.Message}");
    }
};


app.Run();

Console.WriteLine("Closing down");