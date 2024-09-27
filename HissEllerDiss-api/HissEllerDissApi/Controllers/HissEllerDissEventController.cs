using System.Net.Http.Headers;
using HissEllerDissApi.Models.HissEllerDiss;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Text;
using System.Text.Json.Serialization;
using HissEllerDissApi.RabbitMq;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace HissEllerDissApi.Controllers;

// Synchronous controller for HissEllerDiss entries.

[Route("api/hissellerdiss_event")]
[ApiController]
public class HissEllerDissEventController (IServiceProvider provider, IMessageProducer producer) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<IHissEllerDissEntry>> Get(IConfiguration configuration)
    {
        var rpc = new RabbitRpc();
        var response = await rpc.CallAsync("GETALL");
        return JsonSerializer.Deserialize<IEnumerable<HissEllerDissEntry>>(response) ?? Array.Empty<HissEllerDissEntry>();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<IHissEllerDissEntry>> Get(int id, IConfiguration configuration)
    {
        var rpc = new RabbitRpc();
        var response = await rpc.CallAsync($"GET {id}");
        return JsonSerializer.Deserialize<HissEllerDissEntry>(response) ?? new HissEllerDissEntry();
    }

    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Post([FromBody] HissEllerDissEntry? entry)
    {
        if (entry is null)
            return BadRequest();

        var rpc = new RabbitRpc();
        var request = new HissEllerDissCreateRequest { Name = entry.Name, Likes = entry.Likes };
        var response = await rpc.CallAsync(JsonSerializer.Serialize(request), CancellationToken.None);

        var responseEntry = JsonSerializer.Deserialize<HissEllerDissCreateResponse>(response);
        if (responseEntry is null)
            return BadRequest();

        return CreatedAtAction(nameof(Get), new { id = responseEntry.Id }, entry);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Patch(int id, long likes, IMessageProducer producer)
    {
        if (likes == 0)
            return BadRequest();

        await Task.Delay(0); // Start a task and await it

        producer.SendMessage("createQueue", $"PATCH {id}:{likes}");
        return NoContent();
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(int id, IConfiguration configuration)
    {
        using var httpClient = new HttpClient();
        using var response = httpClient.DeleteAsync($"{configuration["HissEllerDissApi:Url"]}/{id}").Result;
        response.EnsureSuccessStatusCode();
        return Ok();
    }
}

public class HissEllerDissCreateRequest
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    [JsonPropertyName("likes")]
    public long Likes { get; set; }
}

public class HissEllerDissCreateResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
}
