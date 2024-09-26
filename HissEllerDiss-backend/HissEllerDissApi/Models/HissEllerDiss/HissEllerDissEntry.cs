using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
    public int Id { get; set; }

    [StringLength(255)]
    public string Name { get; set; }

    [Range(-1000, 1000)]
    public long Likes { get; set; }
}