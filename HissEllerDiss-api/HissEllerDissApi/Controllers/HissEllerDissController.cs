using System.Net.Http.Headers;
using HissEllerDissApi.Models.HissEllerDiss;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Text;
using System.Text.Json;

namespace HissEllerDissApi.Controllers;

// Synchronous controller for HissEllerDiss entries.

[Route("api/hissellerdiss")]
[ApiController]
public class HissEllerDissController() : ControllerBase
{
    [HttpGet]
    public IEnumerable<IHissEllerDissEntry> Get(IConfiguration configuration)
    {
        using var httpClient = new HttpClient();
        using var response = httpClient.GetAsync(configuration["HissEllerDissApi:Url"]).Result;
        response.EnsureSuccessStatusCode();
        var content = response.Content.ReadAsStringAsync().Result;
        var list = JsonSerializer.Deserialize<IEnumerable<HissEllerDissEntry>>(content);
        return list ?? Array.Empty<HissEllerDissEntry>();
    }

    [HttpGet("{id}")]
    public ActionResult<IHissEllerDissEntry> Get(int id, IConfiguration configuration)
    {
        using var httpClient = new HttpClient();
        using var response = httpClient.GetAsync($"{configuration["HissEllerDissApi:Url"]}/{id}").Result;
        response.EnsureSuccessStatusCode();
        var content = response.Content.ReadAsStringAsync().Result;
        var entry = JsonSerializer.Deserialize<HissEllerDissEntry>(content);
        if (entry is null)
            return NotFound();

        return entry;
    }

    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    public IActionResult Post([FromBody] HissEllerDissEntry? entry, IConfiguration configuration)
    {
        if (entry is null)
            return BadRequest();

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));

        var jsonContent = JsonSerializer.Serialize(entry);
        using var httpContent = new StringContent(jsonContent, Encoding.UTF8, MediaTypeNames.Application.Json);
        
        using var response = httpClient.PostAsync(configuration["HissEllerDissApi:Url"], httpContent).Result;
        response.EnsureSuccessStatusCode();

        var content = response.Content.ReadAsStringAsync().Result;
        var responseEntry = JsonSerializer.Deserialize<HissEllerDissEntry>(content);

        if (responseEntry is null)
            return NotFound();

        return CreatedAtAction(nameof(Get), new { id = responseEntry.Id }, responseEntry);
    }

    [HttpPut("{id}")]
    public IActionResult Put(int id, long likes, IConfiguration configuration)
    {
        if (likes == 0)
            return BadRequest();

        using var httpClient = new HttpClient();
        using var response = httpClient.GetAsync($"{configuration["HissEllerDissApi:Url"]}/{id}").Result;
        response.EnsureSuccessStatusCode();
        var content = response.Content.ReadAsStringAsync().Result;
        var entry = JsonSerializer.Deserialize<HissEllerDissEntry>(content);
        if (entry is null)
            return NotFound();

        var updatedEntry = likes > 0 ? entry.IncreaseVote(likes) : entry.DecreaseVote(likes);
        var jsonContent = JsonSerializer.Serialize(updatedEntry.Likes);
        using var httpContent = new StringContent(jsonContent, Encoding.UTF8, MediaTypeNames.Application.Json);

        using var putResponse = httpClient.PutAsync($"{configuration["HissEllerDissApi:Url"]}/{id}", httpContent).Result;
        putResponse.EnsureSuccessStatusCode();

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

