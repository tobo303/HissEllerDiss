using HissEllerDissApi.Models.HissEllerDiss;
using HissEllerDissService.Models;

namespace HissEllerDissService.Services
{
    //public class HissEllerDissCreateRequestService(IServiceProvider provider) : BackgroundService
    //{
    //    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    //    {
    //        using var scope = provider.CreateScope();
    //        var bus = scope.ServiceProvider.GetRequiredService<IBus>();
    //        bus.Rpc.RespondAsync<HissEllerDissCreateRequest, HissEllerDissCreateResponse>(ProcessCreateRequest);
            
    //        return Task.CompletedTask;
    //    }

    //    private HissEllerDissCreateResponse ProcessCreateRequest(HissEllerDissCreateRequest request)
    //    {
    //        var entry = new HissEllerDissEntry
    //        {
    //            Name = request.Name,
    //            Likes = request.Likes
    //        };

    //        using var scope = provider.CreateScope();
    //        var context = scope.ServiceProvider.GetRequiredService<HissEllerDissContext>();

    //        context.Entries.Add(entry);
    //        context.SaveChanges();

    //        return new HissEllerDissCreateResponse
    //        {
    //            Id = entry.Id
    //        };
    //    }
    //}
}
