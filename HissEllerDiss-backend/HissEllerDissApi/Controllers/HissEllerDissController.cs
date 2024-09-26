using HissEllerDissApi.Models.HissEllerDiss;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace HissEllerDissApi.Controllers;

// Synchronous controller for HissEllerDiss entries.

[Route("api/hissellerdiss")]
[ApiController]
public class HissEllerDissController(HissEllerDissContext dbContext) : ControllerBase
{
    [HttpGet]
    public IEnumerable<IHissEllerDissEntry> Get(IConfiguration configuration)
    {
        var maxEntries = configuration.GetValue("MaxEntries", 10);
        return dbContext.Entries.Take(maxEntries);
    }

    [HttpGet("{id}")]
    public ActionResult<IHissEllerDissEntry> Get(int id)
    {
        var entry = dbContext.Entries.Find(id);
        if (entry is null)
        {
            return NotFound();
        }

        return Ok(entry);
    }

    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    public IActionResult Post([FromBody] HissEllerDissEntry? entry)
    {
        if (entry is null)
        {
            return BadRequest("Invalid entry data.");
        }

        dbContext.Entries.Add(entry);
        dbContext.SaveChanges();
        return CreatedAtAction(nameof(Get), new { id = entry.Id }, entry);
    }

    [HttpPut("{id}")]
    public IActionResult Put(int id, long likes)
    {
        // find entry
        var entry = dbContext.Entries.Find(id);
        if (entry is null) return NotFound();
        
        entry.Likes += likes;

        dbContext.Entries.Update(entry);
        dbContext.SaveChanges();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        var entry = dbContext.Entries.Find(id);
        if (entry is null) return NotFound();

        dbContext.Entries.Remove(entry);
        dbContext.SaveChanges();
        return NoContent();
    }
}