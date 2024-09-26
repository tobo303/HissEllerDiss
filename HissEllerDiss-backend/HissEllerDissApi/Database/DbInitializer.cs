using HissEllerDissApi.Models.HissEllerDiss;

namespace HissEllerDissApi.Database;

public static class DbInitializer
{
    public static void SeedDatabase(HissEllerDissContext context)
    {
        // Look for any existing entries.
        if (context.Entries.Any())
        {
            return;   // DB has been seeded
        }

        var entries = new HissEllerDissEntry[]
        {
            new HissEllerDissEntry("Cloud Developer 2024", 1000),
            new HissEllerDissEntry("RabbitMQ", 10)
        };

        context.Entries.AddRange(entries);
        context.SaveChanges();
    }
}
