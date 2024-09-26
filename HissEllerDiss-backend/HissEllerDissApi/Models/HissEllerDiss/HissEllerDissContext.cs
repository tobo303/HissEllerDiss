using Microsoft.EntityFrameworkCore;

namespace HissEllerDissApi.Models.HissEllerDiss;

public class HissEllerDissContext(DbContextOptions<HissEllerDissContext> options) : DbContext(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            optionsBuilder.UseInMemoryDatabase("hissellerdiss_db");

        base.OnConfiguring(optionsBuilder);
    }

    public DbSet<HissEllerDissEntry> Entries { get; set; } = null!;
}