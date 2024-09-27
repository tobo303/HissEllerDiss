using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HissEllerDissApi.Models.HissEllerDiss;

public class HissEllerDissEntry : IHissEllerDissEntry
{
    public HissEllerDissEntry()
    {
        Id = 0;
        Name = string.Empty;
        Likes = 0;
    }

    public HissEllerDissEntry(string name, long likes)
    {
        Name = name;
        Likes = likes;
    }

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    [StringLength(255)]
    public string Name { get; set; }

    [JsonPropertyName("likes")]
    [Range(-1000, 1000)]
    public long Likes { get; set; }
}