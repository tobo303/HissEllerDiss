using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace HissEllerDissApi.Models.HissEllerDiss;

[Table("Entries")]
[PrimaryKey("Id")]
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

    [Key]
    [Column(Order = 1)]
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [StringLength(255)]
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [Range(-1000, 1000)]
    [JsonPropertyName("likes")]
    public long Likes { get; set; }
}