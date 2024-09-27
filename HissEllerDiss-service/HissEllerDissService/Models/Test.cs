using System.Text.Json.Serialization;

namespace HissEllerDissService.Models;

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